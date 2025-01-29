using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Domain.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeWash.MessageExchange.DataAccess.Repositories;

[Obsolete]
public class UserRepository(string connectionString) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using SqlConnection connection = new(connectionString);
        using SqlCommand command = new("sp_GetUserByEmail", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync(cancellationToken);
        using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new User
            {
                Id = reader.GetGuid("Id"),
                Email = reader.GetString("Email"),
                PasswordHash = reader.GetString("PasswordHash")
            };
        }

        return null;
    }

    public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        using SqlConnection connection = new(connectionString);
        using SqlCommand command = new("sp_CreateUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        await connection.OpenAsync(cancellationToken);
        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<User>> GetAllExceptAsync(Guid excludedUserId, CancellationToken cancellationToken)
    {
        List<User> users = [];

        using SqlConnection connection = new(connectionString);
        using SqlCommand command = new("sp_GetAllUsersExcept", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ExcludedUserId", excludedUserId);

        await connection.OpenAsync(cancellationToken);
        using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            users.Add(new User
            {
                Id = reader.GetGuid("Id"),
                Email = reader.GetString("Email")
            });
        }

        return users;
    }
}
