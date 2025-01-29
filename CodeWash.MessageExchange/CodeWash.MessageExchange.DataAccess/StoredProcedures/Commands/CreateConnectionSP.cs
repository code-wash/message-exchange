using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;

public class CreateConnectionSP(Connection connection) : NonQuerySP
{
    public override string ProcedureName => "sp_CreateConnection";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@Id", connection.Id},
        {"@UserId", connection.UserId},
        {"@ConnectedAt", connection.ConnectedAt},
    };
}