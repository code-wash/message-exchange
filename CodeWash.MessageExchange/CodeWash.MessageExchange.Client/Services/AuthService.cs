using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeWash.MessageExchange.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly LocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, LocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            return false; // Invalid credentials
        }

        var token = await response.Content.ReadAsStringAsync();
        await _localStorage.SetItemAsync("authToken", token);

        ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(email);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOutAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
