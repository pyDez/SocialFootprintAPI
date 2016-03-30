CREATE TABLE [dbo].[Log]
(
	[LogId] INT         IDENTITY (1, 1) NOT NULL,  
    [UserName] VARCHAR(512) NULL, 
	[Date] DATETIME2 NOT NULL, 
	[Level] VARCHAR(10) NULL, 
    [Logger] VARCHAR(512) NULL, 
    [Method] VARCHAR(200) NULL, 
    [Parameters] VARCHAR(8000) NULL, 
    [Message] VARCHAR(1000) NULL, 
    [Exception] VARCHAR(4000) NULL,
	[Thread] VARCHAR(32) NOT NULL, 
    [Context] VARCHAR(10) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([LogId] ASC)
)





