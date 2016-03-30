CREATE TABLE [dbo].[CategoryTranslation]
(
	[CategoryTranslationId] INT IDENTITY (1, 1) NOT NULL , 
    [CategoryId] INT NULL, 
    [Label] VARCHAR(4000) NULL, 
    [Locale] VARCHAR(5) NULL,
	CONSTRAINT [FK_CategoryTranslation_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([CategoryId]),
    CONSTRAINT [PK_CategoryTranslation] PRIMARY KEY CLUSTERED ([CategoryTranslationId] ASC) 
)
