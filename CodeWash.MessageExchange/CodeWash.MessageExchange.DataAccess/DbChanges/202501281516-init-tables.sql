CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL
);

CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    ReceiverId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsRead BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (SenderId) REFERENCES Users(Id),
    FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
);

CREATE TABLE Connections (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ConnectedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    DisconnectedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email
ON Users (Email);

-- Index for SenderId and ReceiverId together (composite index)
CREATE NONCLUSTERED INDEX IX_Messages_SenderReceiver
ON Messages (SenderId, ReceiverId);

-- Index for Timestamp (frequently used for sorting)
CREATE NONCLUSTERED INDEX IX_Messages_Timestamp
ON Messages (Timestamp);

-- Index for UserId to speed up queries for a specific user's connections
CREATE NONCLUSTERED INDEX IX_Connections_UserId
ON Connections (UserId);

-- Index for DisconnectedAt to optimize filtering for active connections
CREATE NONCLUSTERED INDEX IX_Connections_DisconnectedAt
ON Connections (DisconnectedAt);
