using ATTi.TMail.Model;
using System;

namespace ATTi.TMail.Service.Implementation.Agent.Messages
{
	public class DelayedMailingMessage : MessageBase
	{
		public DelayedMailingMessage()
			: base()
		{
			this.Kind = AgentMessageKind.MailingDelayed;
		}
		public DelayedMailingMessage(int sequence)
			: base(sequence)
		{
			this.Kind = AgentMessageKind.MailingDelayed;
		}
		public Guid MailTrackingID { get; set; }
		public string Application { get; set; }
		public string Environment { get; set; }
		public string Template { get; set; }
		public Recipient[] Recipients { get; set; }
		public PartialCreateStatus PartialStatus { get; set; }
		public MailTrackingStatus UnrecordedStatus { get; set; }
		public string UnrecordedNote { get; set; }
	}
}
