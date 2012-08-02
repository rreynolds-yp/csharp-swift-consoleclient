using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.Core.Dto;
using ATTi.TMail.Model;
using ATTi.TMail.Common;
using ATTi.TMail.Service.Implementation.Configuration;
using ATTi.TMail.StrongMail.TransactionalApi;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace ATTi.TMail.Service.Implementation.Tests.StrongMail
{
	/// <summary>
	/// Summary description for TransactionalMailTests
	/// </summary>
	[TestClass]
	public class TransactionalMailTests
	{
		public TestContext TestContext { get; set; }

		private string GetLocalIPAddress()
		{
			var nic = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var n in nic)
			{
				var info = n.GetIPProperties();
				if (info.DnsSuffix.EndsWith(".com")
					|| info.DnsSuffix.EndsWith(".net")
					|| info.DnsSuffix.EndsWith(".org")
					|| info.DnsSuffix.EndsWith(".local"))
					return info.DnsAddresses[0].ToString();
			}
			return "ip_address_unknown";
		}
				
		[TestMethod]
		public void DirectSend()
		{
			var now = DateTime.UtcNow.ToString("u");
			var localIPAddress = GetLocalIPAddress();
			var mailing = new
			{
				MailingName = "Mailing.638.5035",
				RecipientList = JsonConvert.DeserializeObject<Recipient[]>(
					String.Concat("[ { Email: 'edwardt.tril@gmail.com', MergeData: ['Edward', '", localIPAddress, "', '", now, "' ] }, ",
						" { Email: 'cerebralkungfu@gmail.com', MergeData: ['Phillip', '", localIPAddress, "', '", now, "' ] } ]"))					
			};

			var config = StrongMailConfigurationSection.Instance;
			var easApi = config.CreateTransactionalApiWebService();
			try
			{
				var status = easApi.GetState(config.EasApiCredentials, mailing.MailingName);

				long count;
				var result = easApi.Send(config.EasApiCredentials, mailing.MailingName,
					mailing.RecipientList.ToStrongMailTokenStream(), out count);

				long serialNum;
				string mailingID;
				MailingStates mailingState;
				string startTime;
				string endTime;
				long elapsedTime;
				string lastRestartTime;
				long restarts;
				long totaldatabaseRecords;
				long messagesDelivered;
				long messagesFailed;
				long messagesDeferred;
				long messagesInvalid;

				var statusResult = easApi.GetStatus(config.EasApiCredentials, mailing.MailingName, out serialNum, out mailingState, out mailingID,
					out startTime,
					out endTime, out elapsedTime, out lastRestartTime, out restarts, out totaldatabaseRecords,
					out messagesDelivered, out messagesFailed, out messagesDeferred, out messagesInvalid);
			}
			catch (Exception)
			{ /* this catch is here to inspect the exception during debugging */
				throw;
			}
		}
	}
}
