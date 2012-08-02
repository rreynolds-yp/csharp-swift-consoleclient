/*
<script release="Initial" xmlns="http://tempuri.org/Schema.xsd">
	<modifications>
		<modification date="03/21/2010" author="Phillip Clark">Initial database</modification>
		</modifications><modification date="05/19/2010" author="Phillip Clark">
			Removed the Application and Environment tables. App and environment are already handled
			in StronMail. The app and environment values in the MailTracking table allow query and
			referrence.
		</modification>
	<dependencies>
	</dependencies>
</script>
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------------------------------------------------------
--	Create the database if it is not present.
-------------------------------------------------------------------------------
USE master
GO
IF IS_SRVROLEMEMBER('securityadmin') = 1
	AND DB_ID (N'tmail') IS NOT NULL
	DROP DATABASE [tmail]
GO
IF IS_SRVROLEMEMBER('securityadmin') = 1
	AND DB_ID (N'tmail') IS NULL
	CREATE DATABASE [tmail];
GO

-------------------------------------------------------------------------------
--	Ensure the database roles are declared on tmail.
-------------------------------------------------------------------------------
USE [tmail]
GO
---- Add an admin role.
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'tmail_admins' AND type = 'R')
	CREATE ROLE [tmail_admins] AUTHORIZATION [dbo]
	-- tmail_admins are database owners.
	EXECUTE sp_addrolemember @rolename = 'db_owner', @membername= 'tmail_admins'
GO

---- Create a role for tmail users.
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'tmail_users' AND type = 'R')
	CREATE ROLE [tmail_users] AUTHORIZATION [dbo]
GO

-------------------------------------------------------------------------------
-- UNCOMMENT ONLY TO INITIALIZE SECURITY
-------------------------------------------------------------------------------
--USE [master]
--GO
----
---- Ensure the application user accounts are present on the database instance;
---- these commands import the users from the domain...
----
--IF NOT EXISTS (SELECT [name] FROM [syslogins] WHERE [name] = 'NCA\svc_TMailAdmin')
--BEGIN
--    -- tmailAdmin is the account used by installation utilities and the
--    -- deployment tools. This user is the database 'onwer'.
--	CREATE LOGIN [NCA\svc_TMailAdmin] FROM WINDOWS;
--END
	
--IF NOT EXISTS (SELECT [name] FROM [syslogins] WHERE [name] = 'NCA\svc_TMailWebService') 
--    -- tmailWebUser is the account used by the tmail Service API (web-service). 
--    -- This user should be granted read on most tables and views and execute on the stored-procs
--    -- supporting the web application.
--	CREATE LOGIN [NCA\svc_TMailWebService] FROM WINDOWS;
	
--IF NOT EXISTS (SELECT [name] FROM [syslogins] WHERE [name] = 'NCA\svc_TMailAgent') 
--	-- tmailAgent is the account used by the tmail Agent (windows-service)
--    -- This user should be granted read on most tables and views and execute on the stored-procs
--    -- supporting the agent application.
--	CREATE LOGIN [NCA\svc_TMailAgent] FROM WINDOWS;
--GO

-------------------------------------------------------------------------------
-- Create roles and role members in the TMail database
-------------------------------------------------------------------------------
USE [tmail]
GO
-- Add the admin account to the database.
CREATE USER [TMailAdmin] FOR LOGIN [NCA\svc_TMailAdmin] WITH DEFAULT_SCHEMA=[dbo]
-- tmail_admins are database owners.
EXECUTE sp_addrolemember @rolename = 'db_owner', @membername= 'tmail_admins'
-- Add the admin account to the admin role.
EXECUTE sp_addrolemember @rolename = 'tmail_admins', @membername= 'TMailAdmin'
GO
---- Add the web user to the database.
--CREATE USER [TMailWebService] FOR LOGIN [NCA\svc_TMailWebService] WITH DEFAULT_SCHEMA=[dbo]
---- Add the web user to the tmail users role.
--EXECUTE sp_addrolemember @rolename = 'tmail_users', @membername= 'TMailWebService'
--GO
---- Add the agent user to the database.
--CREATE USER [TMailAgent] FOR LOGIN [NCA\svc_TMailAgent] WITH DEFAULT_SCHEMA=[dbo]
---- Add the agent user to the tmail users role.
--EXECUTE sp_addrolemember @rolename = 'tmail_users', @membername= 'TMailAgent'
--GO

-------------------------------------------------------------------------------
-- Add our custom error messages
-------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.messages WHERE message_id = 50200)		
	EXEC sp_dropmessage @msgnum = 50200, @lang = 'us_english'	
IF EXISTS (SELECT * FROM sys.messages WHERE message_id = 50201)		
	EXEC sp_dropmessage @msgnum = 50201, @lang = 'us_english'	
IF EXISTS (SELECT * FROM sys.messages WHERE message_id = 50202)		
	EXEC sp_dropmessage @msgnum = 50202, @lang = 'us_english'	
IF EXISTS (SELECT * FROM sys.messages WHERE message_id = 50203)		
	EXEC sp_dropmessage @msgnum = 50203, @lang = 'us_english'	

IF NOT EXISTS (SELECT * FROM sys.messages WHERE message_id = 50200)
	EXEC sp_addmessage @msgnum = 50200, 
		@severity = 16, 
		@msgtext = N'Object not found: %s''%s''.', 
		@lang = 'us_english'	

IF NOT EXISTS (SELECT * FROM sys.messages WHERE message_id = 50201)
	EXEC sp_addmessage @msgnum = 50201, 
		@severity = 16, 
		@msgtext = N'Inconsistent object data: %s''%s''. Update your view of the object and resubmit your changes.', 
		@lang = 'us_english'
		
IF NOT EXISTS (SELECT * FROM sys.messages WHERE message_id = 50202)
	EXEC sp_addmessage @msgnum = 50202, 
		@severity = 16, 
		@msgtext = N'Object not allowed: %s''%s''.', 
		@lang = 'us_english'	

IF NOT EXISTS (SELECT * FROM sys.messages WHERE message_id = 50203)
	EXEC sp_addmessage @msgnum = 50203, 
		@severity = 16, 
		@msgtext = N'Object already exists: %s''%s''.', 
		@lang = 'us_english'	

CREATE TABLE [InstanceConfig]
(
	-- IRV Data Center, partitioned for multi-master merge replication at 0.
  ID INT IDENTITY(0,1),
  
	-- ASH Data Center, partitioned for multi-master merge replication at 0x01000000.
  --ID BIGINT IDENTITY(16777216,1),
  
  -- XXX Data Center, partitioned for multi-master merge replication at 0x02000000.
  --ID BIGINT IDENTITY(33554432,1),

  CurrentEnvironment VARCHAR(40) NOT NULL,
  SchemaVersion INT NOT NULL,
  
  DateCreated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  DateUpdated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),   
  CONSTRAINT PK_InstanceConfig PRIMARY KEY ([ID])
)

GRANT SELECT ON [dbo].[InstanceConfig] TO [tmail_users]

CREATE TABLE [EnvironmentAuthorizations]
(
	-- IRV Data Center, partitioned for multi-master merge replication at 0.
  ID INT IDENTITY(0,1),
  
	-- ASH Data Center, partitioned for multi-master merge replication at 0x01000000.
  --ID BIGINT IDENTITY(16777216,1),
  
  -- XXX Data Center, partitioned for multi-master merge replication at 0x02000000.
  --ID BIGINT IDENTITY(33554432,1),

  AuthorizedEnvironment VARCHAR(40) NOT NULL, 
  DateCreated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  DateUpdated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(), 
  
  CONSTRAINT PK_EnvironmentAuthorizations PRIMARY KEY ([ID]),
  CONSTRAINT AK_EnvironmentAuthorizations_AuthorizedEnvironment UNIQUE NONCLUSTERED ([AuthorizedEnvironment])  
)

GRANT SELECT ON [dbo].[EnvironmentAuthorizations] TO [tmail_users]
GO

CREATE PROCEDURE [InstanceConfig_Select]
WITH EXEC AS OWNER
AS
	BEGIN
	SELECT TOP 1 [ID], 
		[CurrentEnvironment],
		[SchemaVersion],
		[DateCreated],
		[DateUpdated]   
		FROM dbo.[InstanceConfig]
	END
GO
GRANT EXECUTE ON [dbo].[InstanceConfig_Select] TO [tmail_users]
GO

CREATE PROCEDURE [InstanceConfig_Create]
  @CurrentEnvironment VARCHAR(40),
  @SchemaVersion INT
WITH EXEC AS OWNER
AS
	BEGIN
	DECLARE @txCount INT
	DECLARE @timestamp DATETIME2(7)
	DECLARE @id INT
	SET @timestamp = GETUTCDATE()
	
	SET NOCOUNT ON
		
	BEGIN TRY
		SET @txCount = @@TRANCOUNT
		IF @txCount = 0
			BEGIN TRANSACTION
		ELSE
			SAVE TRANSACTION LocalSavepoint
		
		IF EXISTS(SELECT [ID] FROM dbo.[InstanceConfig])
			BEGIN			
			RAISERROR (50203, 16, 1, N'InstanceConfig ', 'only a single-instance is allowed')
			END
		
		-- create the instance config
		INSERT INTO dbo.[InstanceConfig] ([CurrentEnvironment], [SchemaVersion], [DateCreated])
			VALUES (@CurrentEnvironment, @SchemaVersion, @timestamp)
			
		-- automatically authorize the current environment
		INSERT INTO dbo.[EnvironmentAuthorizations] ([AuthorizedEnvironment], [DateCreated], [DateUpdated])
			VALUES (@CurrentEnvironment, @timestamp, @timestamp)
		
		IF @txCount = 0
			COMMIT TRANSACTION
		
		EXEC dbo.[InstanceConfig_Select]
	END TRY
	BEGIN CATCH			
		DECLARE @ErrorMessage NVARCHAR(2047)
    DECLARE @ErrorNumber INT
    DECLARE @ErrorSeverity INT
    DECLARE @ErrorState INT
    DECLARE @xactState INT
    
    SET @xactState = XACT_STATE()

    SELECT     
        @ErrorMessage = ERROR_MESSAGE(),                
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE()

		IF @txCount = 0 AND @xactState <> 0
			ROLLBACK TRANSACTION			
		ELSE IF @xactState <> 0
			ROLLBACK TRANSACTION LocalSavepoint

    -- Reraise the error
    RAISERROR ('|%d| %s', -- formatted for [num] msg
      @ErrorSeverity, -- Severity.
      @ErrorState, -- State.
      @ErrorNumber, -- Message number
      @ErrorMessage -- Message text.        
      )
	END CATCH	
	END   
GO
GRANT EXECUTE ON [dbo].[InstanceConfig_Create] TO [tmail_users]
GO

CREATE PROCEDURE [EnvironmentAuthorization_Create]
  @AuthorizedEnvironment VARCHAR(40)
WITH EXEC AS OWNER
AS
	BEGIN
	DECLARE @txCount INT
	DECLARE @timestamp DATETIME2(7)
	DECLARE @id INT
	SET @timestamp = GETUTCDATE()
	
	SET NOCOUNT ON
		
	BEGIN TRY
		SET @txCount = @@TRANCOUNT
		IF @txCount = 0
			BEGIN TRANSACTION
		ELSE
			SAVE TRANSACTION LocalSavepoint
		
		-- create the authorization
		INSERT INTO dbo.[EnvironmentAuthorizations] ([AuthorizedEnvironment], [DateCreated])
			VALUES (@AuthorizedEnvironment, @timestamp)
			
		IF @txCount = 0
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH			
		DECLARE @ErrorMessage NVARCHAR(2047)
    DECLARE @ErrorNumber INT
    DECLARE @ErrorSeverity INT
    DECLARE @ErrorState INT
    DECLARE @xactState INT
    
    SET @xactState = XACT_STATE()

    SELECT     
        @ErrorMessage = ERROR_MESSAGE(),                
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE()

		IF @txCount = 0 AND @xactState <> 0
			ROLLBACK TRANSACTION			
		ELSE IF @xactState <> 0
			ROLLBACK TRANSACTION LocalSavepoint

    -- Reraise the error
    RAISERROR ('|%d| %s', -- formatted for [num] msg
      @ErrorSeverity, -- Severity.
      @ErrorState, -- State.
      @ErrorNumber, -- Message number
      @ErrorMessage -- Message text.        
      )
	END CATCH	
	END   
GO
GRANT EXECUTE ON [dbo].[EnvironmentAuthorization_Create] TO [tmail_users]
GO

-------------------------------------------------------------------------------
-- MailTrackingStatus
--
-- Indicates the various states for a mailing. These are tmail specific 
-- workflow states and do not reflect the internal StrongMail states for 
-- a mailing.
-------------------------------------------------------------------------------
CREATE TABLE [MailTrackingStatus]
(
  ID INT NOT NULL,
  Status VARCHAR(20) NOT NULL,
  Description VARCHAR(400),
  DateCreated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  DateUpdated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  
  CONSTRAINT PK_MailTrackingStatus PRIMARY KEY ([ID])
)
GRANT SELECT ON [dbo].[MailTrackingStatus] TO [tmail_users]

CREATE TABLE [MailTracking]
(
  ID UNIQUEIDENTIFIER NOT NULL,
  DateCreated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  DateUpdated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  
  StatusID INT NOT NULL,
  Application VARCHAR(40) NOT NULL,
  Environment VARCHAR(40) NOT NULL,
  EmailTemplate VARCHAR(260) NOT NULL,
  
  CONSTRAINT PK_MailTracking PRIMARY KEY ([ID]),
  CONSTRAINT FK_MailTracking_Status FOREIGN KEY ([StatusID]) REFERENCES [MailTrackingStatus]([ID]),
)
CREATE INDEX AK_MailTracking_DateCreated ON [MailTracking]([DateCreated])
CREATE INDEX AK_MailTracking_DateUpdated ON [MailTracking]([DateUpdated])
CREATE INDEX AK_MailTracking_Application ON [MailTracking]([Application]) INCLUDE ([Environment])

GO
GRANT SELECT ON [dbo].[MailTracking] TO [tmail_users]
GO

CREATE TABLE [MailTrackingNote]
(
	-- IRV Data Center, partitioned for multi-master merge replication at 0.
  ID BIGINT IDENTITY(0,1),
  
	-- ASH Data Center, partitioned for multi-master merge replication at 0x100000000000000.
  --ID BIGINT IDENTITY(72057594037927936,1),
  
  -- XXX Data Center, partitioned for multi-master merge replication at 0x200000000000000.
  --ID BIGINT IDENTITY(144115188075855872,1),  

  TrackingID UNIQUEIDENTIFIER NOT NULL,
  DateCreated DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
  
  StatusID INT NOT NULL,
  StatusNote VARCHAR(MAX) NOT NULL,
  
  CONSTRAINT PK_MailTrackingNote PRIMARY KEY ([ID]),
  CONSTRAINT FK_MailTrackingNote_TrackingID FOREIGN KEY ([TrackingID]) REFERENCES [MailTracking]([ID]),
)
CREATE INDEX AK_MailTrackingNote_DateCreated ON [MailTracking]([DateCreated])

GO
GRANT SELECT ON [dbo].[MailTrackingNote] TO [tmail_users]
GO

CREATE PROCEDURE [MailTracking_SelectByID] @ID UNIQUEIDENTIFIER
WITH EXEC AS OWNER
AS
BEGIN
	SELECT [ID], [DateCreated], [DateUpdated],
			[StatusID], [Application], [Environment], [EmailTemplate]
		FROM dbo.[MailTracking]
		  WHERE [ID] = @ID
	SELECT [ID], [TrackingID], [DateCreated], [StatusID], [StatusNote]	
		FROM dbo.[MailTrackingNote]
			WHERE [TrackingID] = @ID
				ORDER BY [DateCreated] ASC
END
GO
GRANT EXECUTE ON [dbo].[MailTracking_SelectByID] TO [tmail_users]
GO

CREATE PROCEDURE [MailTracking_Update]
  @ID UNIQUEIDENTIFIER,
  @DateUpdated DATETIME2(7),
  @StatusID BIGINT,
  @Note VARCHAR(2000)  
WITH EXEC AS OWNER
AS
	BEGIN
	DECLARE @txCount INT
	DECLARE @idStr VARCHAR(20)
	DECLARE @timestamp DATETIME2(7)
	SET @timestamp = GETUTCDATE()
	
	SET NOCOUNT ON
		
	BEGIN TRY
		SET @txCount = @@TRANCOUNT
		IF @txCount = 0
			BEGIN TRANSACTION
		ELSE
			SAVE TRANSACTION LocalSavepoint
			
		-- Determine if the object exists.
		IF NOT EXISTS (SELECT [ID] FROM [dbo].[MailTracking] WHERE [ID] = @ID)
			BEGIN
			SET @idStr = CONVERT(VARCHAR(36), @ID)			
			RAISERROR (50200, 16, 1, N'MailTracking ', @idStr) -- Object not found
			END
		
		-- Update the MailingStatus
		UPDATE [dbo].[MailTracking] 
			SET [StatusID] = @StatusID,
				[DateUpdated] = @timestamp
			WHERE [ID] = @ID
				AND [DateUpdated] = @DateUpdated
		
		-- If nothing updated then tis inconsistent object data.
		IF @@ROWCOUNT = 0
			BEGIN
			SET @idStr = CONVERT(VARCHAR(36), @ID)			
			RAISERROR (50201, 16, 1, N'MailTracking ', @idStr) -- Inconsistent object data
			END
			
		INSERT INTO dbo.[MailTrackingNote] ([TrackingID], [DateCreated], [StatusID], [StatusNote])
			VALUES (@ID, @timestamp, @statusID, @Note)		
			
		IF @txCount = 0
			COMMIT TRANSACTION
		EXEC dbo.[MailTracking_SelectByID] @ID
		
	END TRY
	BEGIN CATCH			
		DECLARE @ErrorMessage NVARCHAR(2047)
    DECLARE @ErrorNumber INT
    DECLARE @ErrorSeverity INT
    DECLARE @ErrorState INT
    DECLARE @xactState INT
    
    SET @xactState = XACT_STATE()

    SELECT     
        @ErrorMessage = ERROR_MESSAGE(),                
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE()

		IF @txCount = 0 AND @xactState <> 0
			ROLLBACK TRANSACTION			
		ELSE IF @xactState <> 0
			ROLLBACK TRANSACTION LocalSavepoint

    -- Reraise the error
    RAISERROR ('|%d| %s', -- formatted for [num] msg
      @ErrorSeverity, -- Severity.
      @ErrorState, -- State.
      @ErrorNumber, -- Message number
      @ErrorMessage -- Message text.        
      )
	END CATCH	
	END
GO
GRANT EXECUTE ON [dbo].[MailTracking_Update] TO [tmail_users]
GO

CREATE PROCEDURE [MailTracking_Create]
  @ID UNIQUEIDENTIFIER,
  @StatusID BIGINT,
	@Application VARCHAR(40),
  @Environment VARCHAR(40),
  @EmailTemplate VARCHAR(260),  
  @Note VARCHAR(2000)
WITH EXEC AS OWNER
AS
	BEGIN
	DECLARE @txCount INT
	DECLARE @timestamp DATETIME2(7)
	DECLARE @ticketStr VARCHAR(36)
	SET @timestamp = GETUTCDATE()
	
	SET NOCOUNT ON
		
	BEGIN TRY
		SET @txCount = @@TRANCOUNT
		IF @txCount = 0
			BEGIN TRANSACTION
		ELSE
			SAVE TRANSACTION LocalSavepoint
				
		IF EXISTS(SELECT [DateCreated] FROM dbo.[MailTracking]
			WHERE [ID] = @ID)
			BEGIN
			SET @ticketStr = CONVERT(VARCHAR(36), @ID)
			RAISERROR (50203, 16, 1, N'MailTracking ', @ticketStr)
			END
		
		-- We're the first one to use the ID, 
		--  create the mailing
		INSERT INTO dbo.[MailTracking]([ID], [DateCreated], [DateUpdated],
			[StatusID], [Application], [Environment], [EmailTemplate])
			VALUES (@ID, @timestamp, @timestamp, @StatusID, @Application, @Environment,
			@EmailTemplate)
			
		INSERT INTO dbo.[MailTrackingNote] ([TrackingID], [DateCreated], [StatusID], [StatusNote])
			VALUES (@ID, @timestamp, @statusID, @Note)  
		
		IF NOT EXISTS(SELECT [ID] FROM dbo.[EnvironmentAuthorizations]
			WHERE [AuthorizedEnvironment] = @Environment)
			BEGIN
			EXEC dbo.[MailTracking_Update] @ID, @timestamp, 1001 /* Failed */, 'Environment not allowed'
			END
			
		IF @txCount = 0
			COMMIT TRANSACTION
			
		EXEC dbo.[MailTracking_SelectByID] @ID		
	END TRY
	BEGIN CATCH			
		DECLARE @ErrorMessage NVARCHAR(2047)
    DECLARE @ErrorNumber INT
    DECLARE @ErrorSeverity INT
    DECLARE @ErrorState INT
    DECLARE @xactState INT
    
    SET @xactState = XACT_STATE()

    SELECT     
        @ErrorMessage = ERROR_MESSAGE(),                
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE()

		IF @txCount = 0 AND @xactState <> 0
			ROLLBACK TRANSACTION			
		ELSE IF @xactState <> 0
			ROLLBACK TRANSACTION LocalSavepoint

    -- Reraise the error
    RAISERROR ('|%d| %s', -- formatted for [num] msg
      @ErrorSeverity, -- Severity.
      @ErrorState, -- State.
      @ErrorNumber, -- Message number
      @ErrorMessage -- Message text.        
      )
	END CATCH	
	END   
GO
GRANT EXECUTE ON [dbo].[MailTracking_Create] TO [tmail_users]
GO
