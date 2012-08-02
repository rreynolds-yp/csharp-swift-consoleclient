using System;
using System.Collections.Generic;

namespace TMailRestService.Models
{
	public class MailStatusNote
	{
		public string Status { get; set; }
		public string Note { get; set; }
		public DateTime DateCreated { get; set; }
	}

	public class MailingStatus
	{
		public string Location { get; set; }

		public Guid ID { get; set; }

		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }

		public string Status { get; set; }
		public string Application { get; set; }
		public string Environment { get; set; }
		public string EmailTemplate { get; set; }

		public IEnumerable<MailStatusNote> Notes { get; set; }
	}

}
