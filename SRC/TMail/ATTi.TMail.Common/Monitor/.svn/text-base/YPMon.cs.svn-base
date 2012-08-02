namespace ATTi.TMail.Common
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using Org.Lwes;
	using Org.Lwes.Emitter;	
	using ATTi.Core.Trace;

	/// <summary>
	/// Helper class for emitting YP:Mon::XXXX events to LWES.
	/// For more info see: https://subversion.flight.yellowpages.com/svnweb/infrastructure/view/lwes/esf/monitoring.esf
	/// </summary>
	public static class YPMon 
	{
		static readonly string CriticalEventName = "YP::Mon::Critical";
		static readonly string WarnEventName = "YP::Mon::Warn";
		static readonly string InfoEventName = "YP::Mon::Info";
		static readonly string DebugEventName = "YP::Mon::Debug";


		static readonly string Attribute_SiteID = "SiteID";
		static readonly string Attribute_version = "version";
		static readonly string Attribute_eventID = "eid";
		static readonly string Attribute_applicationID = "aid";
		static readonly string Attribute_applicationVersionID = "avid";
		static readonly string Attribute_inReq = "inReq";
		static readonly string Attribute_outReq = "outReq";
		static readonly string Attribute_msg = "msg";

		static readonly string Attribute_stack = "trace";

		static readonly ushort TMailSiteID = 0; // TODO: Get a site ID!!
		static readonly string TMailApplicationID = "tmail";
		static readonly string CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		static readonly string TMailApplicationVersionID = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
		
		static IEventEmitter __emitter;


		public static void Critical(string eventID, string message, string stacktrace)
		{
            Emit(CriticalEventName, eventID, message, stacktrace);
			Traceable.TraceEvent(typeof(YPMon), System.Diagnostics.TraceEventType.Error, string.Format("{0} StackTrace: {1}", message, stacktrace));

		}
		public static void Warn(string eventID, string message)
		{
			Emit(WarnEventName, eventID, message, null);
            Traceable.TraceEvent(typeof(YPMon), System.Diagnostics.TraceEventType.Warning, message);
		}
		public static void Info(string eventID, string message)
		{
		    Emit(InfoEventName, eventID, message, null);
			Traceable.TraceEvent(typeof(YPMon), System.Diagnostics.TraceEventType.Information,message);
		}
		public static void Debug<T>(string eventID, string message)
		{
			Traceable.TraceVerbose(typeof(T), eventID, message);
		}

		public static void DebugIf<T>(bool condition, string eventID, string message)
		{
			if(condition)
				Traceable.TraceVerbose(typeof(T), eventID, message);
		}

		private static void Emit(string eventName, string eventID, string message, string stacktrace)
		{
			Event ev = null;
			try
			{
				ev = new Event(eventName);
				ev.SetValue(Attribute_SiteID, TMailSiteID);
				ev.SetValue(Attribute_version, CurrentVersion);
				ev.SetValue(Attribute_eventID, eventID);
				ev.SetValue(Attribute_applicationID, TMailApplicationID);
				ev.SetValue(Attribute_applicationVersionID, TMailApplicationVersionID);
				ev.SetValue(Attribute_msg, message);
				if (!String.IsNullOrEmpty(stacktrace)) ev.SetValue(Attribute_stack, stacktrace);
				var emitter = ATTi.Core.Util.NonBlockingLazyInitializeVolatile(ref __emitter, () => { return EventEmitter.CreateDefault(); });
				emitter.Emit(ev);
			}
			catch (Exception ex)
			{

				Traceable.TraceEvent(typeof(YPMon), System.Diagnostics.TraceEventType.Error, String.Concat(@"YPMon unable Emitt Event with exception",ex.Message));
				if(ev!=null)
				Traceable.TraceEvent(typeof(YPMon), System.Diagnostics.TraceEventType.Information, String.Concat("YPMon failed to emit event", ev.ToString()));
			}
        }
		
    }
    
}
