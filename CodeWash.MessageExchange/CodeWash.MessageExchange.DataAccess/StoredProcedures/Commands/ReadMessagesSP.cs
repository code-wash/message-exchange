using CodeWash.MessageExchange.DataAccess.Contracts;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;

public class ReadMessagesSP(Guid ReaderUserId, Guid SenderUserId) : NonQuerySP
{
    public override string ProcedureName => "sp_ReadMessages";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@ReaderUserId", ReaderUserId},
        {"@SenderUserId", SenderUserId},
    };
}
