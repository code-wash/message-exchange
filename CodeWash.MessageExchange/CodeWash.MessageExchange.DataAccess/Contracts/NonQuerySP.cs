using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeWash.MessageExchange.DataAccess.Contracts;

public abstract class NonQuerySP : BaseSP
{
    public async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken)
    {
        using SqlConnection connection = new(connectionString);
        using SqlCommand command = new(ProcedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        SetParameters(command);

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
