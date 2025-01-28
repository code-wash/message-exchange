using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

public class GetUserByEmailSP(string email) : QuerySP<User>
{
    public override string ProcedureName => "sp_GetUserByEmail";

    public override Dictionary<string, object> Parameters => new()
    {
        { "@Email", email },
    };

    public override User ReadEntity(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
        };
    }
}
