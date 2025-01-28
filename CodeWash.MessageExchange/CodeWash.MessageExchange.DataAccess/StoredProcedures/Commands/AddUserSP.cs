using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;

public class AddUserSP(User user) : NonQuerySP
{
    public override string ProcedureName => "sp_AddUser";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@Id", user.Id},
        {"@Email", user.Email},
        {"@PasswordHash", user.PasswordHash},
    };
}
