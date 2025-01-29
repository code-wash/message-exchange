using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Dtos.ApiDtos.AuthDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeWash.MessageExchange.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IDbConnector dbConnector, IConfiguration configuration) : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        GetUserByEmailVM? user = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(request.Email), cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        string token = GenerateJwtToken(user.Email, configuration);

        await dbConnector.ExecuteCommandAsync(new CreateConnectionSP(new Connection { UserId = user.Id }), cancellationToken);

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        // NOTE: Check if the user already exists
        List<GetUserByEmailVM> existingUsers = await dbConnector.ExecuteQueryAsync(new GetUserByEmailSP(request.Email), cancellationToken);
        if (existingUsers.Count != 0)
        {
            return Conflict("User with this email already exists.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User newUser = new()
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };

        await dbConnector.ExecuteCommandAsync(new CreateUserSP(newUser), cancellationToken);

        return Ok("User registered successfully.");
    }

    private static string GenerateJwtToken(string email, IConfiguration configuration)
    {
        byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing"));
        string? issuer = configuration["Jwt:Issuer"];
        string? audience = configuration["Jwt:Audience"];

        Claim[] claims =
        [
            new(ClaimTypes.Name, email)
        ];

        SymmetricSecurityKey securityKey = new(key);
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
