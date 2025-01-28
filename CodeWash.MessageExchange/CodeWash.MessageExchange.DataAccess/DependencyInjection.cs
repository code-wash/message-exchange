using CodeWash.MessageExchange.DataAccess.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CodeWash.MessageExchange.DataAccess;

public static class DependencyInjection
{
    public static void AddDataAccessServices(this IServiceCollection services)
    {
        services.AddScoped<IDbConnector, DbConnector>();
    }
}
