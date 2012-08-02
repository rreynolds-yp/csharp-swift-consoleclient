using System;

namespace ATTi.TMail.Service.Implementation.Agent
{
	public delegate void LineWriterDelegate(object sender, params object[] args);
	public delegate void ListenerEventDelegate(object sender, string topic);
	public delegate void LeaderUnresponsiveDelegate(object sender, int revision, Guid id);
	public delegate void LeadershipChanged(Guid leaderID, int revision, int leadershipPosition, bool needsFollowers);

	public class InboundMessageArgs<M> : EventArgs
		where M: MessageBase
	{
		public string Queue { get; set; }
		public string ReplyTo { get; set; }
		public string Topic { get; set; }
		public M Message { get; set; }
		public AgentRecord Agent { get; set; }
	}
	public delegate void InboundMessageDelegate<M>(object sender, InboundMessageArgs<M> args)
		where M: MessageBase;
	
	public class ReturnedMessageArgs : EventArgs
	{
		public string Queue { get; set; }
		public string ReplyTo { get; set; }
		public string Topic { get; set; }
		public MessageBase Message { get; set; }
	}
	public delegate void ReturnedMessageDelegate(object sender, ReturnedMessageArgs args);		
}
