using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CodeWash.MessageExchange.Api.Hubs;

public class MessageHub : Hub
{
    private static readonly Dictionary<string, string> _connections = [];

    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (!string.IsNullOrEmpty(email))
        {
            _connections[email] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, email);
            Console.WriteLine($"User {email} connected with ConnectionId: {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var email = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (!string.IsNullOrEmpty(email))
        {
            _connections.Remove(email);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, email);
            Console.WriteLine($"User {email} disconnected.");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
