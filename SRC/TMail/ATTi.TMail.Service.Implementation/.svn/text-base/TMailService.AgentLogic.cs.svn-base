using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Threading;
using System.Web.Services.Protocols;
using ATTi.Core;
using ATTi.Core.Contracts;
using ATTi.Core.Factory;
using ATTi.Core.Trace;
using ATTi.SSO.Client;
using ATTi.TMail.Common;
using ATTi.TMail.Model;
using ATTi.TMail.Service.Implementation.Agent;
using ATTi.TMail.Service.Implementation.Agent.Messages;
using ATTi.TMail.Service.Implementation.Configuration;

namespace ATTi.TMail.Service.Implementation
{
	public partial class TMailService: ITraceable
	{
		internal AgentProtocol Agent { get; private set; }
		public event LineWriterDelegate OnWriteLine;

		void WriteLine(params object[] args)
		{
			var dlg = OnWriteLine;
			YPMon.Debug<TMailService>("TMailService.WriteLine", String.Concat(args));
			if (dlg != null)
				dlg(this, args);
		}

		public void StartAgentLogic(Action<object, string> onStarted)
		{
			var agent = new AgentProtocol();
			Agent = agent;
			Agent.OnMailing = this.OnMailing;
			Agent.OnDelayedMailing = this.OnDelayedMailing;
			Agent.OnPausedMailing = this.OnPausedMailing;
			Agent.StartAgentLogic(onStarted);
		}

		public void StopAgentLogic()
		{
			Agent.StopAgentLogic();
		}

		/// <summary>
		/// Called when a MailingMessage is delivered to the agent (first delivery).
		/// </summary>
		void OnMailing(object sender, InboundMessageArgs<MailingMessage> args)
		{
			var m = args.Message;
			if (String.IsNullOrEmpty(m.AuthTicket))
			{
				YPMon.Warn("AUTHTICKET_MISSING",
					String.Concat("MailingMessage arrived without an auth-ticket and has been discarded. Details: { Application: ", m.Application,
					", Environment: ", m.Environment,
					", EmailTemplate: ", m.Template,
					", RecipientList: \"", m.Recipients.ToStrongMailEmailAddressStream(), "\" }"));
			}
			else
			{
				try
                {

					PerformMailingLogic(m.MailTrackingID, m.Application, m.Environment, m.Template, m.Recipients,
									 	PartialCreateStatus.None, m.AuthTicket, MailTrackingStatus.None, null);
					YPMon.Info("TMAIL_PERFORM_SM",
						string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
						m.Application, m.Environment, m.Template, Enum.GetName(typeof(MailTrackingStatus), MailTrackingStatus.None)));

					var principal = Thread.CurrentPrincipal;
					//Thread.CurrentPrincipal = principal;
                }
                catch (ApplicationException appEx) 
                {
                    if (_asAgent)
                    {
                        YPMon.Critical("UNHANDLED_APP_EXCEPTION_ONMAILING", string.Concat("AgentMode ",_asAgent),appEx.Message);
                    }
                    else
                        throw;
                }
                catch (Exception ex) 
                {
                    if (_asAgent)
                    {
						YPMon.Critical("UNHANDLED_EXCEPTION_ONMAILING", string.Concat("AgentMode ", _asAgent), ex.Message);
                    }
                    else
                        throw;
                }
			}
		}
		
		/// <summary>
		/// Called when a DelayedMailingMessage is redelivered to the agent (has been tried already)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		void OnDelayedMailing(object sender, InboundMessageArgs<DelayedMailingMessage> args)
		{
			var m = args.Message;
			var cachePrincipal = Thread.CurrentPrincipal;
			//ISsoPrincipal principal;
			//TODO this has to make more states .. this is not a good solution
			WriteLine("OnDelayedMailing started");
			try
			{
				WriteLine("OnDelayMailing attempt to PerformMailingLogic started");

				PerformMailingLogic(m.MailTrackingID, m.Application, m.Environment, m.Template,
					m.Recipients, m.PartialStatus, m.AuthTicket,
					m.UnrecordedStatus, m.UnrecordedNote);
				WriteLine("OnDelayMailing attempt to PerformMailingLogic Done");

			}
			catch (ApplicationException appEx)
			{
				if (_asAgent)
				{
					YPMon.Critical("UNHANDLED_APP_EXCEPTION_ONDELAYED_MAILING", string.Concat("AgentMode ", _asAgent), appEx.Message);
				}
				else
					throw;
			}
			catch (Exception ex)
			{
				if (_asAgent)
				{
					YPMon.Critical("UNHANDLED_EXCEPTION_ONDELAYED_MAILING", string.Concat("AgentMode ", _asAgent), ex.Message);
				}
				else
					throw;
			}

		}

