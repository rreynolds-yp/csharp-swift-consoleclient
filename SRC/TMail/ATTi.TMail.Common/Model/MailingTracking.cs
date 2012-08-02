using System;
using ATTi.Core.Dto;
using System.Collections.Generic;

namespace ATTi.TMail.Model
{
	public class MailTracking
	{
		public Guid ID { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }

		public MailTrackingStatus Status { get; set; }
		public string Application { get; set; }
		public string Environment { get; set; }
		public string EmailTemplate { get; set; }

		public IEnumerable<TrackingNote> Notes { get; set; }
	}
}
