using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using ATTi.Core;
using ATTi.Core.Contracts;
using ATTi.Core.Trace;
using ATTi.SSO.Client;
using ATTi.TMail.Common;
using ATTi.TMail.Model;
using ATTi.TMail.Service.Implementation.Agent;
using ATTi.TMail.Service.Implementation.Agent.Messages;
using ATTi.TMail.Service.Implementation.Configuration;
using ATTi.TMail.StrongMail;
using ATTi.TMail.StrongMail.TransactionalApi;

namespace ATTi.TMail.Service.Implementation
{
	//using ATTI.Core.Logging;
	
	public partial class TMailService : ITMailService, ITraceable
	{
		Organization _atti;
		IEnumerable<Organization> _appCache;
		DateTime _appCacheStale = default(DateTime);
		AgentOutput _output;
		readonly bool _asAgent; 		
		public TMailService()
		{
           //this.TraceData(TraceEventType.Information,"TMailService Starting a new Agent");
			//this.LogInfo("TMailService Starting a new Agent");
			_output = new AgentOutput();
			//TODO remove this after remove SSO role based sec
			_asAgent = TMailConfigurationSection.Instance.IsServerOperationMode;
            _traceExecutionSecurityContext = TMailConfigurationSection.Instance.IsTraceExecutionContext;
		}

		#region ITMailService Members

		public void ValidateApplicationAndEnvironment(string app, string env)
		{
			RefreshApplicationCacheIfStale();
			Thread.MemoryBarrier();
			var apps = _appCache;
			var atti = _atti;
			Thread.MemoryBarrier();
            this.TraceData(TraceEventType.Information,"ValidateApplicationAndEnvironment: ValidateApplication and Environment");
			if (apps == null){
				
				return; // can't validate right now
			}

			var a = apps.TakeChildrenOf(atti).Where(o => o.name == app).SingleOrDefault();
			if (a == null)
			{
                ///this.LogInfo("TMailService ValidateAppAndEnv Template State from Strongmail:{0}", 
                //             String.Concat("APP not found in StrongMail: ", app, "/", env));
                this.TraceData(TraceEventType.Error, String.Concat("Application not found in StrongMail: ", app, "/", env));
				throw new ResourceNotFoundException(String.Concat("Application not found in StrongMail: ", app, "/", env));
			}

			var e = apps.TakeChildrenOf(a).Where(o => o.name == env).SingleOrDefault();
			if (e == null){
				
                //this.LogInfo("TMailService ValidateAppAndEnv Template State from Strongmail:{0}", 
                //             String.Concat("Environment not found in StrongMail: ", app, "/", env));
                this.TraceData(TraceEventType.Error, String.Concat("Environment not found in StrongMail: ", app, "/", env));
				throw new ResourceNotFoundException(String.Concat("Environment not found in StrongMail: ", app, "/", env));
			}
            this.TraceData(TraceEventType.Information, "ValidateApplicationAndEnvironment: ValidateApplication and Environment - Passed");
		}

		public bool TemplateExists(string app, string env, string template)
		{
			//this.LogInfo("TMailService TemplateExists Check for valid arguments");
            this.TraceData(TraceEventType.Information, "TMailService TemplateExists Check for valid arguments");
			Require.IsNotEmpty("app", app);
			Require.IsNotEmpty("env", env);
			Require.IsNotEmpty("template", template);
            this.TraceData(TraceEventType.Information, "TMailService TemplateExists Done with argument validation, getting strongmail Instance");
			//this.LogInfo("TMailService TemplateExists Done with argument validation, getting strongmail Instance");
			var config = StrongMailConfigurationSection.Instance;
			using (var easApi = config.CreateTransactionalApiWebService())
			{
				try
				{
					long serialNum;
					string mailingID;
					MailingStates mailingState;
					string startTime;
					string endTime;
					long elapsedTime;
					string lastRestartTime;
					long restarts;
					long totaldatabaseRecords;
					long messagesDelivered;
					long messagesFailed;
					long messagesDeferred;
					long messagesInvalid;
					
                    //this.LogInfo("TMailService Begin GetMailing Template State from Strongmail");
                    this.TraceData(TraceEventType.Information, "TMailService Begin GetMailing Template State from Strongmail");
					var statusResult = easApi.GetStatus(config.EasApiCredentials, template, out serialNum, out mailingState, out mailingID,
						out startTime,
						out endTime, out elapsedTime, out lastRestartTime, out restarts, out totaldatabaseRecords,
						out messagesDelivered, out messagesFailed, out messagesDeferred, out messagesInvalid);
                    this.TraceData(TraceEventType.Information, "TMailService GetMailing Template State from Strongmail with status {0}", statusResult);
                    //this.LogInfo("TMailService GetMailing Template State from Strongmail with status {0}", statusResult);
					
					return mailingState == MailingStates.ACTIVE;
				}
				catch (Exception ex)
				{
					var m = String.Concat("Unexpected error while checking if template exists in StrongMail: ", app, "/", env, "/", template, ".");
					
                    //this.LogInfo("{0} with exp {1}",m,ex.Message);
                    //this.TraceData(TraceEventType.Information, "TMailService GetMailing Template State from Strongmail with status {0}", statusResult);
					this.TraceData(TraceEventType.Error, m, ex.FormatForLogging());
					YPMon.Critical("AGENT_UNEXPECTED_FAILURE", m, ex.FormatForLogging());
					
					throw;
				}
			}
		}
				
