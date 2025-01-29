using CodeWash.MessageExchange.Dtos.QueryDtos;

namespace CodeWash.MessageExchange.DataAccess.Contracts;

public interface IDbConnector
{
    Task<int> ExecuteCommandAsync(NonQuerySP commandSP, CancellationToken cancellationToken);
    Task<List<T>> ExecuteQueryAsync<T>(QuerySP<T> querySP, CancellationToken cancellationToken) where T : IQueryVM;
    Task<T?> ExecuteQueryTop1Async<T>(QuerySP<T> querySP, CancellationToken cancellationToken) where T : IQueryVM;
}
