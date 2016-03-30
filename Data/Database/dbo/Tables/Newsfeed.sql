CREATE TABLE [dbo].[Newsfeed]
(
	[NewsfeedId] INT IDENTITY (1, 1) NOT NULL,
    [AppUserId] INT NULL, 
    [PostId] INT NULL
    CONSTRAINT [FK_Newsfeed_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [FK_Newsfeed_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_Newsfeed] PRIMARY KEY CLUSTERED ([NewsfeedId] ASC) 
)
