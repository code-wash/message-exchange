using CodeWash.MessageExchange.Dtos.QueryDtos;
using System.Net.Http.Json;

namespace CodeWash.MessageExchange.Client.Services;

public class UserService(IHttpClientFactory httpClientFactory)
    : BaseService(httpClientFactory)
{
    public async Task<List<GetUsersExceptCurrentVM>> GetUsersExceptCurrentAsync()
    {
        return await httpClient.GetFromJsonAsync<List<GetUsersExceptCurrentVM>>("api/users/except-current")
            ?? [];
    }
}
