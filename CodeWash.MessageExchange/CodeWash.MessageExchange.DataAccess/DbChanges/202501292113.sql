
CREATE PROCEDURE sp_ReadMessages
    @ReaderUserId UNIQUEIDENTIFIER,
    @SenderUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Messages
    SET IsRead = 1
    WHERE ReceiverId = @ReaderUserId
      AND SenderId = @SenderUserId
      AND IsRead = 0;
END;

GO

ALTER PROCEDURE sp_GetUsersExceptCurrent
    @CurrentUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        U.Id, 
        U.Email,
        COUNT(CASE WHEN M.IsRead = 0 And M.ReceiverId = @CurrentUserId THEN 1 ELSE NULL END) AS UnreadMessagesCount,
        MAX(M.Timestamp) AS LastMessageDate
    FROM Users U
    LEFT JOIN Messages M 
        ON (M.SenderId = U.Id AND M.ReceiverId = @CurrentUserId)
        OR (M.ReceiverId = U.Id AND M.SenderId = @CurrentUserId)
    WHERE U.Id <> @CurrentUserId
    GROUP BY U.Id, U.Email
    ORDER BY COALESCE(MAX(M.Timestamp), '1900-01-01') DESC;
END;
