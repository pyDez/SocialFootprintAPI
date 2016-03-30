CREATE TABLE [dbo].[WordProbability]
(
	[WordProbabilityId] INT IDENTITY (1, 1) NOT NULL, 
    [Word] VARCHAR(255) NULL, 
    [CategoryId] INT NOT NULL, 
    [Matches] INT NOT NULL DEFAULT 0, 
    [NonMatches] INT NOT NULL DEFAULT 0,
	CONSTRAINT [FK_WordProbability_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([CategoryId]), 
	CONSTRAINT [PK_WordProbability] PRIMARY KEY CLUSTERED ([WordProbabilityId] ASC)
)
