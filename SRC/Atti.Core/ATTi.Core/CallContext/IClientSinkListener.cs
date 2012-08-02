using System.Runtime.Remoting.Messaging;

namespace ATTi.Core.CallContext
{
	public interface IClientSinkListener
	{
		/// <summary>
		/// Handles a message in the client channel sink.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="lcc">The logical call context associated with the message.</param>
		/// <param name="handback">An object to be handed back when handling the message reply.</param>
		/// <returns><b>true</b> if the message reply should also be handled; otherwise <b>false</b>.</returns>
		bool HandleClientMessage(IMessage msg, LogicalCallContext lcc, out object handback);
		/// <summary>
		/// Handles a message reply.
		/// </summary>
		/// <param name="msg">The message reply.</param>
		/// <param name="lcc">The logical call context associated with the message.</param>
		/// <param name="handback">An object given while handling of the client message.</param>
		void HandleClientMessageReply(IMessage msg, LogicalCallContext lcc, object handback);
	}
}
