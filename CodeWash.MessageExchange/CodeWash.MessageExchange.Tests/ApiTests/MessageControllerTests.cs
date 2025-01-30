using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CodeWash.MessageExchange.Api.Controllers;
using CodeWash.MessageExchange.Api.Hubs;
using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Dtos.ApiDtos.MessageDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CodeWash.MessageExchange.Tests.ApiTests;

[TestFixture]
public class MessageControllerTests
{
    private Mock<IDbConnector> _dbConnectorMock;
    private Mock<IHubContext<MessageHub>> _hubContextMock;
    private Mock<IClientProxy> _clientProxyMock;
    private MessageController _messageController;
    private ClaimsPrincipal _mockUser;

    [SetUp]
    public void SetUp()
    {
        _dbConnectorMock = new Mock<IDbConnector>();
        _hubContextMock = new Mock<IHubContext<MessageHub>>();
        _clientProxyMock = new Mock<IClientProxy>();

        _messageController = new MessageController(_dbConnectorMock.Object, _hubContextMock.Object);

        // Mock authenticated user
        _mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "mock"));

        _messageController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = _mockUser }
        };

        // Mock HubContext to simulate real-time messaging
        _hubContextMock.Setup(h => h.Clients.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
    }

    [Test]
    public async Task GetMessagesBetweenUsersAsync_ShouldReturnMessages_WhenValidUsersProvided()
    {
        // Arrange
        string userEmail = "receiver@example.com";
        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new GetUserByEmailVM(Guid.NewGuid(), userEmail, "hashedpassword"));

        _dbConnectorMock.Setup(db => db.ExecuteQueryAsync(It.IsAny<GetMessagesBetweenUsersSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(
                        [
                            new("Hello", DateTime.UtcNow, "test@example.com")
                        ]);

        // Act
        var result = await _messageController.GetMessagesBetweenUsersAsync(new GetMessagesBetweenUsersRequestDto(userEmail), CancellationToken.None);
        var okResult = result as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<List<GetMessagesBetweenUsersVM>>());
    }

    [Test]
    public async Task GetMessagesBetweenUsersAsync_ShouldReturnBadRequest_WhenUserEmailIsMissing()
    {
        // Act
        var result = await _messageController.GetMessagesBetweenUsersAsync(new GetMessagesBetweenUsersRequestDto(""), CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetMessagesBetweenUsersAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((GetUserByEmailVM?)null);

        // Act
        var result = await _messageController.GetMessagesBetweenUsersAsync(new GetMessagesBetweenUsersRequestDto("unknown@example.com"), CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task SendMessageAsync_ShouldSendMessage_WhenValidRequest()
    {
        // Arrange
        string senderEmail = "test@example.com";
        string receiverEmail = "receiver@example.com";
        string messageContent = "Hello, how are you?";

        var senderUser = new GetUserByEmailVM(Guid.NewGuid(), senderEmail, "hashedpassword");
        var receiverUser = new GetUserByEmailVM(Guid.NewGuid(), receiverEmail, "hashedpassword");

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.Is<GetUserByEmailSP>(q => q.Parameters["@Email"].ToString() == senderEmail), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(senderUser);

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.Is<GetUserByEmailSP>(q => q.Parameters["@Email"].ToString() == receiverEmail), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(receiverUser);

        _dbConnectorMock.Setup(db => db.ExecuteCommandAsync(It.IsAny<CreateMessageSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

        var request = new SendMessageDto(receiverEmail, messageContent);

        // Act
        var result = await _messageController.SendMessageAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo("Message sent successfully."));
    }

    [Test]
    public async Task SendMessageAsync_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange - No authenticated user
        _messageController.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var request = new SendMessageDto("receiver@example.com", "Hello!");

        // Act
        var result = await _messageController.SendMessageAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task SendMessageAsync_ShouldReturnBadRequest_WhenSenderAndReceiverAreSame()
    {
        // Arrange
        var request = new SendMessageDto("test@example.com", "Hello!");

        // Act
        var result = await _messageController.SendMessageAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task ReadMessagesAsync_ShouldMarkMessagesAsRead_WhenValidRequest()
    {
        // Arrange
        string senderEmail = "receiver@example.com";
        string currentUserEmail = "test@example.com";

        var senderUser = new GetUserByEmailVM(Guid.NewGuid(), senderEmail, "hashedpassword");
        var currentUser = new GetUserByEmailVM(Guid.NewGuid(), currentUserEmail, "hashedpassword");

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.Is<GetUserByEmailSP>(q => q.Parameters["@Email"].ToString() == senderEmail), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(senderUser);

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.Is<GetUserByEmailSP>(q => q.Parameters["@Email"].ToString() == currentUserEmail), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(currentUser);

        _dbConnectorMock.Setup(db => db.ExecuteCommandAsync(It.IsAny<ReadMessagesSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

        var request = new ReadMessagesRequestDto(senderEmail);

        // Act
        var result = await _messageController.ReadMessagesAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }


    [Test]
    public async Task ReadMessagesAsync_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange - No authenticated user
        _messageController.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var request = new ReadMessagesRequestDto("receiver@example.com");

        // Act
        var result = await _messageController.ReadMessagesAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }
}
