﻿using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeWash.MessageExchange.DataAccess.Repositories;

public class MessageRepository(string connectionString) : IMessageRepository
{
    public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        var messages = new List<Message>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("sp_GetMessagesBetweenUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@UserId1", userId1);
        command.Parameters.AddWithValue("@UserId2", userId2);

        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            messages.Add(new Message
            {
                Id = reader.GetGuid("Id"),
                SenderId = reader.GetGuid("SenderId"),
                ReceiverId = reader.GetGuid("ReceiverId"),
                Content = reader.GetString("Content"),
                Timestamp = reader.GetDateTime("Timestamp"),
                IsRead = reader.GetBoolean("IsRead")
            });
        }

        return messages;
    }

    public async Task<bool> AddMessageAsync(Message message, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("sp_AddMessage", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", message.Id);
        command.Parameters.AddWithValue("@SenderId", message.SenderId);
        command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
        command.Parameters.AddWithValue("@Content", message.Content);
        command.Parameters.AddWithValue("@Timestamp", message.Timestamp);

        await connection.OpenAsync(cancellationToken);
        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }
}
