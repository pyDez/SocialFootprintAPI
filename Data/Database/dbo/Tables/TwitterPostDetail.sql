CREATE TABLE [dbo].[TwitterPostDetail]
(
	[TwitterPostDetailId] INT IDENTITY (1, 1) NOT NULL , 
    [PostId] INT NOT NULL,
	[TwitterPostId] VARCHAR(100) NULL, 
    [Text] VARCHAR(5000) NULL,
    [CreationTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
	[RetweetedPostId] INT NULL,
    CONSTRAINT [FK_TwitterPostDetail_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [FK_TwitterPostDetail_Post_Inheritance] FOREIGN KEY ([RetweetedPostId]) REFERENCES [Post]([PostId]),
    CONSTRAINT [PK_TwitterPostDetail] PRIMARY KEY CLUSTERED ([TwitterPostDetailId] ASC) 
)

      