using System.Data;
using CodeWash.MessageExchange.DataAccess.Contracts;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess;

public class DbConnector(string connectionString) : IDbConnector
{
    public async Task<int> ExecuteCommandAsync(NonQuerySP commandSP, CancellationToken cancellationToken)
    {
        return await commandSP.ExecuteAsync(connectionString, cancellationToken);
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(QuerySP<T> querySP, CancellationToken cancellationToken)
    {
        return await querySP.ExecuteAsync(connectionString, cancellationToken);
    }
}
