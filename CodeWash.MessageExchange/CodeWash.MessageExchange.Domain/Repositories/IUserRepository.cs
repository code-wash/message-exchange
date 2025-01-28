using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> AddAsync(User user, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllExceptAsync(Guid excludedUserId, CancellationToken cancellationToken);
}

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<bool> AddMessageAsync(Message message, CancellationToken cancellationToken);
}

public interface IConnectionRepository
{
    Task<bool> AddConnectionAsync(Connection connection, CancellationToken cancellationToken);
    Task<bool> UpdateConnectionAsync(Guid connectionId, DateTime disconnectedAt, CancellationToken cancellationToken);
}
