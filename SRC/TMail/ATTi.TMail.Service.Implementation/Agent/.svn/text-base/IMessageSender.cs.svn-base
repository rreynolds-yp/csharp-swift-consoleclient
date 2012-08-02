using System;

namespace ATTi.TMail.Service.Implementation.Agent
{
	public interface IMessageSender
	{
		T CreateMessage<T>(Action<T> loader)
			where T: MessageBase;

		void SendMessage<T>(string exchange, string replyTo, T message)
			where T : MessageBase;

		void SendMessage<T>(string exchange, T message)
			where T : MessageBase;

		T CreateAndSendMessage<T>(string exchange, string replyTo, Action<T> loader)
			where T : MessageBase;

		T CreateAndSendMessage<T>(string exchange, Action<T> loader)
			where T : MessageBase;
	}
}
