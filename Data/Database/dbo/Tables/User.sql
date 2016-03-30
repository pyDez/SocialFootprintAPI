CREATE TABLE [dbo].[AppUser]
(
	[AppUserId]  INT IDENTITY (1, 1) NOT NULL, 
	[UserName] VARCHAR(MAX) NULL, 
	[PasswordHash] VARCHAR(MAX) NULL, 
    [SecurityStamp] VARCHAR(MAX) NULL,
    [FirstName] VARCHAR(500) NULL, 
    [LastName] VARCHAR(500) NULL, 
    [EmailAddress] VARCHAR(100) NULL, 
	[Gender] BIT NOT NULL DEFAULT 1, 
    [Birthday] DATETIME2 NULL DEFAULT GETDATE(), 
    [Activated] BIT NOT NULL DEFAULT 1, 
	[Locale] VARCHAR(5) NULL,
    [SigningUpDate] DATETIME2 NULL DEFAULT GETDATE(), 
    [LastLogInDate] DATETIME2 NULL DEFAULT GETDATE(), 
    
    CONSTRAINT [PK_AppUser] PRIMARY KEY CLUSTERED ([AppUserId] ASC)
)
