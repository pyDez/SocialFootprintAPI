CREATE TABLE [dbo].[AccessTokenPair]
(
	[AccessTokenPairId] INT         IDENTITY (1, 1) NOT NULL,  
    [AccessToken] VARCHAR(4000) NOT NULL,
	[AccessTokenSecret] VARCHAR(4000) NOT NULL 
	CONSTRAINT [PK_AccessTokenPair] PRIMARY KEY CLUSTERED ([AccessTokenPairId] ASC)
)