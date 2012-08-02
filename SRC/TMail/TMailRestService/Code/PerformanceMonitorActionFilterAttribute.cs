namespace TMailRestService.Code
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Management;
	using System.Web.Mvc;

	public class PerformanceMonitorActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext) 
		{
			base.OnActionExecuted(filterContext);
			WebBaseEvent.Raise(new PerformanceMonitoringEvent(filterContext.ActionDescriptor.ActionName,
				filterContext.Controller.GetType(), 
				String.Format("TMail Action {0} has executed", filterContext.ActionDescriptor.ActionName),
				filterContext.Controller));
		}
	}
}
