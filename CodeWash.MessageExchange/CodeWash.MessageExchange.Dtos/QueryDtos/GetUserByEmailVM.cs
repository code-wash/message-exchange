namespace CodeWash.MessageExchange.Dtos.QueryDtos;

public record GetUserByEmailVM(Guid Id, string Email, string PasswordHash)
    : IQueryVM;

