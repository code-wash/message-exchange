using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Dtos.ApiDtos.MessageDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeWash.MessageExchange.Api.Controllers;

[Route("api/messages")]
[ApiController]
[Authorize]
public class MessageController(IDbConnector dbConnector) : BaseApiController
{
    [HttpGet("between-users")]
    public async Task<IActionResult> GetMessagesBetweenUsersAsync([FromQuery] GetMessagesBetweenUsersRequestDto requestDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(requestDto?.UserEmail))
        {
            return BadRequest("Invalid User info to get messages.");
        }
        
        GetUserByEmailVM? user = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(requestDto.UserEmail), cancellationToken);

        GetUserByEmailVM? currentUser = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(CurrentUserEmail!), cancellationToken);

        if (user is null || currentUser is null)
        {
            return NotFound("User not found to get messages.");
        }

        List<GetMessagesBetweenUsersVM> messages = await dbConnector.ExecuteQueryAsync(
            new GetMessagesBetweenUsersSP(user.Id, currentUser.Id), cancellationToken
        );

        return Ok(messages);
    }
}
