using CodeWash.MessageExchange.Domain.Entities;

namespace CodeWash.MessageExchange.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> AddAsync(User user, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllExceptAsync(Guid excludedUserId, CancellationToken cancellationToken);
}
