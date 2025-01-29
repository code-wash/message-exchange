namespace CodeWash.MessageExchange.Dtos.QueryDtos;
public record GetMessagesBetweenUsersVM(string Content, DateTime Timestamp, string SenderEmail)
    : IQueryVM;
