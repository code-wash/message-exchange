using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeWash.MessageExchange.Api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController(IDbConnector dbConnector) : BaseApiController
{
    [HttpGet("except-current")]
    public async Task<IActionResult> GetUsersExceptCurrentAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(CurrentUserEmail))
        {
            return Unauthorized("Invalid or missing User Email claim.");
        }

        List<GetUsersExceptCurrentVM> users = await dbConnector.ExecuteQueryAsync(
            new GetUsersExceptCurrentSP(CurrentUserEmail), cancellationToken
        );

        return Ok(users);
    }
}