		public MailTracking GetMailingStatus(string app, string env, Guid mailingID)
		{
			//this.LogInfo("TMailService GetMailingStatus Check for valid arguments");
            this.TraceData(TraceEventType.Information, "TMailService GetMailing Status Check for valid arguments");
			Require.IsNotEmpty("app", app);
			Require.IsNotEmpty("env", env);

			//this.LogInfo("TMailService GetMailing Check for AuthenticationThreadPrincipal");
            this.TraceData(TraceEventType.Information, "TMailService GetMailing Check for AuthenticationThreadPrincipal");
			
			AuthenticationUtil.RequireAuthenticatedThreadPrincipal();
			
			//this.LogInfo("TMailService GetMailing Status from TrackingDB");
            this.TraceData(TraceEventType.Information, "TMailService GetMailing Status from TrackingDB");

			return ExecuteDbActionsWithinTransaction<MailTracking>(null, new DbAction<MailTracking>[] {
				(cn, a, l) =>
				{
					return PerformMailTrackingSelectByID(cn, mailingID);
				} });
		}

		public void CreateMailing(Guid id, string app, string env, string template, Recipient[] recipients)
		{
			//this.LogInfo("TMailService CreateMailing Check for valid arguments");
            this.TraceData(TraceEventType.Information, "TMailService CreateMailing Check for valid arguments");
			Require.IsNotEmpty("app", app);
			Require.IsNotEmpty("env", env);
			Require.IsNotEmpty("template", template);
			Require.IsNotNull("recipients", recipients);
			Require.AtLeastOneParam("recipients", recipients);
            this.TraceData(TraceEventType.Information, "TMailService CreateMailing Check for AuthenticationThreadPrincipal");
			//this.LogInfo("TMailService CreateMailing Check for AuthenticationThreadPrincipal");
			
			// Original submitter must have a valid authentication ticket.
			ISsoPrincipal principal = AuthenticationUtil.RequireAuthenticatedThreadPrincipal();
			
			//this.LogInfo("TMailService CreateMailing Begin Enqueuing for Agent");
			// Enqueue for Agent...
            this.TraceData(TraceEventType.Information, "TMailService CreateMailing Begin Enqueuing for Agent");
			_output.CreateAndSendMessage<MailingMessage>(AgentProtocol.CTMailAccept, m =>
				{
					m.AuthTicket = principal.GetRawAuthTicketData(true); //role-based sec stop here.
					m.MailTrackingID = id;
					m.Application = app;
					m.Environment = env;
					m.Template = template;
					m.Recipients = recipients;
				});
            this.TraceData(TraceEventType.Information, "TMailService CreateMailing Done Enqueuing for Agent");
			//this.LogInfo("TMailService CreateMailing Done Enqueuing for Agent");
		}

		#endregion

		public void RefreshApplicationCacheIfStale()
		{
			Thread.MemoryBarrier();
			var stale = _appCacheStale < DateTime.Now;
			Thread.MemoryBarrier();
            this.TraceData(TraceEventType.Information, "TMailService RefreshApplicationIfStale check if stale");
			//this.LogInfo("TMailService RefreshApplicationIfStale check if stale");
			if (stale)
			{
                this.TraceData(TraceEventType.Information, "TMailService RefreshApplicationIfStale is stale");
				//this.LogInfo("TMailService RefreshApplicationIfStale is stale");
				var config = StrongMailConfigurationSection.Instance;
				using (var studio = config.CreateMailingService())
				{
				
					try
					{
						//this.LogInfo("TMailService RefreshApplicationIfStale is starting");
                        this.TraceData(TraceEventType.Information, "TMailService RefreshApplicationIfStale is starting");
						var orgs = studio.AllOrganizations();
						var atti = orgs.SingleOrDefault(o => o.name == "ATTI");
						
						Thread.MemoryBarrier();
						_appCache = orgs;
						_atti = atti;
						_appCacheStale = DateTime.Now.Add(CApplicationCacheTimeout);
						Thread.MemoryBarrier();
                        this.TraceData(TraceEventType.Information, "TMailService RefreshApplicationIfStale is done");
						//this.LogInfo("TMailService RefreshApplicationIfStale is done");
					}
					catch (WebException e)
					{
						//this.LogInfo("TMailService RefreshApplicationIfStale is stale with WebException {0}",e.Message);
                        this.TraceData(TraceEventType.Error, "TMailService RefreshApplicationIfStale is stale with WebException {0}", e.Message);
						YPMon.Warn("STRONGMAIL_UNREACHABLE", e.FormatForLogging());
					}
					catch (SoapHeaderException e)
					{
						//this.LogInfo("TMailService RefreshApplicationIfStale is stale with SoapException {0}, with translated exception {1} ",
					   //	              e.Message,studio.TranslateException(e));
                        this.TraceData(TraceEventType.Error,
                            "TMailService RefreshApplicationIfStale is stale with SoapException {0}, with translated exception {1} ", 
                            e.Message,studio.TranslateException(e));
						throw studio.TranslateException(e);
					}
				}
			}
		}
				
		
		
	}
}
