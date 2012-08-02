using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATTi.TMail.Common;
using ATTi.TMail.Model;

namespace ATTi.TMail.Service.Implementation
{
	/// <summary>
	/// Extension methods for the IRecipientList type.
	/// </summary>
	public static class RecipientListExtensions
	{
		static readonly int CDefaultStringBuilderSize = 400;
		static readonly string CRecordDelimeter = "\n";
		static readonly string CLogRecordDelimeter = ";";
		static readonly string CFieldDelimeter = "::";

		public static string ToStrongMailTokenStream(this Recipient[] list)
		{
			return ToStrongMailTokenStream(list, true, CRecordDelimeter);
		}

		public static string ToStrongMailEmailAddressStream(this Recipient[] list)
		{
			return ToStrongMailTokenStream(list, false, CLogRecordDelimeter);
		}

		private static string ToStrongMailTokenStream(this Recipient[] list, bool emailAddressOnly, string recordDelimeter)
		{
			list.EnsureRecipientListIsValid();
			var builder = new StringBuilder(CDefaultStringBuilderSize);
			foreach (var recipient in list)
			{
				builder.Append(recipient.Email);
				if (emailAddressOnly && recipient.MergeData != null && recipient.MergeData.Length > 0)
				{
					builder.Append(CFieldDelimeter).Append(String.Join(CFieldDelimeter, recipient.MergeData));
				}
				builder.Append(recordDelimeter);
			}
			return builder.ToString();
		}
	}
}
