
ALTER PROCEDURE sp_DisconnectUser
    @UserEmail NVARCHAR(256),
    @DisconnectedAt DATETIME
AS
BEGIN
    UPDATE Connections
    SET DisconnectedAt = @DisconnectedAt
    FROM Connections
    INNER JOIN Users ON Users.Id = Connections.UserId
    WHERE Connections.DisconnectedAt IS NULL 
    AND Users.Email = @UserEmail;
END;
