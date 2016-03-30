CREATE TABLE [dbo].[Skill]
(
	[SkillId] INT IDENTITY (1, 1) NOT NULL,  
    [AppUserId] INT NOT NULL, 
    [CategoryId] INT NOT NULL, 
    [SkillLevel] FLOAT NULL, 
    [MaxSkillLevel] FLOAT NULL, 
    CONSTRAINT [FK_Skill_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [AppUser]([AppUserId]),
	CONSTRAINT [FK_Skill_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([CategoryId]),
	CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED ([SkillId] ASC)
)
