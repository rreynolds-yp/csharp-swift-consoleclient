using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus
{
	public class MessageProperties
	{
		public string AppId { get; set; }
		public string ClusterId { get; set; }
		public string ContentEncoding { get; set; }
		public string ContentType { get; set; }
		public byte DeliveryMode { get; set; }
		public string MessageId { get; set; }
		public byte Priority { get; set; }
		public string ReplyTo { get; set; }
		public string Type { get; set; }
		public string UserId { get; set; }
	}
}
