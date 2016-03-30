CREATE TABLE [dbo].[PostMentionedUser]
(
	[PostMentionedUserId] INT         IDENTITY (1, 1) NOT NULL,  
    [AppUserId] int NOT NULL,
	[PostId] INT NOT NULL,
	[Provider] VARCHAR(4000) NOT NULL 
	CONSTRAINT [FK_PostMentionedUser_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
	CONSTRAINT [FK_PostMentionedUser_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [PK_PostMentionedUser] PRIMARY KEY CLUSTERED ([PostMentionedUserId] ASC)
)