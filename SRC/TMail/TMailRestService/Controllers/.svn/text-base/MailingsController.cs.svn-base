using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using ATTi.Core;
using ATTi.Core.Factory;
using ATTi.Core.Trace;
using ATTi.TMail.Common;
using RabbitMQ.Client.Exceptions;
using TMailRestService.Models;
using TMailService.Models;

namespace TMailRestService.Controllers
{
	//using ATTI.Core.Logging;
	
	public class MailingsController : BaseController
	{
		[AcceptVerbs(HttpVerbs.Post)]
		[RequiresSso]
		public ActionResult PostMailing(string app, string env)
		{
			this.TraceData(TraceEventType.Information,
                String.Format("MailingController PostMailing Received Request {0} with app:{1} and env:{2}",
                Request.ToString(), app,env));
            
			// Ensure the client can handle our JSON result...
			if (!Request.CanAcceptType(Static.MimeType_ApplicationJSON))
			{
				this.TraceData(TraceEventType.Warning,
                    "MailingController PostMailing Received Request Accepting Json format is returning 406 ", 
                    Request.CanAcceptType(Static.MimeType_ApplicationJSON));
				return this.Result_406_NotAcceptable(Static.Error_406, null);
			}

			NewMailingInfo info;
			ResultEncoding encoding;
			try
			{
				this.TraceData(TraceEventType.Information,"MailinController PostMailing BeforeDeserialising Json Body");
				if (!this.TryDeserializePostData(out encoding, out info))
				{
					this.TraceData(TraceEventType.Warning,"MailinController PostMailing Unable to deserialize Json Body, return 404");
					return this.Result_400_BadRequest(Static.Error_400, null);
				}
			}
			catch(Exception ex)
			{
				this.TraceData(TraceEventType.Warning,"MailinController PostMailing unexpected managed error return 400 with exception ",ex.Message);
				return this.Result_400_BadRequest(Static.Error_400, null);
			}


			if (!Static.IsMessagebusConnectivityAvailable)
				return this.Result_503_ServiceUnavailable(Static.Error_503_BrokerDown, null);

			//if (!Static.IsMessagebusConnectivityAvailable && !Static.IsDirectSubmission)
			//    return this.Result_503_ServiceUnavailable(Static.Error_503_BrokerDown, null);

			try
			{
				using (var svc = Factory<ITMailService>.CreateInstance())
				{
					var id = Guid.NewGuid();
					this.TraceData(TraceEventType.Information,
                        "MailinController PostMailing Before CreateMailing with id {0}, app {1},env{2},info.EmailTemplate{3},info.Receipients{4}",
					              id, app, env, info.EmailTemplate.ToString(), info.Recipients.ToString());
					svc.CreateMailing(id, app, env, info.EmailTemplate, info.Recipients);
					
					Static.SetMessagebusConnectivity(ConnectivityState.Available);

					var mailingRoute = RouteTable.Routes.GetVirtualPath(null, new RouteValueDictionary 
								{ 
									{ "app", app }, 
									{ "env", env },
									{ "id", id }
								}).VirtualPath;
					this.TraceData(TraceEventType.Information,"MailTracking created for new mailing: ", mailingRoute);
					
					this.Response.StatusCode = (int)HttpStatusCode.Accepted;
					this.Response.RedirectLocation = mailingRoute;

					return this.Json(new
						{
							MailingID = id,
							Location = mailingRoute
						});
				}
			}
			catch (BrokerUnreachableException ex)
			{
				this.TraceData(TraceEventType.Information,
                    "MailingController PostMailing RabbitMQ is down; unable to accept mailing: ", 
                     Request.Url.ToString());
				this.TraceData(TraceEventType.Error,
					String.Concat("RabbitMQ is down; unable to accept mailing: ", Request.Url.ToString()),
					ex.FormatForLogging());
				Static.SetMessagebusConnectivity(ConnectivityState.Broken);
				this.TraceData(TraceEventType.Information,
                    "MailingController PostMailing RabbitMQ is down; unable to accept mailing: Returning 503 with ex ", 
                    ex.Message);
				return this.Result_503_ServiceUnavailable(Static.Error_503_BrokerDown, ex);
			}
			catch (ResourceNotFoundException ex) 
			{
				this.TraceData(TraceEventType.Warning,"MailingController PostMailing Resource not exists: ", 
				             String.Concat("Resource not found for route: ", Request.Url.ToString()), ex.Message);

				this.TraceData(TraceEventType.Warning, String.Concat("Resource not found for route: ", Request.Url.ToString()), ex.Message);
				return this.Result_404_NotFound(new { Error = ex.Message }, ex);
			}
			catch (ResourceAlreadyExistsException ex)
			{			
				this.TraceData(TraceEventType.Information,
                    "MailingController PostMailing Resource already exists: {0} with exp {1}", Request.Url.ToString(), ex.Message);
				
				this.TraceData(TraceEventType.Error, String.Concat("Resource already exists: ", Request.Url.ToString()), ex.Message);
				return this.Result_409_Conflict(new { Error = ex.Message }, ex);
			}
			catch (Exception ex)
			{
				this.TraceData(TraceEventType.Information,"MailingController PostMailing {0}",String.Concat("Unexpected exception occurred while processing route: ", Request.Url.ToString()),
				             ex.Message);

				this.TraceData(TraceEventType.Error,
					String.Concat("Unexpected exception occurred while processing route: ", Request.Url.ToString()),
					ex.FormatForLogging());
				return this.Result_500_InternalServerError(Static.Error_500, ex);
			}
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[RequiresSso]
		public ActionResult GetMailing(string app, string env, Guid id)
		{
			this.TraceData(TraceEventType.Information,"MailingController GetMailing Received Request {0} GetMailing with app:{0} and env:{1} Guid: {2}",Request.ToString(), app,env,id);

			// Ensure the client can handle our JSON result...
			if (!Request.CanAcceptType(Static.MimeType_ApplicationJSON))
			{
				this.TraceData(TraceEventType.Information,"MailingController GetMailing Received Request Accepting Json format is {0},return 406", 
				             Request.CanAcceptType(Static.MimeType_ApplicationJSON));

				return this.Result_406_NotAcceptable(Static.Error_406, null);
			}
			if (!Static.IsDatabaseConnectivityAvailable)
			{
				this.TraceData(TraceEventType.Information,"MailingController GetMailing Static.IsDatabaseConnectivityAvailable, returning 503");

				return this.Result_503_ServiceUnavailable(Static.Error_503_SqlServerDown, null);
			}

			try
			{
				using (var svc = Factory<ITMailService>.CreateInstance())
				{
					this.TraceData(TraceEventType.Information,"MailinController GetMailing Before GetMailingStatus with id {0}, app {1},env{2}", id, app, env);
					
					var mailing = svc.GetMailingStatus(app, env, id);
					if (mailing == null)
					{
						this.TraceData(TraceEventType.Information,"MailinController GetMailing After GetMailingStatus with id {0}, app {1},env{2} is NOTFOUND return 404", id, app, env);
						return this.Result_404_NotFound(Static.Error_404_Mailing, null);
					
					}

					//Static.SetDatabaseConnectivity(ConnectivityState.Broken);

					var mailingRoute = RouteTable.Routes.GetVirtualPath(null, new RouteValueDictionary 
								{ 
									{ "app", app }, 
									{ "env", env },
									{ "id", id }
								}).VirtualPath;
					
					var output = new MailingStatus
					{
						ID = mailing.ID,
						Application = app,
						Environment = env,
						EmailTemplate = mailing.EmailTemplate,
						Status = mailing.Status.ToString(),
						DateCreated = mailing.DateCreated,
						DateUpdated = mailing.DateUpdated,
						Location = mailingRoute,
						Notes = (from n in mailing.Notes
										 select new MailStatusNote
										 {
											 Status = n.Status.ToString(),
											 Note = n.StatusNote,
											 DateCreated = n.DateCreated
										 }).ToArray()
					};
					this.TraceData(TraceEventType.Information,"MailinController GetMailing Returning Json Result with id {0}, app {1},env{2}", id, app, env);
					return this.Json(output, JsonRequestBehavior.AllowGet);
				}
			}			
			catch (SqlException ex)
			{
				this.TraceData(TraceEventType.Information,"MailinController GetMailing SQL Server connection is down; unable to retrieve mailing with SQL exception {0}", ex.Message);
				
				this.TraceData(TraceEventType.Error,
					String.Concat("SQL Server connection is down; unable to retrieve mailing: ", Request.Url.ToString()),
					ex.FormatForLogging());
				Static.SetDatabaseConnectivity(ConnectivityState.Broken);
				this.TraceData(TraceEventType.Information,"MailinController GetMailing SQL Server connection is down; unable to retrieve mailing with SQL exception {0} return 503", ex.Message);
				return this.Result_503_ServiceUnavailable(Static.Error_503_SqlServerDown, ex);
			}
			catch (Exception e)
			{
				this.TraceData(TraceEventType.Information,"MailinController GetMailing {0}", 
				             String.Concat("Unexpected exception occurred while processing route: ", Request.Url.ToString(), e.Message));
				
				this.TraceData(TraceEventType.Error,
					String.Concat("Unexpected exception occurred while processing route: ", Request.Url.ToString()),
					e.FormatForLogging());
				//YPMon.Critical("TMailRestService", 
				return this.Result_500_InternalServerError(Static.Error_500, e);
			}
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[RequiresSso]
		public ActionResult GetMailings(string app, string env)
		{
			return this.Result_501_NotImplemented(Static.Error_501, null);
		}
	}
}
