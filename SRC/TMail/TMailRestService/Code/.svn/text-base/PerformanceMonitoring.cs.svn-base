namespace TMailRestService.Code
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Management;
	using ATTi.Core.Contracts;

	public class PerformanceMonitoringEvent : WebManagementEvent
	{
		public string Action { get; set; }
		public Type Controller { get; set; }
		public const int ActionEventCode = WebEventCodes.WebExtendedBase + 1;

		public PerformanceMonitoringEvent(string action, Type controller, string msg, object source) 
			: base(msg,source, ActionEventCode)
		{
			Action = action;
			Controller = controller;
		}

		protected override void IncrementPerfCounters()
		{
			base.IncrementPerfCounters();
		}

		public override void FormatCustomEventDetails(WebEventFormatter formatter)
		{
			base.FormatCustomEventDetails(formatter);
			formatter.AppendLine(String.Concat("TMailController: ", Controller));
			formatter.AppendLine(String.Concat("TMailAction: ", Action));
		}

	}
}
