﻿CREATE TABLE [dbo].[PostUrl]
(
	[PostUrlId] INT         IDENTITY (1, 1) NOT NULL,  
    [Url] VARCHAR(4000) NOT NULL,
	[PostId] INT NOT NULL,
	[Provider] VARCHAR(4000) NOT NULL 
	CONSTRAINT [FK_PostUrl_Post] FOREIGN KEY ([PostId]) REFERENCES [Post]([PostId]),
	CONSTRAINT [PK_PostUrl] PRIMARY KEY CLUSTERED ([PostUrlId] ASC)
)