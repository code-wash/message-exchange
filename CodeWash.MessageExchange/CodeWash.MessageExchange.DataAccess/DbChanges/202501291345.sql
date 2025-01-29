
ALTER PROCEDURE sp_GetMessagesBetweenUsers
    @UserId1 UNIQUEIDENTIFIER,
    @UserId2 UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        m.Content, 
        m.Timestamp, 
        u.Email AS SenderEmail
    FROM Messages m
    INNER JOIN Users u ON m.SenderId = u.Id
    WHERE 
        (m.SenderId = @UserId1 AND m.ReceiverId = @UserId2)
        OR 
        (m.SenderId = @UserId2 AND m.ReceiverId = @UserId1)
    ORDER BY m.Timestamp;
END;
