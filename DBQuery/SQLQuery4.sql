drop table RefreshTokens
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId INT NOT NULL, -- changed from UNIQUEIDENTIFIER
    TokenHash NVARCHAR(256) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    RevokedAt DATETIME NULL,
    ReplacedByTokenId UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
