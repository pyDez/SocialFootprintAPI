CREATE TABLE [dbo].[AuthToken]
(
	[AuthTokenId]  INT         IDENTITY (1, 1) NOT NULL, 
    [Token] VARCHAR(50) NULL, 
    [Expiration] DATETIME2 NULL, 
    [AppUserId] INT NOT NULL, 
	CONSTRAINT [PK_AuthToken] PRIMARY KEY CLUSTERED ([AuthTokenId] ASC),
    CONSTRAINT [FK_AuthToken_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId])
)
