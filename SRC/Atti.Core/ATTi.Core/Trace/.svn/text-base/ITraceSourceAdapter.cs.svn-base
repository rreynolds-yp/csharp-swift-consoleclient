
namespace ATTi.Core.Trace
{
	using System;
	using System.Diagnostics;

	internal interface ITraceSourceAdapter
	{
		TraceSource Source
		{
			get;
		}

		bool ShouldTrace(TraceEventType eventType);

		void TraceData(TraceEventType eventType, int id, object data);

		void TraceData(TraceEventType eventType, int id, params object[] data);

		void TraceError(int id, string message);

		void TraceError(int id, string format, params object[] args);

		void TraceEvent(TraceEventType eventType, int id);

		void TraceEvent(TraceEventType eventType, int id, string message);

		void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);

		void TraceTransfer(int id, string message, Guid relatedActivityId);

		void TraceVerbose(int id, string message);

		void TraceVerbose(int id, string format, params object[] args);

		void TraceWarning(int id, string message);

		void TraceWarning(int id, string format, params object[] args);
	}
}