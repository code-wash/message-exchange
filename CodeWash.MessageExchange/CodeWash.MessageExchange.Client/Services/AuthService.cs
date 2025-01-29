using System.Net.Http.Headers;
using System.Net.Http.Json;
using CodeWash.MessageExchange.Dtos.ApiDtos.AuthDtos;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeWash.MessageExchange.Client.Services;

public class AuthService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
    : BaseService(httpClientFactory)
{
    private readonly CustomAuthStateProvider authenticationStateProvider = (CustomAuthStateProvider)authenticationStateProvider;

    public async Task<bool> LoginAsync(string email, string password)
    {
        LoginRequestDto loginRequest = new(Email: email, Password: password);

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            // NOTE: Invalid credentials
            return false;
        }

        string token = await response.Content.ReadAsStringAsync();

        await authenticationStateProvider.MarkUserAsAuthenticatedAsync(email, token);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await authenticationStateProvider.MarkUserAsLoggedOutAsync();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        RegisterRequestDto registerRequest = new(Email: email, Password: password);
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/register", registerRequest);

        return response.IsSuccessStatusCode;
    }

}
