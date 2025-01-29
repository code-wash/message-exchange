using CodeWash.MessageExchange.Dtos.ApiDtos.MessageDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace CodeWash.MessageExchange.Client.Services;

public class MessageService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
    : BaseService(httpClientFactory)
{
    private HubConnection? hubConnection;
    private readonly CustomAuthStateProvider authenticationStateProvider = (CustomAuthStateProvider)authenticationStateProvider;

    public event Action<string, string, string>? OnMessageReceived;

    public async Task StartConnectionAsync()
    {
        var token = await authenticationStateProvider.GetTokenAsync();

        if (hubConnection is null)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{httpClient.BaseAddress}messageHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<string, string, string>("ReceiveMessage", (senderEmail, message, timestamp) =>
            {
                Console.WriteLine($"[SignalR] Received message from {senderEmail}: {message} at {timestamp}");
                OnMessageReceived?.Invoke(senderEmail, message, timestamp);
            });
        }

        if (hubConnection.State == HubConnectionState.Disconnected)
        {
            Console.WriteLine("Starting SignalR connection...");
            await hubConnection.StartAsync();
            Console.WriteLine("SignalR Connected!");
        }
    }

    public async Task<List<GetMessagesBetweenUsersVM>> GetMessagesBetweenUsersAsync(string email)
    {
        string requestUri = $"api/messages/between-users?UserEmail={Uri.EscapeDataString(email)}";

        return await httpClient.GetFromJsonAsync<List<GetMessagesBetweenUsersVM>>(requestUri) ?? [];
    }

    public async Task<bool> SendMessageAsync(string receiverEmail, string message)
    {
        SendMessageDto messageDto = new(receiverEmail, message);
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/messages/send", messageDto);

        //if (hubConnection is { State: HubConnectionState.Connected })
        //{
        //    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        //    await hubConnection.SendAsync("SendMessage", receiverEmail, message, timestamp);
        //}

        return response.IsSuccessStatusCode;
    }
}
