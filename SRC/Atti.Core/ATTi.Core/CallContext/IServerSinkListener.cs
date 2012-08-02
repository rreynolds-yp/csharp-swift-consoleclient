using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace ATTi.Core.CallContext
{
	public interface IServerSinkListener
	{
		bool HandleServerMessage(IMessage msg, ITransportHeaders hdrs, LogicalCallContext lcc, out object handback);

		void HandleServerMessageResponse(IMessage msg, ITransportHeaders hdrs, LogicalCallContext lcc, object handback);
	}
}
