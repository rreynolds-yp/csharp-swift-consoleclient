
namespace ATTi.Core.Trace.Adapters
{
	using System;
	using System.Diagnostics;

	internal class VerboseSourceAdapter : ITraceSourceAdapter
	{
		

		private TraceSource _traceSource;

		

		#region Constructors

		internal VerboseSourceAdapter(TraceSource source)
		{
			if (source == null) throw new ArgumentNullException("source");

			_traceSource = source;
		}

		internal VerboseSourceAdapter(string sourceName)
		{
			if (sourceName == null) throw new ArgumentNullException("sourceName");

			_traceSource = new TraceSource(sourceName);
		}

		#endregion Constructors

		

		TraceSource ITraceSourceAdapter.Source
		{
			get { return _traceSource; }
		}

		

		

		bool ITraceSourceAdapter.ShouldTrace(TraceEventType traceType)
		{
			return _traceSource.Switch.ShouldTrace(traceType);
		}

		void ITraceSourceAdapter.TraceData(TraceEventType eventType, int id, object data)
		{
			if (eventType <= TraceEventType.Verbose)
				_traceSource.TraceData(eventType, id, data);
		}

		void ITraceSourceAdapter.TraceData(TraceEventType eventType, int id, params object[] data)
		{
			if (eventType <= TraceEventType.Verbose)
				_traceSource.TraceData(eventType, id, data);
		}

		void ITraceSourceAdapter.TraceError(int id, string message)
		{
			_traceSource.TraceEvent(TraceEventType.Error, id, message);
		}

		void ITraceSourceAdapter.TraceError(int id, string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");

			_traceSource.TraceEvent(TraceEventType.Error, id, String.Format(format, args));
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id)
		{
			if (eventType <= TraceEventType.Verbose)
				_traceSource.TraceEvent(eventType, id);
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id, string message)
		{
			if (message == null) throw new ArgumentNullException("message");

			if (eventType <= TraceEventType.Verbose)
				_traceSource.TraceEvent(eventType, id, message);
		}

		void ITraceSourceAdapter.TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");

			if (eventType <= TraceEventType.Verbose)
				_traceSource.TraceEvent(eventType, id, format, args);
		}

		void ITraceSourceAdapter.TraceTransfer(int id, string message, Guid relatedActivityId)
		{
		}

		void ITraceSourceAdapter.TraceVerbose(int id, string message)
		{
			_traceSource.TraceEvent(TraceEventType.Verbose, id, message);
		}

		void ITraceSourceAdapter.TraceVerbose(int id, string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");

			_traceSource.TraceEvent(TraceEventType.Verbose, id, String.Format(format, args));
		}

		void ITraceSourceAdapter.TraceWarning(int id, string message)
		{
			_traceSource.TraceEvent(TraceEventType.Warning, id, message);
		}

		void ITraceSourceAdapter.TraceWarning(int id, string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");

			_traceSource.TraceEvent(TraceEventType.Warning, id, String.Format(format, args));
		}

		
	}
}