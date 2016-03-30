CREATE TABLE [dbo].[BadgeCollected]
(
	[BadgeCollectedId] INT         IDENTITY (1, 1) NOT NULL,  
    [AppUserId] INT NOT NULL, 
    [BadgeId] INT NOT NULL, 
	[PostId] INT NULL,
	[CollectDate] DATETIME2 NOT NULL,
	CONSTRAINT [FK_BadgeCollected_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [FK_BadgeCollected_Badge] FOREIGN KEY ([BadgeId]) REFERENCES [Badge]([BadgeId]),
	CONSTRAINT [FK_BadgeCollected_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
	CONSTRAINT [PK_BadgeCollected] PRIMARY KEY CLUSTERED ([BadgeCollectedId] ASC)
)