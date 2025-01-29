using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeWash.MessageExchange.DataAccess.Repositories;

[Obsolete]
public class ConnectionRepository(string connectionString) : IConnectionRepository
{
    public async Task<bool> CreateConnectionAsync(Connection connection, CancellationToken cancellationToken)
    {
        using SqlConnection sqlConnection = new(connectionString);
        using SqlCommand command = new("sp_CreateConnection", sqlConnection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", connection.Id);
        command.Parameters.AddWithValue("@UserId", connection.UserId);
        command.Parameters.AddWithValue("@ConnectedAt", connection.ConnectedAt);

        await sqlConnection.OpenAsync(cancellationToken);
        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateConnectionAsync(Guid connectionId, DateTime disconnectedAt, CancellationToken cancellationToken)
    {
        using SqlConnection sqlConnection = new(connectionString);
        using SqlCommand command = new("sp_DisconnectUser", sqlConnection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ConnectionId", connectionId);
        command.Parameters.AddWithValue("@DisconnectedAt", disconnectedAt);

        await sqlConnection.OpenAsync(cancellationToken);
        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }
}
