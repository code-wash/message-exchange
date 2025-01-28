using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

public class GetMessagesBetweenUsersSP(Guid userId1, Guid userId2) : QuerySP<Message>
{
    public override string ProcedureName => "sp_GetMessagesBetweenUsers";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@UserId1", userId1},
        {"@UserId2", userId2},
    };

    public override Message ReadEntity(SqlDataReader reader)
    {
        return new Message
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            SenderId = reader.GetGuid(reader.GetOrdinal("SenderId")),
            ReceiverId = reader.GetGuid(reader.GetOrdinal("ReceiverId")),
            Content = reader.GetString(reader.GetOrdinal("Content")),
            Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
            IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead"))
        };
    }
}
