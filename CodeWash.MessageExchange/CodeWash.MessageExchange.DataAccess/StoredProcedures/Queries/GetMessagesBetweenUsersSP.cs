using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

public class GetMessagesBetweenUsersSP(Guid userId1, Guid userId2) : QuerySP<GetMessagesBetweenUsersVM>
{
    public override string ProcedureName => "sp_GetMessagesBetweenUsers";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@UserId1", userId1},
        {"@UserId2", userId2},
    };

    public override GetMessagesBetweenUsersVM ReadEntity(SqlDataReader reader)
    {
        return new GetMessagesBetweenUsersVM
        (
            Content: reader.GetString(reader.GetOrdinal("Content")),
            Timestamp: reader.GetDateTime(reader.GetOrdinal("Timestamp")),
            SenderEmail: reader.GetString(reader.GetOrdinal("SenderEmail"))
        );
    }
}
