CREATE TABLE [dbo].[Hashtag]
(
	[HashtagId] INT         IDENTITY (1, 1) NOT NULL,  
    [Text] VARCHAR(4000) NOT NULL,
	[PostId] INT NOT NULL,
	[Provider] VARCHAR(4000) NOT NULL 
	CONSTRAINT [FK_Hashtag_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [PK_Hashtag] PRIMARY KEY CLUSTERED ([HashtagId] ASC)
)