using ATTi.Core.Dto;
using System;

namespace ATTi.TMail.Model
{	
	public class Recipient
	{
		public string Email { get; set; }
		public string[] MergeData { get; set; }
	}
}
