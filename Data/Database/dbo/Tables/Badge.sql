CREATE TABLE [dbo].[Badge]
(
	[BadgeId] INT         IDENTITY (1, 1) NOT NULL,  
    [Label] VARCHAR(4000) NOT NULL,
	[Level] VARCHAR(500) NOT NULL, 
    [CategoryId] INT NULL, 
	[Explanation] VARCHAR(8000) NULL, 
    CONSTRAINT [FK_Badge_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([CategoryId]),
	CONSTRAINT [PK_Badge] PRIMARY KEY CLUSTERED ([BadgeId] ASC)
)