using CodeWash.MessageExchange.Domain.Contracts;

namespace CodeWash.MessageExchange.Domain.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; } // FK: Sender (User ID)
    public Guid ReceiverId { get; set; } // FK: Receiver (User ID)
    public string Content { get; set; } = string.Empty; // Message content
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Message creation timestamp
    public bool IsRead { get; set; } = false; // Read status
}
