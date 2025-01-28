using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;

public class AddMessageSP(Message message) : NonQuerySP
{
    public override string ProcedureName => "sp_AddMessage";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@Id", message.Id},
        {"@SenderId", message.SenderId},
        {"@ReceiverId", message.ReceiverId},
        {"@Content", message.Content},
        {"@Timestamp", message.Timestamp},
    };
}
