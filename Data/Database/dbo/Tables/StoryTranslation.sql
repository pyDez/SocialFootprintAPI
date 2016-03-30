CREATE TABLE [dbo].[StoryTranslation]
(
	[StoryTranslationId] INT IDENTITY (1, 1) NOT NULL , 
    [FacebookPostDetailId] INT NULL, 
    [Story] VARCHAR(4000) NULL, 
    [Locale] VARCHAR(5) NULL,
	CONSTRAINT [FK_StoryTranslation_FacebookPostDetail] FOREIGN KEY ([FacebookPostDetailId]) REFERENCES [FacebookPostDetail]([FacebookPostDetailId]),
    CONSTRAINT [PK_StoryTranslation] PRIMARY KEY CLUSTERED ([StoryTranslationId] ASC) 
)
