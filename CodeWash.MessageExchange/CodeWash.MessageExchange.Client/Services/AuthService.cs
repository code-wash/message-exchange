﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeWash.MessageExchange.Client.Services;

public class AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, LocalStorageService localStorage)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly AuthenticationStateProvider authStateProvider = authStateProvider;
    private readonly LocalStorageService localStorage = localStorage;

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new
        {
            Email = email,
            Password = password,
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            return false; // Invalid credentials
        }

        var token = await response.Content.ReadAsStringAsync();
        await localStorage.SetItemAsync("authToken", token);

        ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(email);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");
        await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOutAsync();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
