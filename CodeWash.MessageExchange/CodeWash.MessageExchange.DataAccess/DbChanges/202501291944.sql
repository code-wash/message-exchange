
ALTER PROCEDURE sp_GetUsersExceptCurrent
    @CurrentUserEmail NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        U.Id, 
        U.Email,
        COUNT(CASE WHEN M.IsRead = 0 THEN 1 ELSE NULL END) AS UnreadMessagesCount
    FROM Users U
    LEFT JOIN Messages M ON U.Id = M.SenderId 
                         AND M.ReceiverId = (SELECT Id FROM Users WHERE Email = @CurrentUserEmail)
                         AND M.IsRead = 0
    WHERE U.Email <> @CurrentUserEmail
    GROUP BY U.Id, U.Email
    ORDER BY U.Email;
END;
