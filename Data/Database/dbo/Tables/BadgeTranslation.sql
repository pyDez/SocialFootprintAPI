CREATE TABLE [dbo].[BadgeTranslation]
(
	[BadgeTranslationId] INT IDENTITY (1, 1) NOT NULL , 
    [BadgeId] INT NULL, 
    [Label] VARCHAR(4000) NULL, 
    [Locale] VARCHAR(5) NULL,
	[Explanation] VARCHAR(8000) NULL, 
    CONSTRAINT [FK_BadgeTranslation_Badge] FOREIGN KEY ([BadgeId]) REFERENCES [Badge]([BadgeId]),
    CONSTRAINT [PK_BadgeTranslation] PRIMARY KEY CLUSTERED ([BadgeTranslationId] ASC) 
)
