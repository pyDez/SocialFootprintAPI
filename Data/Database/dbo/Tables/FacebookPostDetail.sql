CREATE TABLE [dbo].[FacebookPostDetail]
(
	[FacebookPostDetailId] INT IDENTITY (1, 1) NOT NULL , 
    [PostId] INT NOT NULL,
	[FacebookPostId] VARCHAR(100) NULL, 
    [Link] VARCHAR(5000) NULL,
	[Caption] VARCHAR(500) NULL, 
    [Message] VARCHAR(MAX) NULL, 
    [LinkName] VARCHAR(1000) NULL, 
    [AttachedObjectId] VARCHAR(50) NULL, 
    [Picture] VARCHAR(5000) NULL, 
    [Privacy] VARCHAR(500) NULL, 
    [VideoSource] VARCHAR(5000) NULL, 
    [StatusType] VARCHAR(50) NULL, 
    [Story] VARCHAR(4000) NULL, 
    [GeneralStatusType] VARCHAR(50) NULL, 
    [UpdateTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
	[ChildPostId] INT NULL,
    CONSTRAINT [FK_FacebookPostDetail_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [FK_FacebookPostDetail_Post_Inheritance] FOREIGN KEY ([ChildPostId]) REFERENCES [Post]([PostId]),
    CONSTRAINT [PK_FacebookPostDetail] PRIMARY KEY CLUSTERED ([FacebookPostDetailId] ASC) 
)
