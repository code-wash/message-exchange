using Moq;
using Microsoft.AspNetCore.SignalR;
using CodeWash.MessageExchange.Api.Hubs;
using System.Security.Claims;

namespace CodeWash.MessageExchange.Tests.ApiTests;

[TestFixture]
public class MessageHubTests
{
    private Mock<IHubCallerClients> _clientsMock;
    private Mock<IClientProxy> _clientProxyMock;
    private Mock<HubCallerContext> _hubContextMock;
    private MessageHub _messageHub;

    [SetUp]
    public void SetUp()
    {
        _clientsMock = new Mock<IHubCallerClients>();
        _clientProxyMock = new Mock<IClientProxy>();
        _hubContextMock = new Mock<HubCallerContext>();

        _clientsMock.Setup(clients => clients.Client(It.IsAny<string>()))
            .Returns(_clientProxyMock.Object as ISingleClientProxy);

        _messageHub = new MessageHub
        {
            Clients = _clientsMock.Object,
            Context = _hubContextMock.Object
        };
    }

    [TearDown]
    public void TearDown()
    {
        _messageHub?.Dispose();
    }

    [Test]
    public async Task OnConnectedAsync_ShouldStoreConnectionId()
    {
        string email = "test@example.com";
        string connectionId = "test-connection-123";

        List<Claim> claims = [new(ClaimTypes.Email, email)];
        ClaimsIdentity identity = new(claims, "mock");
        ClaimsPrincipal user = new(identity);

        _hubContextMock.SetupGet(c => c.User).Returns(user);
        _hubContextMock.SetupGet(c => c.ConnectionId).Returns(connectionId);

        await _messageHub.OnConnectedAsync();

        Assert.That(MessageHub.Connections.ContainsKey(email), Is.True);
        Assert.That(MessageHub.Connections[email], Is.EqualTo(connectionId));
    }

    [Test]
    public async Task OnDisconnectedAsync_ShouldRemoveConnectionId()
    {
        string email = "test@example.com";
        string connectionId = "test-connection-123";

        List<Claim> claims = [new(ClaimTypes.Email, email)];
        ClaimsIdentity identity = new(claims, "mock");
        ClaimsPrincipal user = new(identity);

        _hubContextMock.SetupGet(c => c.User).Returns(user);
        _hubContextMock.SetupGet(c => c.ConnectionId).Returns(connectionId);

        await _messageHub.OnConnectedAsync();

        await _messageHub.OnDisconnectedAsync(null);

        Assert.That(MessageHub.Connections.ContainsKey(email), Is.False);
    }
}
