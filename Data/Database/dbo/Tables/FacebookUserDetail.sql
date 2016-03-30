CREATE TABLE [dbo].[FacebookUserDetail]
(
	[FacebookUserDetailId] INT IDENTITY (1, 1) NOT NULL , 
	[AppUserId] INT NOT NULL, 
    [FacebookAccessToken] VARCHAR(MAX) NULL, 
    [FacebookUserId] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_FacebookUserDetail_User] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
    CONSTRAINT [PK_FacebookUserDetail] PRIMARY KEY CLUSTERED ([FacebookUserDetailId] ASC) 
)
