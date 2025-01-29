using Microsoft.AspNetCore.Components.Authorization;
using CodeWash.MessageExchange.Client.Services;
using CodeWash.MessageExchange.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddMudServices();

builder.Services.AddScoped<CustomAuthorizationHandler>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7216/") });
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7216/");
}).AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LocalStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MessageService>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
