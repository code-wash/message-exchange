using CodeWash.MessageExchange.Dtos.QueryDtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CodeWash.MessageExchange.Client.Services;

public class MessageService(HttpClient httpClient, LocalStorageService localStorageService)
{
    public async Task<List<GetMessagesBetweenUsersVM>> GetMessagesBetweenUsersAsync(string email)
    {
        var token = await localStorageService.GetItemAsync("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        string requestUri = $"api/messages/between-users?UserEmail={Uri.EscapeDataString(email)}";

        return await httpClient.GetFromJsonAsync<List<GetMessagesBetweenUsersVM>>(requestUri) ?? [];
    }
}
