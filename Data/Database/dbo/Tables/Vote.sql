CREATE TABLE [dbo].[Vote]
(
	[VoteId] INT IDENTITY (1, 1) NOT NULL, 
    [AppUserId] INT NOT NULL, 
    [UpPostId] INT NOT NULL, 
    [DownPostId] INT NOT NULL, 
    CONSTRAINT [FK_Vote_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]), 
    CONSTRAINT [FK_Vote_Post_Up] FOREIGN KEY ([UpPostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [FK_Vote_Post_Down] FOREIGN KEY ([DownPostId]) REFERENCES [Post]([PostId]), 
    CONSTRAINT [PK_Vote] PRIMARY KEY CLUSTERED ([VoteId] ASC) 
)
