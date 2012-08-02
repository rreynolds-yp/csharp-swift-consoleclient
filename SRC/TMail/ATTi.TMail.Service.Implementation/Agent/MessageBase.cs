using System;
using System.Threading;

namespace ATTi.TMail.Service.Implementation.Agent
{
	public class MessageBase
	{
		public MessageBase()
		{
		}
		public MessageBase(int sequence) 
		{
			this.Sequence = sequence;
		}
		public AgentMessageKind Kind { get; protected set; }		
		public Guid AgentID { get; set; }
		public int Sequence { get; set; }
		public string SenderIPAddress { get; set; }
		public string AuthTicket { get; set; }
	}

	public class ResponseMessageBase : MessageBase
	{
		public ResponseMessageBase()
			: base()
		{
		}
		protected ResponseMessageBase(int sequence)
			: base(sequence)
		{
		}
		public AgentMessageKind OriginKind { get; set; }
		public Guid OriginID { get; set; }
		public int OriginSequence { get; set; }
	}
}
