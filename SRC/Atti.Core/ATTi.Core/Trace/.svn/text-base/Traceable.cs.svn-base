using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;

using ATTi.Core.Trace.Adapters;

namespace ATTi.Core.Trace
{

	/// <summary>
	/// Diagnostics utility.
	/// </summary>
	public static class Traceable
	{
		/// <summary>
		/// Trace ID used when none was given.
		/// </summary>
		public static int DefaultTraceEventID = 0;

		static Guid __traceProcessGuid = Guid.NewGuid();
		static readonly String __traceEnvironment;
		static readonly String __traceComponent;
		static readonly String __traceApplication;
		static readonly String __traceDefaultSource;
		static readonly int __traceOsProcess = -1;
		static readonly Object __lock = new Object();
		static readonly Dictionary<string, ITraceSourceAdapter> __filters = new Dictionary<string, ITraceSourceAdapter>();

		#region Constructors

		static Traceable()
		{
			TraceConfigurationSection diag = TraceConfigurationSection.Current;

			__traceEnvironment = diag.Environment;
			__traceComponent = diag.Component;
			__traceDefaultSource = diag.DefaultTraceSource;
			__traceApplication = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
			try
			{
				__traceOsProcess = Process.GetCurrentProcess().Id;
			}
			catch (SecurityException)
			{ // Not run with fulltrust, can't get the process Id. 
			}
		}

		#endregion Constructors

		//This is kinda not nice...
		public static string DefaultTraceSource() 
		{
			return __traceDefaultSource;
		}

