using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Dtos.ApiDtos.MessageDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpPost("send")]
    public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageDto request, CancellationToken cancellationToken)
    {
        if (CurrentUserEmail is null)
        {
            return Unauthorized("Invalid token.");
        }

        if (CurrentUserEmail.Equals(request.ReceiverEmail, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("You cannot send a message to yourself.");
        }

        Message message = new()
        {
            SenderId = await GetUserIdByEmailAsync(CurrentUserEmail, cancellationToken),
            ReceiverId = await GetUserIdByEmailAsync(request.ReceiverEmail, cancellationToken),
            Content = request.Content,
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };

        CreateMessageSP createMessageSP = new(message);
        int rowsAffected = await dbConnector.ExecuteCommandAsync(createMessageSP, cancellationToken);

        return rowsAffected > 0 
            ? Ok("Message sent successfully.") 
            : StatusCode(500, "Failed to send message.");
    }

    private async Task<Guid> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken)
    {
        GetUserByEmailVM? user = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(email), cancellationToken);
        return user?.Id ?? throw new Exception("User not found.");
    }
}
