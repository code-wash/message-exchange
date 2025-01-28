using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeWash.MessageExchange.DataAccess.Contracts;
public abstract class QuerySP<T> : BaseSP
{
    public abstract T ReadEntity(SqlDataReader reader);

    public async Task<List<T>> ExecuteAsync(string connectionString, CancellationToken cancellationToken)
    {
        using SqlConnection connection = new(connectionString);
        using SqlCommand command = new(ProcedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        SetParameters(command);

        await connection.OpenAsync(cancellationToken);
        using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        List<T> results = [];

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(ReadEntity(reader));
        }

        return results;
    }
}