		#region TraceData overloaded

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">the source's type T</typeparam>
		/// <param name="source">the source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="data">the trace data</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, object data)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceData(eventType, DefaultTraceEventID, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">the source's type T</typeparam>
		/// <param name="source">the source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="dataGenerator">A delegate that produces an object array containing the trace data.</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, Func<object[]> dataGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(eventType))
			{
				TraceAdapter<T>.Wrapper.TraceData(eventType, DefaultTraceEventID, dataGenerator());
			}
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">the source's type T</typeparam>
		/// <param name="source">the source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">the trace data</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, int id, object data)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceData(eventType, id, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="data">An object array containing the trace data.</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, params object[] data)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceData(eventType, DefaultTraceEventID, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An object array containing the trace data.</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, int id, params object[] data)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceData(eventType, id, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="dataGenerator">A delegate that produces an object array containing the trace data.</param>
		public static void TraceData<T>(this T source, TraceEventType eventType, int id, Func<object[]> dataGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(eventType))
			{
				TraceAdapter<T>.Wrapper.TraceData(eventType, id, dataGenerator());
			}
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="data">The trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, object data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceData(eventType, DefaultTraceEventID, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="dataGenerator">A delegate that produces an object array containing the trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, Func<object[]> dataGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, DefaultTraceEventID, dataGenerator());
			}
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">The trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, object data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceData(eventType, id, data);
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="data">An object array containing the trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, params object[] data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceData(eventType, DefaultTraceEventID, data);			
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An object array containing the trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, params object[] data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceData(eventType, id, data);			
		}

		/// <summary>
		/// Writes trace data to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="dataGenerator">A delegate that produces an object array containing the trace data.</param>
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, Func<object[]> dataGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, id, dataGenerator());
			}
		}

		#endregion

		#region TraceError overloaded

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="message">The error message to write.</param>
		public static void TraceError<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The error message to write.</param>
		public static void TraceError<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, id, message);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceError<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceError<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, id, format, args);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the error message to trace.</param>
		public static void TraceError<T>(this T source, int id, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Error))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="messageGenerator">A delegate that generates the error message to trace.</param>
		public static void TraceError<T>(this T source, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Error))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Error, DefaultTraceEventID, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="message">The error message to write.</param>
		public static void TraceError(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Error, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The error message to write.</param>
		public static void TraceError(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Error, id, message);
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceError(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Error, DefaultTraceEventID, format, args);			
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceError(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Error, id, format, args);			
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the error message to trace.</param>
		public static void TraceError(Type sourceType, int id, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an error message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="messageGenerator">A delegate that generates the error message to trace.</param>
		public static void TraceError(Type sourceType, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, DefaultTraceEventID, messageGenerator());
			}
		}

		#endregion

		#region TraceEvent overloaded
		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(eventType, id);
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="message">The trace message to write.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(eventType, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(eventType, id, message);
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(eventType, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(eventType, id, format, args);
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="messageGenerator">A delegate that generates a message to trace.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(eventType))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(eventType, DefaultTraceEventID, messageGenerator());
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates a message to trace.</param>
		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(eventType))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(eventType, id, messageGenerator());
			}
		}


		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id);
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="message">The trace message to write.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, DefaultTraceEventID, message);
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id, message);
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, DefaultTraceEventID, format, args);
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id, format, args);
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates a message to trace.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes a trace event message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values that specifies
		/// the event type of the trace data.</param>
		/// <param name="messageGenerator">A delegate that generates a message to trace.</param>
		public static void TraceEvent(Type sourceType, TraceEventType eventType, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, DefaultTraceEventID, messageGenerator());
			}
		}

		#endregion

		#region TraceInformation overloaded

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="message">The informational message to write.</param>
		public static void TraceInformation<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The informational message to write.</param>
		public static void TraceInformation<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, id, message);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceInformation<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceInformation<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, id, format, args);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the informational message to trace.</param>
		public static void TraceInformation<T>(this T source, int id, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Information))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="messageGenerator">A delegate that generates the informational message to trace.</param>
		public static void TraceInformation<T>(this T source, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Information))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Information, DefaultTraceEventID, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="message">The informational message to write.</param>
		public static void TraceInformation(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Information, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The informational message to write.</param>
		public static void TraceInformation(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Information, id, message);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceInformation(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Information, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceInformation(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Information, id, format, args);
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the informational message to trace.</param>
		public static void TraceInformation(Type sourceType, int id, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Information))
			{
				f.TraceEvent(TraceEventType.Information, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an informational message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="messageGenerator">A delegate that generates the informational message to trace.</param>
		public static void TraceInformation(Type sourceType, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Information))
			{
				f.TraceEvent(TraceEventType.Information, DefaultTraceEventID, messageGenerator());
			}
		}

		#endregion

		#region TraceTransfer overloaded

		/// <summary>
		/// Writes a trace transfer message to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="message">The trace message to write.</param>
		/// <param name="relatedActivityId">A Guid structure that identifies the related activity.</param>
		public static void TraceTransfer<T>(this T source, string message, Guid relatedActivityId)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceTransfer(DefaultTraceEventID, message, relatedActivityId);
		}

		/// <summary>
		/// Writes a trace transfer message to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		/// <param name="relatedActivityId">A Guid structure that identifies the related activity.</param>
		public static void TraceTransfer<T>(this T source, int id, string message, Guid relatedActivityId)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceTransfer(id, message, relatedActivityId);
		}

		/// <summary>
		/// Writes a trace transfer message to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type</param>
		/// <param name="message">The trace message to write.</param>
		/// <param name="relatedActivityId">A Guid structure that identifies the related activity.</param>
		public static void TraceTransfer(Type sourceType, string message, Guid relatedActivityId)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Transfer))
			{
				f.TraceTransfer(DefaultTraceEventID, message, relatedActivityId);
			}
		}

		/// <summary>
		/// Writes a trace transfer message to the trace listeners in the Listeners collection 
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		/// <param name="relatedActivityId">A Guid structure that identifies the related activity.</param>
		public static void TraceTransfer(Type sourceType, int id, string message, Guid relatedActivityId)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Transfer))
			{
				f.TraceTransfer(id, message, relatedActivityId);
			}
		}

		#endregion

		#region TraceVerbose overloaded

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="message">The verbose message to write.</param>
		public static void TraceVerbose<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The verbose message to write.</param>
		public static void TraceVerbose<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, id, message);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceVerbose<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceVerbose<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, id, format, args);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the verbose message to trace.</param>
		public static void TraceVerbose<T>(this T source, int id, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Verbose))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="messageGenerator">A delegate that generates the verbose message to trace.</param>
		public static void TraceVerbose<T>(this T source, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Verbose))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="message">The verbose message to write.</param>
		public static void TraceVerbose(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The verbose message to write.</param>
		public static void TraceVerbose(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Verbose, id, message);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceVerbose(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceVerbose(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Verbose, id, format, args);
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the verbose message to trace.</param>
		public static void TraceVerbose(Type sourceType, int id, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an verbose message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="messageGenerator">A delegate that generates the verbose message to trace.</param>
		public static void TraceVerbose(Type sourceType, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, messageGenerator());
			}
		}

		#endregion

		#region TraceWarning overloaded

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="message">The warning message to write.</param>
		public static void TraceWarning<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The warning message to write.</param>
		public static void TraceWarning<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, id, message);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceWarning<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceWarning<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, id, format, args);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the warning message to trace.</param>
		public static void TraceWarning<T>(this T source, int id, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Warning))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">The source's type T</typeparam>
		/// <param name="source">The source instance initiating the trace</param>
		/// <param name="messageGenerator">A delegate that generates the warning message to trace.</param>
		public static void TraceWarning<T>(this T source, Func<string> messageGenerator)
			where T : ITraceable
		{
			ITraceSourceAdapter w = TraceAdapter<T>.Wrapper;
			if (w.ShouldTrace(TraceEventType.Warning))
			{
				TraceAdapter<T>.Wrapper.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="message">The warning message to write.</param>
		public static void TraceWarning(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Warning, DefaultTraceEventID, message);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The warning message to write.</param>
		public static void TraceWarning(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Warning, id, message);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceWarning(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Warning, DefaultTraceEventID, format, args);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public static void TraceWarning(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			AcquireSourceWrapper(sourceType).TraceEvent(TraceEventType.Warning, id, format, args);
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="messageGenerator">A delegate that generates the warning message to trace.</param>
		public static void TraceWarning(Type sourceType, int id, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, id, messageGenerator());
			}
		}

		/// <summary>
		/// Writes an warning message to the trace listeners in the Listeners collection
		/// on behalf of the <paramref name="sourceType"/>.
		/// </summary>
		/// <param name="sourceType">The source's type.</param>
		/// <param name="messageGenerator">A delegate that generates the warning message to trace.</param>
		public static void TraceWarning(Type sourceType, Func<string> messageGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceAdapter f = AcquireSourceWrapper(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, messageGenerator());
			}
		}

		#endregion

		internal static ITraceSourceAdapter AcquireSourceWrapper(Type t)
		{
			if (t == null) throw new ArgumentNullException("t");

			ITraceSourceAdapter result;
			string key = t.Namespace.InternIt();
			
			lock (__lock)
			{
				if (!__filters.TryGetValue(key, out result))
				{
					Stack<string> keys = new Stack<string>();
					TraceSource src = new TraceSource(key);
					keys.Push(key);
					while (src.Switch.DisplayName == key)
					{
						int i = key.LastIndexOf('.');
						if (i <= 0) break;
						key = key.Substring(0, i).InternIt();
						if (__filters.TryGetValue(key, out result))
						{
							src = result.Source;
							break;
						}
						keys.Push(key);
						src = new TraceSource(key);
					}
					switch (src.Switch.Level)
					{
						case SourceLevels.ActivityTracing:
						case SourceLevels.All:
							result = new AllSourceAdapter(src);
							break;
						case SourceLevels.Critical:
							result = new CriticalSourceAdapter(src);
							break;
						case SourceLevels.Error:
							result = new ErrorSourceAdapter(src);
							break;
						case SourceLevels.Information:
							result = new InformationSourceAdapter(src);
							break;
						case SourceLevels.Verbose:
							result = new VerboseSourceAdapter(src);
							break;
						case SourceLevels.Warning:
							result = new WarningSourceAdapter(src);
							break;
						case SourceLevels.Off:
						default:
							result = new NullSourceAdapter(src);
							break;
					}
					foreach (string kk in keys)
					{
						__filters.Add(kk, result);
					}
				}
			}
			return result;
		}
		internal static class TraceAdapter<T>
			where T : ITraceable
		{

			private static ITraceSourceAdapter __wrapper;

			#region Constructors

			static TraceAdapter()
			{
				__wrapper = Traceable.AcquireSourceWrapper(typeof(T));
			}

			#endregion Constructors

			internal static ITraceSourceAdapter Wrapper
			{
				get { return __wrapper; }
			}
		}
	}
}