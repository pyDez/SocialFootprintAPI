CREATE TABLE [dbo].[Friendship]
(
	[FriendshipId]  INT IDENTITY (1, 1) NOT NULL, 
    [AppUserAId] INT NOT NULL, 
    [AppUserBId] INT NOT NULL, 
     CONSTRAINT [FK_Friendship_AppUserA] FOREIGN KEY ([AppUserAId]) REFERENCES [AppUser]([AppUserId]),
	 CONSTRAINT [FK_Friendship_AppUserB] FOREIGN KEY ([AppUserBId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_Friendship] PRIMARY KEY CLUSTERED ([FriendshipId] ASC) 
)
