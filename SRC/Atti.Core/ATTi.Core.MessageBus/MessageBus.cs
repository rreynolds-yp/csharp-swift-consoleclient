using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using ATTi.Core.Contracts;
using ATTi.Core.MessageBus.Configuration;
using RabbitMQ.Client.Events;
using ATTi.Core.Trace;
using System.Diagnostics;
using System.IO;
using ATTi.Core.Factory;
using System.Threading;

namespace ATTi.Core.MessageBus
{
	public class MessageBus
	{
		public static IMessageBusConnection Connect(string connectionName)
		{
			Require.IsNotEmpty("connectionName", connectionName);
			
			return new MessageBusConnection(connectionName);
		}		
	}
}
