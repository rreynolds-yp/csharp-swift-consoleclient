
using ATTi.Core;
using System;
using System.Threading;
namespace TMailRestService
{
	public enum ConnectivityState
	{
		Available = 1,
		Broken = 2,
	}

	public enum SubmisstionMode 
	{ 
		QueuedInMessageBus = 1,
		DirectSubmission =2,
	}

	public static class Static
	{
		public static readonly string DatabaseConnectivity = "DatabaseConnectivity";
		public static readonly string MessageBusConnectivity = "MessageBusConnectivity";
		public static readonly string MimeType_ApplicationJSON = "application/json";
		
		public static readonly object NoErrorData = new { Error = "No error data" };

		public static readonly object Error_400 = new { Error = "Unable to process PostData; it is either not JSON or has an unrecognized structure." };
		public static readonly object Error_404_Mailing = new { Error = "Not found. If the mailing was recently accepted please retry in a few minutes." };
		public static readonly object Error_406 = new { Error = "Client must accept JSON content (mime-type: application/json)." };
		public static readonly object Error_500 = new { Error = "Server error processing your request." };
		public static readonly object Error_501 = new { Error = "Not implemented." };
		public static readonly object Error_503_BrokerDown = new { Error = "Service unavailable; message bus broker unreachable." };
		public static readonly object Error_503_SqlServerDown = new { Error = "Service unavailable; SQL Server unreachable. Please retry in a few minutes." };


		static DateTime __databaseConnectivityRetryTime;
		static DateTime __messagebusConnectivityRetryTime;
		static Status<ConnectivityState> __databaseConnectivity = new Status<ConnectivityState>(ConnectivityState.Available);
		static Status<ConnectivityState> __messagebusConnectivity = new Status<ConnectivityState>(ConnectivityState.Available);
		static Status<SubmisstionMode> __messageSubmissionMode = new Status<SubmisstionMode>(SubmisstionMode.QueuedInMessageBus);

		public static bool IsDatabaseConnectivityAvailable
		{
			get
			{
				Thread.MemoryBarrier();
				var result = __databaseConnectivity.CurrentState == ConnectivityState.Available
					|| __databaseConnectivityRetryTime < DateTime.Now;
				Thread.MemoryBarrier();
				return result;
			}
		}

		public static void SetDatabaseConnectivity(ConnectivityState state)
		{
			__databaseConnectivity.SetState(state);
			if (state == ConnectivityState.Broken)
			{
				Thread.MemoryBarrier();
				__databaseConnectivityRetryTime = DateTime.Now.Add(TimeSpan.FromMinutes(1));
				Thread.MemoryBarrier();
			}
		}

		public static bool IsMessagebusConnectivityAvailable
		{
			get
			{
				Thread.MemoryBarrier();
				var result = __messagebusConnectivity.CurrentState == ConnectivityState.Available
					|| __messagebusConnectivityRetryTime < DateTime.Now;
				Thread.MemoryBarrier();
				return result;
			}
		}

		public static void SetMessagebusConnectivity(ConnectivityState state)
		{
			__messagebusConnectivity.SetState(state);
			if (state == ConnectivityState.Broken)
			{
				Thread.MemoryBarrier();
				__messagebusConnectivityRetryTime = DateTime.Now.Add(TimeSpan.FromMinutes(5));
				Thread.MemoryBarrier();
			}
		}

		public static bool IsDirectSubmission
		{
			get
			{
				Thread.MemoryBarrier();
				var result = (__messageSubmissionMode.CurrentState == SubmisstionMode.DirectSubmission); 								
				Thread.MemoryBarrier();
				return result;
			}
		}

		public static void SetMessagebusConnectivity(SubmisstionMode mode)
		{
			__messageSubmissionMode.SetState(mode);
		}

	}
}
