CREATE TABLE [dbo].[PostMedia]
(
	[PostMediaId] INT         IDENTITY (1, 1) NOT NULL,  
	[PostMediaTwitterId] VARCHAR(4000) NOT NULL,
    [Url] VARCHAR(4000) NOT NULL, 
    [Type] VARCHAR(4000) NOT NULL,
	[PostId] INT NOT NULL,
	[Provider] VARCHAR(4000) NOT NULL 
	CONSTRAINT [FK_PostMedia_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [PK_PostMedia] PRIMARY KEY CLUSTERED ([PostMediaId] ASC)
)