using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.NonQueryDtos;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;

public class DisconnectUserSP(DisconnectUserVM vm) : NonQuerySP
{
    public override string ProcedureName => "sp_DisconnectUser";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@UserEmail", vm.UserEmail},
        {"@DisconnectedAt", vm.DisconnectedAt},
    };
}
