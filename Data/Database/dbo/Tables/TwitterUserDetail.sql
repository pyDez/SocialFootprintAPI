CREATE TABLE [dbo].[TwitterUserDetail]
(
	[TwitterUserDetailId] INT IDENTITY (1, 1) NOT NULL , 
	[AppUserId] INT NOT NULL, 
    [TwitterAccessToken] VARCHAR(4000) NULL, 
    [TwitterAccessTokenSecret] VARCHAR(4000) NULL, 
	[ScreenName] VARCHAR(4000) NULL, 
    [TwitterUserId] VARCHAR(4000) NOT NULL,
	[Description] VARCHAR(MAX) NULL,
	[Url] VARCHAR(4000) NULL,
    CONSTRAINT [FK_TwitterUserDetail_User] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_TwitterUserDetail] PRIMARY KEY CLUSTERED ([TwitterUserDetailId] ASC) 
)