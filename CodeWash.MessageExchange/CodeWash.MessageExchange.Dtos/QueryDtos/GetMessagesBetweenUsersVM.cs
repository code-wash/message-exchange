namespace CodeWash.MessageExchange.Dtos.QueryDtos;

public record GetMessagesBetweenUsersVM(Guid Id, Guid SenderId, Guid ReceiverId, string Content, DateTime Timestamp, bool IsRead);
