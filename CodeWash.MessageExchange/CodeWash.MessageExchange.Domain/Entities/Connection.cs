using CodeWash.MessageExchange.Domain.Contracts;

namespace CodeWash.MessageExchange.Domain.Entities;

public class Connection : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DisconnectedAt { get; set; }
}