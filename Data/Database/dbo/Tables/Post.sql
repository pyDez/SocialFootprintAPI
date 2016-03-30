CREATE TABLE [dbo].[Post]
(
	[PostId] INT         IDENTITY (1, 1) NOT NULL,  
    [AppUserId] INT NOT NULL, 
    [CategoryId] INT NULL, 
    [Activated] BIT NOT NULL DEFAULT 1, 
    [CreationDate] DATETIME2 NOT NULL DEFAULT GETDATE(),  
    CONSTRAINT [FK_Post_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
	CONSTRAINT [FK_Post_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([CategoryId]),
	CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED ([PostId] ASC)
)

/*

status_type
enum{mobile_status_update, created_note, added_photos, added_video, shared_story, created_group, created_event, wall_post, app_created_story, published_story, tagged_in_photo, approved_friend}
*/