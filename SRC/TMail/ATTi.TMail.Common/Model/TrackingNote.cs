using System;
using ATTi.Core.Dto;

namespace ATTi.TMail.Model
{
	public class TrackingNote
	{
		public long ID { get; set; }
		public MailTracking Tracking { get; set; }
		public MailTrackingStatus Status { get; set; }
		public DateTime DateCreated { get; set; }
		public string StatusNote { get; set; }
	}
}
