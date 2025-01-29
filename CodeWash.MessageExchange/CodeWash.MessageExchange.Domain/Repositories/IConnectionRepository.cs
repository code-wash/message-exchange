using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.Domain.Repositories;

public interface IConnectionRepository
{
    Task<bool> CreateConnectionAsync(Connection connection, CancellationToken cancellationToken);
    Task<bool> UpdateConnectionAsync(Guid connectionId, DateTime disconnectedAt, CancellationToken cancellationToken);
}
