using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeWash.MessageExchange.Client.Services;

public class CustomAuthStateProvider(LocalStorage localStorage) : AuthenticationStateProvider
{
    private readonly LocalStorage localStorage = localStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = await localStorage.GetItemAsync("authToken");

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        Claim[] claims =
        [
            new(ClaimTypes.Name, "User")
        ];

        ClaimsIdentity identity = new(claims, "jwt");
        ClaimsPrincipal user = new(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string email, string token)
    {
        await localStorage.SetItemAsync("authToken", token);

        Claim[] claims =
        [
            new(ClaimTypes.Name, email)
        ];

        ClaimsIdentity identity = new(claims, "jwt");
        ClaimsPrincipal user = new(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    public async Task AddAuthorizationToken(HttpRequestHeaders requestHeaders)
    {
        string? token = await localStorage.GetItemAsync("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            requestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
