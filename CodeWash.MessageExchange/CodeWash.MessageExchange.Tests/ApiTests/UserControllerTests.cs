using Moq;
using CodeWash.MessageExchange.Api.Controllers;
using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using Microsoft.AspNetCore.Http;

namespace CodeWash.MessageExchange.Tests.ApiTests;

[TestFixture]
public class UserControllerTests
{
    private Mock<IDbConnector> _dbConnectorMock;
    private UserController _userController;
    private ClaimsPrincipal _mockUser;

    [SetUp]
    public void SetUp()
    {
        _dbConnectorMock = new Mock<IDbConnector>();

        _mockUser = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Email, "test@example.com")], "mock"));

        _userController = new UserController(_dbConnectorMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _mockUser }
            }
        };
    }

    [Test]
    public async Task GetUsersExceptCurrentAsync_ShouldReturnUsers_WhenUserIsAuthenticated()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var users = new List<GetUsersExceptCurrentVM>
        {
            new(Guid.NewGuid(), "user1@example.com", 2),
            new(Guid.NewGuid(), "user2@example.com", 0)
        };

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new GetUserByEmailVM(currentUserId, "test@example.com", "passwordhash"));

        _dbConnectorMock.Setup(db => db.ExecuteQueryAsync(It.IsAny<GetUsersExceptCurrentSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(users);

        // Act
        var result = await _userController.GetUsersExceptCurrentAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(users));
    }

    [Test]
    public async Task GetUsersExceptCurrentAsync_ShouldReturnUnauthorized_WhenUserEmailIsMissing()
    {
        // Arrange
        _userController = new UserController(_dbConnectorMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // No user context
            }
        };

        // Act
        var result = await _userController.GetUsersExceptCurrentAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }
}
