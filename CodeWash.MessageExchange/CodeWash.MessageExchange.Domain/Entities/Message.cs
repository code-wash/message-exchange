using CodeWash.MessageExchange.Domain.Contracts;

namespace CodeWash.MessageExchange.Domain.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // TODO: IsRead property is created but the logic is not implemented
    //       on Client, Api, and even at the SP level (e.g. CreateMessageSP)
    public bool IsRead { get; set; } = false;
}
