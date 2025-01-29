using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeWash.MessageExchange.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected string? CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;
}
