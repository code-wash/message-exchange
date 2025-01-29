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
public class UserController(IDbConnector dbConnector) : ControllerBase
{
    [HttpGet("except-current")]
    public async Task<IActionResult> GetUsersExceptCurrent(CancellationToken cancellationToken)
    {
        string? currentUserEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(currentUserEmail))
        {
            return Unauthorized("Invalid or missing User Email claim.");
        }

        List<GetUsersExceptCurrentVM> users = await dbConnector.ExecuteQueryAsync(
            new GetUsersExceptCurrentSP(currentUserEmail), cancellationToken
        );

        return Ok(users);
    }
}
