﻿namespace CodeWash.MessageExchange.Dtos.QueryDtos;

public record GetUsersExceptCurrentVM(Guid Id, string Email, int UnreadMessagesCount)
    : IQueryVM;
