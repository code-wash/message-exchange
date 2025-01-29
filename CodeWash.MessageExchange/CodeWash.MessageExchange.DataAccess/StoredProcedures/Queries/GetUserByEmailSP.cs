using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

public class GetUserByEmailSP(string email) : QuerySP<GetUserByEmailVM>
{
    public override string ProcedureName => "sp_GetUserByEmail";

    public override Dictionary<string, object> Parameters => new()
    {
        { "@Email", email },
    };

    public override GetUserByEmailVM ReadEntity(SqlDataReader reader)
    {
        return new GetUserByEmailVM
        (
            Id: reader.GetGuid(reader.GetOrdinal("Id")),
            Email: reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash: reader.GetString(reader.GetOrdinal("PasswordHash"))
        );
    }
}
