using CodeWash.MessageExchange.Dtos.ApiDtos.MessageDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using System.Net.Http.Json;

namespace CodeWash.MessageExchange.Client.Services;

public class MessageService(IHttpClientFactory httpClientFactory)
    : BaseService(httpClientFactory)
{
    public async Task<List<GetMessagesBetweenUsersVM>> GetMessagesBetweenUsersAsync(string email)
    {
        string requestUri = $"api/messages/between-users?UserEmail={Uri.EscapeDataString(email)}";

        return await httpClient.GetFromJsonAsync<List<GetMessagesBetweenUsersVM>>(requestUri) ?? [];
    }

    public async Task<bool> SendMessageAsync(string receiverEmail, string content)
    {
        SendMessageDto messageDto = new(receiverEmail, content);
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/messages/send", messageDto);

        return response.IsSuccessStatusCode;
    }
}
