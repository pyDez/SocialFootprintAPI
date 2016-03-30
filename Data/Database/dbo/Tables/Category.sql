CREATE TABLE [dbo].[Category]
(
	[CategoryId] INT IDENTITY (1, 1) NOT NULL, 
    [Label] VARCHAR(50) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC) 
)
