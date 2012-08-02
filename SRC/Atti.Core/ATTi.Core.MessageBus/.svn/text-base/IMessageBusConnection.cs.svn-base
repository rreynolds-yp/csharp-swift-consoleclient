using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus
{
	public interface IMessageBusConnection : IDisposable
	{
		MessageBusConnectionStatus Status { get; }
		IMessageBusConsumerEndpoint OpenQueueAsConsumer(string queueName, string routingKey);
		IMessageBusProducerEndpoint OpenExchangeAsProducer(string exchangeName, string routingKey);
		void CallbackOnShutdown(Action<IMessageBusConnection, object> shutdownHandler, object handback);
	}
}
