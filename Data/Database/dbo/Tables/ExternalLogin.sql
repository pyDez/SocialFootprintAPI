CREATE TABLE [dbo].[ExternalLogin]
(
	[ExternalLoginId] INT IDENTITY (1, 1) NOT NULL, 
    [AppUserId] INT NOT NULL, 
    [LoginProvider] VARCHAR(MAX) NOT NULL, 
    [ProviderKey] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_ExternalLogins_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_ExternalLogin] PRIMARY KEY CLUSTERED ([ExternalLoginId] ASC) 
)
