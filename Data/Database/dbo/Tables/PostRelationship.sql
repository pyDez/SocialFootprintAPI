CREATE TABLE [dbo].[PostRelationship]
(
	[PostRelationshipId] INT IDENTITY (1, 1) NOT NULL,
    [ParentPostId] INT NULL, 
    [TargetedUserId] INT NULL, 
	[TargetedUserName] VARCHAR(200) NULL, 
    CONSTRAINT [FK_PostRelationship_Post] FOREIGN KEY ([ParentPostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [FK_PostRelationship_AppUser] FOREIGN KEY ([TargetedUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_PostRelationship] PRIMARY KEY CLUSTERED ([PostRelationshipId] ASC) 
)
