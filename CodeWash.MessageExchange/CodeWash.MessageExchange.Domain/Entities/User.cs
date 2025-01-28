using CodeWash.MessageExchange.Domain.Contracts;

namespace CodeWash.MessageExchange.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty; // User's email (must be unique)
    public string PasswordHash { get; set; } = string.Empty; // Hashed password for security
}
