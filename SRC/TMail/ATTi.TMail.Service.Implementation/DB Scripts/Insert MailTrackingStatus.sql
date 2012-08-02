/*
<script release="Initial" xmlns="http://tempuri.org/Schema.xsd">
	<modifications>
		<modification date="03/21/2010" author="Phillip Clark">Initial database</modification>
		<modification date="05/19/2010" author="Phillip Clark">Synchronized values with
		the ATTi.TMail.Common.Model.MailTrackingStatus enum.</modification>
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
--	Insert the tracking status'
-------------------------------------------------------------------------------
USE [tmail]
GO
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(0, 'None', 'Indicates that the mailing does not have status (initial state).')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(1, 'Accepted', 'Indicates that the mailing has been recorded in the queue.')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(2, 'Submitted', 'Indicates that the mailing has been submitted to StrongMail and a tracking serial number has been recorded.')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(3, 'Completed', 'Indicates that StrongMail has complete the mailing.')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(4, 'Canceled', 'Indicates that the mailing has been canceled.')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(5, 'Paused', 'Indicates that the mailing has been paused.')	
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(1001, 'Failed', 'Indicates that the mailing failed and a failure record has been created.')
INSERT INTO [MailTrackingStatus] ([ID], [Status], [Description])
	VALUES(1002, 'Dropped', 'Indicates that the mailing dropped and a failure record has been created.')	
GO