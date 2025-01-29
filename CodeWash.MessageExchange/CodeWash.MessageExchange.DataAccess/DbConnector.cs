using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.Extensions.Configuration;

namespace CodeWash.MessageExchange.DataAccess;

public class DbConnector(IConfiguration configuration) : IDbConnector
{
    private readonly string connectionString = configuration["ConnectionStrings:MessageExchangeDB"]!;

    public async Task<int> ExecuteCommandAsync(NonQuerySP commandSP, CancellationToken cancellationToken)
    {
        return await commandSP.ExecuteAsync(connectionString, cancellationToken);
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(QuerySP<T> querySP, CancellationToken cancellationToken)
        where T : IQueryVM
    {
        return await querySP.ExecuteAsync(connectionString, cancellationToken);
    }

    public async Task<T?> ExecuteQueryTop1Async<T>(QuerySP<T> querySP, CancellationToken cancellationToken)
        where T : IQueryVM
    {
        var results = await ExecuteQueryAsync(querySP, cancellationToken); 
        return results.FirstOrDefault();
    }
}
