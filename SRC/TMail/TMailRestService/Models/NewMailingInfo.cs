using ATTi.TMail.Model;

namespace TMailService.Models
{
	public class NewMailingInfo
	{
		public string EmailTemplate { get; set; }
		public Recipient[] Recipients { get; set; }
	}
}
