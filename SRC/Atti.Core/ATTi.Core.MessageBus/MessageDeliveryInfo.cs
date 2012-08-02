using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus
{
	public class MessageDeliveryInfo
	{
		public string Exchange { get; set; }
		public string Queue { get; set; }
		public string RoutingKey { get; set; }
		public bool Redelivered { get; set; }
	}	
}
