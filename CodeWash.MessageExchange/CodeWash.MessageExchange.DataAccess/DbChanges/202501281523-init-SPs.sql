CREATE PROCEDURE sp_CreateUser
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    INSERT INTO Users (Id, Email, PasswordHash)
    VALUES (@Id, @Email, @PasswordHash);
END;

GO

CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SELECT Id, Email, PasswordHash
    FROM Users
    WHERE Email = @Email;
END;

GO

CREATE PROCEDURE sp_CreateMessage
    @Id UNIQUEIDENTIFIER,
    @SenderId UNIQUEIDENTIFIER,
    @ReceiverId UNIQUEIDENTIFIER,
    @Content NVARCHAR(MAX),
    @Timestamp DATETIME
AS
BEGIN
    INSERT INTO Messages (Id, SenderId, ReceiverId, Content, Timestamp, IsRead)
    VALUES (@Id, @SenderId, @ReceiverId, @Content, @Timestamp, 0);
END;

GO

CREATE PROCEDURE sp_GetMessagesBetweenUsers
    @UserId1 UNIQUEIDENTIFIER,
    @UserId2 UNIQUEIDENTIFIER
AS
BEGIN
    SELECT Id, SenderId, ReceiverId, Content, Timestamp, IsRead
    FROM Messages
    WHERE (SenderId = @UserId1 AND ReceiverId = @UserId2)
       OR (SenderId = @UserId2 AND ReceiverId = @UserId1)
    ORDER BY Timestamp ASC;
END;

GO

CREATE PROCEDURE sp_MarkMessageAsRead
    @MessageId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE Messages
    SET IsRead = 1
    WHERE Id = @MessageId;
END;

GO

CREATE PROCEDURE sp_CreateConnection
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @ConnectedAt DATETIME
AS
BEGIN
    INSERT INTO Connections (Id, UserId, ConnectedAt)
    VALUES (@Id, @UserId, @ConnectedAt);
END;

GO

CREATE PROCEDURE sp_DisconnectUser
    @Id NVARCHAR(256),
    @DisconnectedAt DATETIME
AS
BEGIN
    UPDATE Connections
    SET DisconnectedAt = @DisconnectedAt
    WHERE Id = @Id AND DisconnectedAt IS NULL;
END;
