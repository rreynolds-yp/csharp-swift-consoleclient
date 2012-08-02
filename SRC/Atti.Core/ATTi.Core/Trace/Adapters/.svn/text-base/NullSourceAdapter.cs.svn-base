
namespace ATTi.Core.Trace.Adapters
{
	using System;
	using System.Diagnostics;

	internal class NullSourceAdapter : ITraceSourceAdapter
	{
		

		TraceSource _traceSource;

		

		#region Constructors

		internal NullSourceAdapter(TraceSource src)
		{
			_traceSource = src;
		}

		#endregion Constructors

		

		TraceSource ITraceSourceAdapter.Source
		{
			get { return _traceSource; }
		}

		

		

		bool ITraceSourceAdapter.ShouldTrace(TraceEventType traceType)
		{
			return false;
		}

		void ITraceSourceAdapter.TraceData(TraceEventType eventType, int id, object data)
		{
		}

		void ITraceSourceAdapter.TraceData(TraceEventType eventType, int id, params object[] data)
		{
		}

		void ITraceSourceAdapter.TraceError(int id, string message)
		{
		}

		void ITraceSourceAdapter.TraceError(int id, string format, params object[] args)
		{
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id)
		{
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id, string message)
		{
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
		}

		void ITraceSourceAdapter.TraceTransfer(int id, string message, Guid relatedActivityId)
		{
		}

		void ITraceSourceAdapter.TraceVerbose(int id, string message)
		{
		}

		void ITraceSourceAdapter.TraceVerbose(int id, string format, params object[] args)
		{
		}

		void ITraceSourceAdapter.TraceWarning(int id, string message)
		{
		}

		void ITraceSourceAdapter.TraceWarning(int id, string format, params object[] args)
		{
		}

		
	}
}