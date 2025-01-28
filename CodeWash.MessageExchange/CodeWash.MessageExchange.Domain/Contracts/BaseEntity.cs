namespace CodeWash.MessageExchange.Domain.Contracts;

public interface IBaseEntity
{
    Guid Id { get; set; }
}

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the user
}