		/// <summary>
		/// Called when a PausedMailingMessage is redelivered to the agent (has been tried already)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		void OnPausedMailing(object sender, InboundMessageArgs<PausedMailingMessage> args)
		{
			var m = args.Message;
			var cachePrincipal = Thread.CurrentPrincipal;
			//ISsoPrincipal principal;
			//TODO this has to make more states .. this is not a good solution
			WriteLine("OnPausedMailing started");
			try
			{
				WriteLine("OnPausedMailing attempt to PerformMailingLogic started");

				PerformMailingLogic(m.MailTrackingID, m.Application, m.Environment, m.Template,
					m.Recipients, m.PartialStatus, m.AuthTicket,
					m.UnrecordedStatus, m.UnrecordedNote);
				WriteLine("OnPausedMailing attempt to PerformMailingLogic Done");

			}
			catch (ApplicationException appEx)
			{
				if (_asAgent)
				{
					YPMon.Critical("UNHANDLED_APP_EXCEPTION_ONPAUSED_MAILING", string.Concat("AgentMode ", _asAgent), appEx.Message);
				}
				else
					throw;
			}
			catch (Exception ex)
			{
				if (_asAgent)
				{
					YPMon.Critical("UNHANDLED_EXCEPTION_ONPAUSED_MAILING", string.Concat("AgentMode ", _asAgent), ex.Message);
				}
				else
					throw;
			}

		}


