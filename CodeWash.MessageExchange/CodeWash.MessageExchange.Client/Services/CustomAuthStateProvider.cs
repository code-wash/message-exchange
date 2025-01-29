using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeWash.MessageExchange.Client.Services;

public class CustomAuthStateProvider(LocalStorage localStorage) : AuthenticationStateProvider
{
    private readonly LocalStorage localStorage = localStorage;
    private const string AuthTokenKey = "authToken";

    public async Task<string?> GetTokenAsync()
    {
        return await localStorage.GetItemAsync(AuthTokenKey);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(emailClaim))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, emailClaim),
            new(ClaimTypes.Email, emailClaim)
        ];

        ClaimsIdentity identity = new(claims, "jwt");
        ClaimsPrincipal user = new(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string email, string token)
    {
        await localStorage.SetItemAsync(AuthTokenKey, token);

        Claim[] claims =
        [
            new(ClaimTypes.Email, email)
        ];

        ClaimsIdentity identity = new(claims, "jwt");
        ClaimsPrincipal user = new(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await localStorage.RemoveItemAsync(AuthTokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    public async Task AddAuthorizationToken(HttpRequestHeaders requestHeaders)
    {
        string? token = await GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            requestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
