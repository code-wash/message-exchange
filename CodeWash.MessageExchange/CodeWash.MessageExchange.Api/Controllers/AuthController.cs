using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;
using CodeWash.MessageExchange.Domain.Entities;
using CodeWash.MessageExchange.Dtos.ApiDtos.AuthDtos;
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

        // Fetch user from the database
        var user = await dbConnector.ExecuteQueryTop1Async(new GetUserByEmailSP(request.Email), cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        // Generate JWT Token
        var token = GenerateJwtToken(user.Email, configuration);

        // Register user connection
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
        var existingUsers = await dbConnector.ExecuteQueryAsync(new GetUserByEmailSP(request.Email), cancellationToken);
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
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing"));
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email)
        };

        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
