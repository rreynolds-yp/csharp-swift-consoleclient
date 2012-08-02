/*
<script release="Initial" xmlns="http://tempuri.org/Schema.xsd">
	<modifications>
		<modification date="05/19/2010" author="Phillip Clark">Initial</modification>
	</modifications>
	<dependencies>
	</dependencies>
</script>
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------------------------------------------
--	Authorize the environment
-------------------------------------------------------------------------------
USE [tmail]
GO
DECLARE @env VARCHAR(40)
SET @env = 'dev'
--SET @env = 'test'
--SET @env = 'stage'
--SET @env = 'live'
EXEC [dbo].[InstanceConfig_Create] @env, 1
GO