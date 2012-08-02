using ATTi.TMail.Model;
using System;

namespace ATTi.TMail.Service.Implementation.Agent.Messages
{
	public class MailingMessage : MessageBase
	{
		public MailingMessage()
			: base()
		{
			this.Kind = AgentMessageKind.Mailing;
		}
		public MailingMessage(int sequence)
			: base(sequence)
		{
			this.Kind = AgentMessageKind.Mailing;
		}
		public Guid MailTrackingID { get; set; }
		public string Application { get; set; }
		public string Environment { get; set; }
		public string Template { get; set; }
		public Recipient[] Recipients { get; set; }
	}
}
