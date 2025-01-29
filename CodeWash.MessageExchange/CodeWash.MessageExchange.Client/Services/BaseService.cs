namespace CodeWash.MessageExchange.Client.Services;

public abstract class BaseService(IHttpClientFactory httpClientFactory)
{
    protected readonly HttpClient httpClient = httpClientFactory.CreateClient("ApiClient");
}
