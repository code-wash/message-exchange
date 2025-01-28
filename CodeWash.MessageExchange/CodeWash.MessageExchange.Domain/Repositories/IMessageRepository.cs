using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.Domain.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<bool> AddMessageAsync(Message message, CancellationToken cancellationToken);
}
