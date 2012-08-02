using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus
{
	public interface IMessageBusConsumerEndpoint : IDisposable
	{
		IMessageBusConsumerEndpoint MatchMessagesWithProperties(Predicate<MessageProperties> predicate);
		IMessageBusConsumerEndpoint TransformMessageBody<T>(Func<MessageProperties, byte[], T> transformation);
		IMessageBusConsumerEndpoint HandleMessages<T>(Func<MessageProperties, MessageDeliveryInfo, T, MessageReturnAction> handler);
		IMessageBusConsumerEndpoint HandleRawMessages(Func<MessageProperties, MessageDeliveryInfo, byte[], MessageReturnAction> handler);
		IMessageBusConsumerEndpoint UseTransactions();

		void Startup();
		void Shutdown();
		void WaitForShutdown();
		bool WaitForShutdown(TimeSpan timeout);
	}
}
