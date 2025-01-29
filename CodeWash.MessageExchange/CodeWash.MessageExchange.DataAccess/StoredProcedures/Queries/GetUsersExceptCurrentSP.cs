using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

public class GetUsersExceptCurrentSP(Guid CurrentUserId) : QuerySP<GetUsersExceptCurrentVM>
{
    public override string ProcedureName => "sp_GetUsersExceptCurrent";

    public override Dictionary<string, object> Parameters => new()
    {
        { "@CurrentUserId", CurrentUserId },
    };

    public override GetUsersExceptCurrentVM ReadEntity(SqlDataReader reader)
    {
        return new GetUsersExceptCurrentVM
        (
            Id: reader.GetGuid(reader.GetOrdinal("Id")),
            Email: reader.GetString(reader.GetOrdinal("Email")),
            UnreadMessagesCount: reader.GetInt32(reader.GetOrdinal("UnreadMessagesCount"))
        );
    }
}
