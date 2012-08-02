using System;
using System.Threading;
using System.Web.Mvc;
using ATTi.Core;
using ATTi.Core.Trace;

namespace TMailRestService
{
	public class BaseController : Controller, ITraceable
	{
		protected override void OnException(ExceptionContext filterContext)
		{
			this.TraceData(System.Diagnostics.TraceEventType.Error, 
				String.Concat("Unhandled exception occurred while processing route: ", filterContext.HttpContext.Request.Url.ToString()),
				filterContext.Exception.FormatForLogging());
		}
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
			Thread.CurrentPrincipal = null;
		}
	}
}
