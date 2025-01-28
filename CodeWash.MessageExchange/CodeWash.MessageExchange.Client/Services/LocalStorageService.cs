﻿using Microsoft.JSInterop;

namespace CodeWash.MessageExchange.Client.Services;

public class LocalStorageService(IJSRuntime jsRuntime)
{
    public async Task SetItemAsync(string key, string value) =>
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

    public async Task<string?> GetItemAsync(string key) =>
        await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

    public async Task RemoveItemAsync(string key) =>
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
}
