using CodeWash.MessageExchange.Dtos.QueryDtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CodeWash.MessageExchange.Client.Services;

public class UserService(HttpClient httpClient, LocalStorageService localStorageService)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly LocalStorageService localStorageService = localStorageService;

    public async Task<List<GetUsersExceptCurrentVM>> GetUsersExceptCurrentAsync()
    {
        var token = await localStorageService.GetItemAsync("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await httpClient.GetFromJsonAsync<List<GetUsersExceptCurrentVM>>("api/users/except-current")
            ?? [];
    }
}
