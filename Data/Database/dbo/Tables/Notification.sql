CREATE TABLE [dbo].[Notification]
(
	[NotificationId] INT IDENTITY (1, 1) NOT NULL, 
	[AppUserId] int NOT NULL,
    [Information] VARCHAR(500) NULL,
	[IsRed] BIT NOT NULL DEFAULT 0,
	[ObjectType] VARCHAR(50) NULL, 
    [ObjectId] INT NULL, 
    [NotificationDate] DATETIME2 NULL, 
    CONSTRAINT [FK_Notification_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([NotificationId] ASC) 
)
