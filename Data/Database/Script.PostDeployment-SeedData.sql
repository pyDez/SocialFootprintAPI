/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/***** SEED DATA FOR AppUsers TABLE *****/

--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp]) VALUES ('Michael', 'Jordan', 'michael@bulls.com',0,'12-10-2005 12:32:10.12999', 1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','MJordan','password','SecurityStamp' );
--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp]) VALUES ('LaBron', 'James', 'labron@heat.com',0,'12-10-2005 12:32:10.12999',  1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','JLaBron','password','SecurityStamp');
--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp])  VALUES ('Kobe', 'Bryant', 'kobe@lakers.com',0,'12-10-2005 12:32:10.12999',  1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','KBryant','password','SecurityStamp');
--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp]) VALUES ('Kevin', 'Durant', 'kevin@thunder.com',1,'12-10-2005 12:32:10.12999',  1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','KDurant','password','SecurityStamp');
--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp]) VALUES ('Kyrie', 'Irving', 'kyrie@cavs.com',1,'12-10-2005 12:32:10.12999',  1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','IKyrie','password','SecurityStamp');
--INSERT INTO [AppUser] ([FirstName], [LastName],[EmailAddress],[Gender],[Birthday], [Activated],[SigningUpDate],[LastLogInDate],[UserName],[PasswordHash],[SecurityStamp]) VALUES ('Chris', 'Paul', 'chris@clippers.com',1,'12-10-2005 12:32:10.12999',  1,'12-10-2005 12:32:10.12999','12-10-2013 12:32:10.12999','PChris','password','SecurityStamp');

--INSERT INTO [ExternalLogin] VALUES (1,'Facebook','ThisIsAKey' );
--INSERT INTO [ExternalLogin] VALUES (2,'Twitter','ThisIsAKey' );
--INSERT INTO [ExternalLogin] VALUES (3,'Linkedin','ThisIsAKey' );
--INSERT INTO [ExternalLogin] VALUES (4,'Google','ThisIsAKey' );
--INSERT INTO [ExternalLogin] VALUES (5,'Microsoft','ThisIsAKey' );
--INSERT INTO [ExternalLogin] VALUES (6,'Amazon','ThisIsAKey' );



INSERT INTO [Category] ([Label]) VALUES( 'Other');
INSERT INTO [Category] ([Label]) VALUES( 'Comic');
INSERT INTO [Category] ([Label]) VALUES( 'Policy');
INSERT INTO [Category] ([Label]) VALUES( 'Art');
INSERT INTO [Category] ([Label]) VALUES( 'Spirituality');
INSERT INTO [Category] ([Label]) VALUES( 'Family');
INSERT INTO [Category] ([Label]) VALUES( 'Friends');
INSERT INTO [Category] ([Label]) VALUES( 'Business');
INSERT INTO [Category] ([Label]) VALUES( 'People');
INSERT INTO [Category] ([Label]) VALUES( 'Sport');
INSERT INTO [Category] ([Label]) VALUES( 'Vacation');
INSERT INTO [Category] ([Label]) VALUES( 'Well-being');
INSERT INTO [Category] ([Label]) VALUES( 'Love');
INSERT INTO [Category] ([Label]) VALUES( 'WTF');


--/***** SEED DATA FOR Personalities TABLE *****/
--INSERT INTO [Skill] VALUES(1, 2,150,378);
--INSERT INTO [Skill] VALUES(1, 3,180,405);
--INSERT INTO [Skill] VALUES(1, 4,10,38);
--INSERT INTO [Skill] VALUES(1, 1,159,208);
--INSERT INTO [Skill] VALUES(1, 5,100,825);

--/***** SEED DATA FOR Posts TABLE *****/

--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(1, 1, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(2, 3, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(2, 4, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(3, 2, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(4, 3, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(5, 2, 1, '12-10-2005 12:32:10.12999');
--INSERT INTO [Post] ([AppUserId], [CategoryId],[Activated],[CreationDate]) VALUES(6, 4, 1, '12-10-2005 12:32:10.12999');


--/***** SEED DATA FOR Votes TABLE *****/
--INSERT INTO [Vote] VALUES(1, 3,4);
--INSERT INTO [Vote] VALUES(1,5,1 );
--INSERT INTO [Vote] VALUES(2,6,2 );
--INSERT INTO [Vote] VALUES(3,4,3 );
--INSERT INTO [Vote] VALUES(4,5,6);
--INSERT INTO [Vote] VALUES(5,1,3 );
--INSERT INTO [Vote] VALUES(6,3,5);


--/***** SEED DATA FOR Friendship TABLE *****/
--INSERT INTO [Friendship] VALUES(1,3);
--INSERT INTO [Friendship] VALUES(1,5);
--INSERT INTO [Friendship] VALUES(3,4);
--INSERT INTO [Friendship] VALUES(4,5);
--INSERT INTO [Friendship] VALUES(5,6);
--INSERT INTO [Friendship] VALUES(6,3);
--INSERT INTO [Friendship] VALUES(2,1);
--INSERT INTO [Friendship] VALUES(2,3);
--INSERT INTO [Friendship] VALUES(2,4);
--INSERT INTO [Friendship] VALUES(2,5);
--INSERT INTO [Friendship] VALUES(2,6);




--INSERT INTO [FacebookPostDetail] VALUES(1, 2088787,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',NULL);
--INSERT INTO [FacebookPostDetail] VALUES(2, 2088788,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',1);
--INSERT INTO [FacebookPostDetail] VALUES(3, 2088789,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',NULL);
--INSERT INTO [FacebookPostDetail] VALUES(4, 2088790,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',NULL);
--INSERT INTO [FacebookPostDetail] VALUES(5, 2088791,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',2);
--INSERT INTO [FacebookPostDetail] VALUES(6, 2088792,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',NULL);
--INSERT INTO [FacebookPostDetail] VALUES(7, 2088793,'https://mobilewhoyouare.azurewebsites.net','caption','Kikoo les amis', 'prout',87975465464,'https://mobilewhoyouare.azurewebsites.net/Images/Sigmund.jpg','private','novideo','cool','py is a BG','cool', '12-10-2005 12:32:10.12999',NULL);

--INSERT INTO [FacebookUserDetail] VALUES(1, 2088787,2088787);
--INSERT INTO [FacebookUserDetail] VALUES(2, 2088787,2088788);
--INSERT INTO [FacebookUserDetail] VALUES(3, 2088787,2088789);
--INSERT INTO [FacebookUserDetail] VALUES(4, 2088787,2088790);
--INSERT INTO [FacebookUserDetail] VALUES(5, 2088787,2088791);
--INSERT INTO [FacebookUserDetail] VALUES(6, 2088787,2088792);
     


/****** Object:  Table [dbo].[CacheState]    Script Date: 07/25/2012 13:38:35 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CacheState]') AND type in (N'U'))
DROP TABLE [dbo].[CacheState]
GO


/****** Object:  Table [dbo].[CacheState]    Script Date: 07/25/2012 13:38:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CacheState](
	[CacheKeyHash] [binary](20) NOT NULL,
	[RoutePattern] [nvarchar](256) NOT NULL,
	[ResourceUri] [nvarchar](256) NOT NULL,
	[ETag] [nvarchar](100) NOT NULL,
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_CacheState] PRIMARY KEY CLUSTERED 
(
	[CacheKeyHash] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)  
)  

GO

SET ANSI_PADDING OFF
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_AddUpdateCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Server_AddUpdateCache
GO

-- =============================================
-- Author:		Ali Kheyrollahi
-- Create date: 2012-07-12
-- Description:	Adds or updates cache entry
-- =============================================
CREATE PROCEDURE Server_AddUpdateCache
	@cacheKeyHash	BINARY(20),
	@routePattern	NVARCHAR(256),
	@resourceUri	NVARCHAR(256),
	@eTag			NVARCHAR(100),
	@lastModified	DATETIME
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON

	BEGIN TRAN
	IF EXISTS (SELECT 1 FROM dbo.CacheState 
			WITH (UPDLOCK,SERIALIZABLE) WHERE CacheKeyHash = @cacheKeyHash)
		BEGIN
			UPDATE dbo.CacheState SET 
					ETag = @eTag,
					LastModified = @lastModified,
					RoutePattern = @routePattern,
					ResourceUri	 = @resourceUri
				WHERE CacheKeyHash = @cacheKeyHash
		END
	ELSE
	
		BEGIN
			INSERT INTO dbo.CacheState 
				(CacheKeyHash, RoutePattern, ResourceUri, ETag, LastModified)
			values 
				(@cacheKeyHash, @routePattern, @resourceUri, @eTag, @lastModified)
		END
	COMMIT TRAN

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_ClearCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Server_ClearCache]
GO

-- =============================================
-- Author:		Carl Duguay
-- Create date:	2013-07-10
-- Description:	Removes all CacheKey records
-- =============================================
CREATE PROCEDURE Server_ClearCache	 
AS
BEGIN
	SET NOCOUNT OFF
	DELETE FROM [dbo].[CacheState]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_DeleteCacheById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Server_DeleteCacheById]
GO

-- =============================================
-- Author:		Ali Kheyrollahi
-- Create date: 2012-07-12
-- Description:	Deletes a CacheKey record by its id
-- =============================================
CREATE PROCEDURE Server_DeleteCacheById
	@CacheKeyHash BINARY(20) 
AS
BEGIN
	SET NOCOUNT OFF
	DELETE FROM dbo.CacheState WHERE CacheKeyHash = @CacheKeyHash
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_DeleteCacheByResourceUri]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Server_DeleteCacheByResourceUri
GO

-- =============================================
-- Author:		Ali Kheyrollahi
-- Create date: 2013-11-16
-- Description:	Deletes all CacheKey records by its resource uri
-- =============================================
CREATE PROCEDURE Server_DeleteCacheByResourceUri
	@resourceUri NVARCHAR(256) 
AS
BEGIN
	SET NOCOUNT OFF

	DELETE FROM dbo.CacheState WHERE ResourceUri = @resourceUri
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_DeleteCacheByRoutePattern]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Server_DeleteCacheByRoutePattern
GO

-- =============================================
-- Author:		Ali Kheyrollahi
-- Create date: 2012-07-12
-- Description:	Deletes all CacheKey records by its route pattern
-- =============================================
CREATE PROCEDURE Server_DeleteCacheByRoutePattern
	@routePattern NVARCHAR(256) 
AS
BEGIN
	SET NOCOUNT OFF

	DELETE FROM dbo.CacheState WHERE RoutePattern = @routePattern
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server_GetCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Server_GetCache
GO

-- =============================================
-- Author:		Ali Kheyrollahi
-- Create date: 2012-07-12
-- Description:	returns cache entry by its Id
-- =============================================
CREATE PROCEDURE Server_GetCache
	@cacheKeyHash		BINARY(20)
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		ETag, LastModified
	FROM
		dbo.CacheState
	WHERE
		CacheKeyHash = @cacheKeyHash

END
GO


-- defining TVP type

CREATE TYPE dbo.TVPType AS TABLE
(
    id INT
);

CREATE TYPE dbo.TVPTypeString AS TABLE
(
    id NVARCHAR(MAX)
);


-- Text mining

CREATE TABLE [iRocks].[dbo].[ExclusionTerm] (
    [Terme] [nvarchar](128)
)

CREATE TABLE [iRocks].[dbo].[PostTerm] (
    [Terme] [nvarchar](128),
    [Score] [float]
)
CREATE TABLE [iRocks].[dbo].[TermInPost] (
    [Term] [nvarchar](128),
    [Frequency] [int],
    [PostId] [int]
)