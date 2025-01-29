using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.DataAccess;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CodeWash.MessageExchange.DataAccess.Contracts;

namespace CodeWash.MessageExchange.Api.Controllers;

public abstract class BaseApiController(IDbConnector dbConnector) : ControllerBase
{
    protected readonly IDbConnector dbConnector = dbConnector;

    protected string? CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

    protected async Task<Guid> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken)
    {
        GetUserByEmailVM? user = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(email), cancellationToken);
        return user?.Id ?? throw new Exception("User not found.");
    }
}
