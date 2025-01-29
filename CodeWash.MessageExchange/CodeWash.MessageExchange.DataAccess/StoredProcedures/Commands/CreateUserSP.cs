using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
public class CreateUserSP(User user) : NonQuerySP
{
    public override string ProcedureName => "sp_CreateUser";

    public override Dictionary<string, object> Parameters => new()
    {
        {"@Id", user.Id},
        {"@Email", user.Email},
        {"@PasswordHash", user.PasswordHash},
    };
}
