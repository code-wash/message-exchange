using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

namespace CodeWash.MessageExchange.Client.Services;

public class CustomAuthorizationHandler(AuthenticationStateProvider authenticationStateProvider, NavigationManager navigation) : DelegatingHandler
{
    private readonly CustomAuthStateProvider authenticationStateProvider = (CustomAuthStateProvider)authenticationStateProvider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await authenticationStateProvider.AddAuthorizationToken(request.Headers);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            await authenticationStateProvider.MarkUserAsLoggedOutAsync();
            navigation.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}