		internal void PerformMailingLogic(Guid id, string app, string env, string template, Recipient[] recipients,
				PartialCreateStatus partial, string rawAuthTicketData, MailTrackingStatus unrecordedStatus, 
				string unrecordedNote)
		{
			Require.IsNotEmpty("app", app);
			Require.IsNotEmpty("env", env);
			Require.IsNotEmpty("template", template);
			Require.IsNotNull("recipients", recipients);
			Require.AtLeastOneParam("recipients", recipients);

			MailTracking tracking = null;
			while (true)
			{
				if (tracking == null && partial >= PartialCreateStatus.Saved)
				{
					YPMon.Info("TMAIL_PERFORM_SM",
								string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								app,env,template,partial.ToString()));
                    this.TraceInformation("Processing mailing logic for App: {0}, Env: {1}, Template: {2}, State: {3}",
                                app, env, template, partial.ToString());

					if (!TryLoadTracking(id, out tracking))
					{ // could not load, requeue and try again later...

						YPMon.Info("TMAIL_PERFORM_SM",
						string.Format("CANNOT LOAD TRACKING App: {0}, Env: {1}, Template: {2}, States: {3}",
						app, env, template, partial.ToString()));
                        this.TraceError("Cannot load tracking for App: {0}, Env: {1}, Template: {2}, State: {3}",
                                   app, env, template, Enum.GetName(typeof(PartialCreateStatus), tracking.Status));
                        //if (partial == PartialCreateStatus.Paused)
						//{
						//    Agent.CreateAndSendMessage<PausedMailingMessage>(AgentProtocol.CTMailPaused, m =>
						//    {
						//        m.AuthTicket = rawAuthTicketData;
						//        m.MailTrackingID = id;
						//        m.Application = app;
						//        m.Environment = env;
						//        m.Template = template;
						//        m.Recipients = recipients;
						//        m.PartialStatus = partial;
						//    });
						//}
						//else 
						//{
						//PAUSED mailing and deay mailing will retry now.
							Agent.CreateAndSendMessage<DelayedMailingMessage>(AgentProtocol.CTMailDelayed, m =>
							{
								m.AuthTicket = rawAuthTicketData;
								m.MailTrackingID = id;
								m.Application = app;
								m.Environment = env;
								m.Template = template;
								m.Recipients = recipients;
								m.PartialStatus = partial;
							});
						//}
						return;
					}
					if (unrecordedStatus != MailTrackingStatus.None && !String.IsNullOrEmpty(unrecordedNote))
					{
						// Record the unrecorded message
						if (!ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, partial, unrecordedStatus, unrecordedNote))
							return;
					}
				}
				switch (partial)
				{
					case PartialCreateStatus.None:
						// 1) Save the tracking record...
						tracking = Factory<MailTracking>.CreateInstance();
						tracking.ID = id;
						tracking.Application = app;
						tracking.Environment = env;
						tracking.EmailTemplate = template;
						tracking.Status = MailTrackingStatus.None;
						YPMon.Info("TMAIL_PERFORM_SM",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), tracking.Status)));
                        this.TraceInformation("Saving tracking record for App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), tracking.Status));

						if (TrySaveMailing(ref tracking, rawAuthTicketData, ref partial, recipients))
						{
							continue;
						}
						return;
                    case PartialCreateStatus.Dropped:
                        YPMon.Info("TMAIL_PERFORM_SM",
                                 string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
                                 app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
						//if (ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, partial, MailTrackingStatus.Dropped,
						//    String.Concat("Email message has invalid states or irrecoverable error: dropped ", app, "/", env, "."),false))
						//{
						//    continue;
						//}
                        return;
					case PartialCreateStatus.Saved:
						// 2) validate app and environment...
						try
						{
							ValidateApplicationAndEnvironment(app, env);
							partial = PartialCreateStatus.Validated;
							YPMon.Info("TMAIL_PERFORM_SM",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
							if (ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, partial, MailTrackingStatus.Accepted,
								String.Concat("Application and environment accepted: ", app, "/", env, ".")))
							{
								continue;
							}
						}
						catch (ResourceNotFoundException e)
						{
							ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, PartialCreateStatus.Failed, MailTrackingStatus.Failed,
								e.Message);
						}
						return;
					case PartialCreateStatus.Validated:
						// 3) record the email addresses
						partial = PartialCreateStatus.EmailsRecorded;
						YPMon.Info("TMAIL_PERFORM_SM",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
						if (ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, partial, MailTrackingStatus.Accepted,
							String.Concat("Email recipients: ", recipients.ToStrongMailEmailAddressStream())))
						{
							continue;
						}
						return;
					case PartialCreateStatus.EmailsRecorded:
						// 4) Submit to StrongMail...
						YPMon.Info("TMAIL_PERFORM_SM",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3} PreSubmit",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
						TrySubmitMailing(tracking, rawAuthTicketData, recipients, ref partial);
						YPMon.Info("TMAIL_PERFORM_SM",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3} PostSubmit",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
						return;
					default:
						break;
				}
			}
		}			

		bool TryLoadTracking(Guid id, out MailTracking tracking)
		{
			try
			{
				YPMon.Info("TMAIL_AGENTLOGIC_TRYLOADTRACKING", "TryLoadTracking started");
				tracking = ExecuteDbActionsWithinTransaction<MailTracking>(null, new DbAction<MailTracking>[] {
				(cn, a, l) =>
				{
					return PerformMailTrackingSelectByID(cn, id);
				} });
				return true;
			}
			catch (DbException e)
			{
				YPMon.Warn("TMAIL_LOADMAILINGTRACKING_SQLERROR",
					       string.Format("Guid: {0} SQLERROR: {1} ", id.ToString(), e.Message));
			}
			tracking = default(MailTracking);
			return false;
		}

		bool TrySaveMailing(ref MailTracking tracking,
			string rawAuthTicketData,
			ref PartialCreateStatus partial,
			Recipient[] recipients) 
		{
			YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", "TrySaveMailing started");
			try
			{
				YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", "TrySaveMailing Tracking started");
				tracking = ExecuteDbActionsWithinTransaction(tracking,
					(cn, t, l) =>
					{
						return PerformMailTrackingInsert(cn, t, String.Concat("Created by auth-ticket: ", rawAuthTicketData/*.GetRawAuthTicketData(false)*/)); //TODO: Signature?
					});
				partial = PartialCreateStatus.Saved;
				YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", "TrySaveMailing Tracking Created");
				return true;
			}
			catch (ResourceAlreadyExistsException)
			{
				/* fall through, the inserting process is behind the times */
			}
			catch (SqlException e)
			{
				if (e.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server"))
				{
					var capture = tracking;
					// failure during initial create... 
					string server = (string.IsNullOrEmpty(e.Server)) ? string.Empty : e.Server;
					YPMon.Warn("TMAIL_SAVEMAILING_INSERTRACKING_SQLERROR",
							   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3} SQLERROR: {4} SQLServer: {5}",
							   capture.Application, capture.Environment, capture.EmailTemplate, Enum.GetName(typeof(PartialCreateStatus), partial),
							   e.Message,server));

					_output.CreateAndSendMessage<DelayedMailingMessage>(AgentProtocol.CTMailDelayed, m =>
					{
						m.AuthTicket = rawAuthTicketData;					
						m.MailTrackingID = capture.ID;
						m.Application = capture.Application;
						m.Environment = capture.Environment;
						m.Template = capture.EmailTemplate;
						m.Recipients = recipients;
						m.PartialStatus = PartialCreateStatus.None;
					});
					YPMon.Info("TMAIL_SAVEMAILING",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   capture.Application, capture.Environment, capture.EmailTemplate, Enum.GetName(typeof(PartialCreateStatus), PartialCreateStatus.None)));
			
				}
			}
			return false;
		
		}

		bool TrySubmitMailing(MailTracking tracking, string rawAuthTicketData, Recipient[] recipients, ref PartialCreateStatus partial) 
		{
			Require.IsNotNull("tracking", tracking);
			Require.IsNotNull("recipients", recipients);
			Require.AtLeastOneParam("recipients", recipients);

			var app = tracking.Application;
			var env = tracking.Environment;
			var template = tracking.EmailTemplate;
			YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING","TrySubMailing started");
			var config = StrongMailConfigurationSection.Instance;
		
			using (var easApi = config.CreateTransactionalApiWebService())
			{
				try
				{
					var status = easApi.GetState(config.EasApiCredentials, template);
                    
					YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", "SendingTo StrongMail started: GetMailingId State");
             		if (status == ATTi.TMail.StrongMail.TransactionalApi.MailingStates.ACTIVE)
					{
						YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", "SendingTo StrongMail started: GetMailingId State ACTIVE");
						// Submit to StrongMail...
						long count = 0;
						var serialNo = easApi.Send(config.EasApiCredentials, template,
							recipients.ToStrongMailTokenStream(), out count);

						partial = PartialCreateStatus.Submitted;
						YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", string.Concat("SendingTo StrongMail submitted",serialNo));
						YPMon.Info("TMAIL_SAVEMAILING",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));

						return ReliableRecordStatus(ref tracking,
							rawAuthTicketData,
							recipients,
							partial,
							MailTrackingStatus.Submitted,
							String.Concat("Mailing submitted -- StrongMail serial number: ", serialNo)							
							);
					}
					else if (status == ATTi.TMail.StrongMail.TransactionalApi.MailingStates.PAUSED
						|| status == ATTi.TMail.StrongMail.TransactionalApi.MailingStates.PAUSEDOUTBOUND
						|| status == ATTi.TMail.StrongMail.TransactionalApi.MailingStates.PAUSING
						|| status == ATTi.TMail.StrongMail.TransactionalApi.MailingStates.PAUSINGOUTBOUND
                        )
					{
						partial = PartialCreateStatus.Paused;
						YPMon.Warn("MAILING_PAUSED", String.Concat("Mailing paused in StrongMail: ", app, "/", env, "/", template, "."));
						// Schedule for a retry... the mailing is paused. Probably need a backoff strategy here!
						// Consider putting traffic for paused mailings into another queue, one that doesn't get
						// polled continually. This will reduce churn on the mq.
						return ReliableRecordStatus(ref tracking,
							rawAuthTicketData,
							recipients,
							partial,
							MailTrackingStatus.Paused,
							String.Concat("Email template paused in StrongMail: ", status)
							);
						//return ReliableRecordStatus(ref tracking,
						//    rawAuthTicketData,
						//    recipients,
						//    partial,
						//    MailTrackingStatus.Paused,
						//    String.Concat("Email template paused in StrongMail: ", status)
						//    );
						//return ReliableRecordStatus(ref tracking,
						//    rawAuthTicketData,
						//    recipients,
						//    partial,
						//    MailTrackingStatus.Paused,
						//    String.Concat("Email template paused in StrongMail: ", status)
						//    );
					}
                    else if(status ==ATTi.TMail.StrongMail.TransactionalApi.MailingStates.CANCELLED
                        || status ==ATTi.TMail.StrongMail.TransactionalApi.MailingStates.COMPLETED
                        || status ==ATTi.TMail.StrongMail.TransactionalApi.MailingStates.COMPLETING)
                    {
                        partial = PartialCreateStatus.Dropped; //TODO right now dropping paused email
                        YPMon.Warn("MAILING_DROPPED", String.Concat("Mailing not valid in StrongMail, please check with strongmail administrator: ", app, "/", env, "/", template, "."));
                        return ReliableRecordStatus(ref tracking,
                            rawAuthTicketData,
                            recipients,
                            partial,
                            MailTrackingStatus.Dropped,
                            String.Concat("Mailing not valid in StrongMail: ", status)
                            );
                    }
					else
					{
						partial = PartialCreateStatus.Failed;
						YPMon.Info("TMAIL_AGENTLOGIC_SUBMITMAILING", string.Concat("SendingTo StrongMail failed", status));
						YPMon.Info("TMAIL_SAVEMAILING",
								   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
								   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
			
						return ReliableRecordStatus(ref tracking,
							rawAuthTicketData,
							recipients,
							partial,
							MailTrackingStatus.Failed,
							String.Concat("Email template not active in StrongMail: ", status)
							);
					}
				}
				catch (WebException e)
				{
                    //TODO: check what happen to strongmail webapi returns on bad mailing/bad tokens, then drop message

					// Probably failure to connect to SM.
					this.TraceData(TraceEventType.Error, e.FormatForLogging());
					var capturePartial = partial;
					YPMon.Info("TMAIL_SAVEMAILING",
							   string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
							   app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
				
					_output.CreateAndSendMessage<DelayedMailingMessage>(AgentProtocol.CTMailDelayed, m =>
					{
						m.AuthTicket =  rawAuthTicketData;
						m.MailTrackingID = tracking.ID;
						m.Application = tracking.Application;
						m.Environment = tracking.Environment;
						m.Template = tracking.EmailTemplate;
						m.Recipients = recipients;
						m.PartialStatus = capturePartial;
					});
					return false;
				}
				catch (SoapException ex)
				{
					if (ex.Message == "Connection to TMailing daemon failed: Connection refused")
					{
						// One of the critical services in SM is down.
						YPMon.Critical("STRONGMAIL_UNREACHABLE", "StrongMail is unreachable.", String.Empty);
						this.TraceData(TraceEventType.Error, ex.FormatForLogging());
						var capturePartial = partial;
						YPMon.Info("TMAIL_TRYSUBMITT_MAILING",
							  string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
					 		  app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));

						_output.CreateAndSendMessage<DelayedMailingMessage>(AgentProtocol.CTMailDelayed, m =>
						{
							m.AuthTicket = rawAuthTicketData; 
							m.MailTrackingID = tracking.ID;
							m.Application = tracking.Application;
							m.Environment = tracking.Environment;
							m.Template = tracking.EmailTemplate;
							m.Recipients = recipients;
							m.PartialStatus = capturePartial;
						});
						return false;
					}

					if(ex.Message.Contains("Mailing is not loaded"))
					{
						partial = PartialCreateStatus.Dropped;
						YPMon.Warn("TMAIL_SUBMIT_BADMAILING",string.Concat("Mailing is not valid ",ex.Message));
						this.TraceData(TraceEventType.Error, ex.FormatForLogging());
						var capturePartial = partial;
						YPMon.Info("TMAIL_TRYSUBMITT_MAILING",
							  string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
					 		  app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
						return false;
					}

					// SM doesn't give us good errors, this one can be one of dozens
					// that are only distinguishable by the exception text.
					partial = PartialCreateStatus.Failed;
					YPMon.Warn("STRONGMAIL_ERROR", ex.Message);
					return ReliableRecordStatus(ref tracking,
							rawAuthTicketData,
							recipients,
							partial,
							MailTrackingStatus.Failed,
							String.Concat("Failure submitting to StrongMail:", ex.Message)
							);
				}
				catch (InvariantContractException e)
				{
					partial = PartialCreateStatus.Failed;
					YPMon.Warn("STRONGMAIL_ERROR", e.Message);
					YPMon.Info("TMAIL_TRYSUBMITT_MAILING",
						  string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
						  app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
					return ReliableRecordStatus(ref tracking,
							rawAuthTicketData,
							recipients,
							partial,
							MailTrackingStatus.Failed,
							String.Concat("Bad data submitting to StrongMail:", e.Message)
							);
				}
				catch (Exception ex)
				{
					var err = String.Concat("Unexpected error while submitting a mailing: ", app, "/", env, "/", template);
					YPMon.Critical("AGENT_UNEXPECTED_FAILURE", err, ex.FormatForLogging());
					YPMon.Info("TMAIL_TRYSUBMITT_MAILING",
						  string.Format("App: {0}, Env: {1}, Template: {2}, States: {3}",
						  app, env, template, Enum.GetName(typeof(PartialCreateStatus), partial)));
					this.TraceData(TraceEventType.Error, err, ex.FormatForLogging());
					throw;
				}
			}
		}

        bool ReliableRecordStatus(ref MailTracking tracking,
           string rawAuthTicketData,
           Recipient[] recipients,
           PartialCreateStatus partial,
           MailTrackingStatus unrecordedStatus,
           string unrecordedNote
           ) 
        {
            return ReliableRecordStatus(ref tracking, rawAuthTicketData, recipients, partial, unrecordedStatus, unrecordedNote, true);
        }


		bool ReliableRecordStatus(ref MailTracking tracking,
			string rawAuthTicketData,
			Recipient[] recipients,
			PartialCreateStatus partial,
			MailTrackingStatus unrecordedStatus,
			string unrecordedNote,bool reSubmit
			)
		{
			try
			{
				tracking.Status = unrecordedStatus;
				tracking = ExecuteDbActionsWithinTransaction(tracking,
				(cn, t, l) =>
				{
					return PerformMailTrackingUpdate(cn, t, unrecordedNote);
				});
				return true;
			}
			catch (SqlException e)
			{
				
				if (e.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server"))
				{
					YPMon.Critical("SQLSERVER_UNREACHABLE", e.FormatForLogging(), null);
					var capture = tracking;
					string server = (string.IsNullOrEmpty(e.Server)) ? string.Empty : e.Server;
					YPMon.Info("TMAIL_RELIABLERECORDSTATUS_INSERTRACKING_SQLERROR",
							 string.Format("App: {0}, Env: {1}, Template: {2}, States: {3} SQLERROR: {4} SQLServer: {5}",
							 capture.Application, capture.Environment, capture.EmailTemplate, Enum.GetName(typeof(PartialCreateStatus), partial),
							 e.Message,server));

                    if (partial == PartialCreateStatus.Paused)
                    {
                        //_output.CreateAndSendMessage<PausedMailingMessage>(AgentProtocol.CTMailPaused, m =>
                        //{
                        //    m.AuthTicket = rawAuthTicketData;
                        //    m.MailTrackingID = capture.ID;
                        //    m.Application = capture.Application;
                        //    m.Environment = capture.Environment;
                        //    m.Template = capture.EmailTemplate;
                        //    m.Recipients = recipients;
                        //    m.PartialStatus = partial;
                        //    m.UnrecordedStatus = unrecordedStatus;
                        //    m.UnrecordedNote = unrecordedNote;
                        //});                    
                    }
                    if (partial != PartialCreateStatus.Dropped)
                    {
                        _output.CreateAndSendMessage<DelayedMailingMessage>(AgentProtocol.CTMailDelayed, m =>
                        {
                            m.AuthTicket = rawAuthTicketData;
                            m.MailTrackingID = capture.ID;
                            m.Application = capture.Application;
                            m.Environment = capture.Environment;
                            m.Template = capture.EmailTemplate;
                            m.Recipients = recipients;
                            m.PartialStatus = partial;
                            m.UnrecordedStatus = unrecordedStatus;
                            m.UnrecordedNote = unrecordedNote;
                        });
                    }
                
					YPMon.Info("TMAIL_RECORDMAILING_STATUS",
					  string.Format("SQLSERVER_UNREACHABLE App: {0}, Env: {1}, Template: {2}, States: {3}",
					  capture.Application, capture.Environment, capture.EmailTemplate, Enum.GetName(typeof(PartialCreateStatus), partial)));
				}
				return false;
			}
		}
	}
}
