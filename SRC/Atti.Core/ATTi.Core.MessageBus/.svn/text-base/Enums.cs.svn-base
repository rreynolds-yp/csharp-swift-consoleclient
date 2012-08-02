using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus
{	
	public enum MessageBusConnectionStatus
	{
		Unknown = 0,
		Open = 1,
		Shutdown = 2,
		Disposed = 4
	}

	[Flags]
	public enum MessageReturnAction
	{
		None = 0,
		Ack = 1,
		CommitTx = 2,
		Shutdown = 4
	}

	[Flags]
	internal enum ModelStatus
	{
		Unknown = 0,
		Open = 1,
		Starting = 2 | Open,
		Started = 4 | Open,
		Reopening = 8,
		Stopping = 16 | Open,
		Shutdown = 32,
		Disposed = 64,
	}
}
