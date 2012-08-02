using ATTi.Core.Contracts;
using ATTi.TMail.Common;
using ATTi.Core.Dto;
using ATTi.Core.Factory;
using ATTi.TMail.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Newtonsoft.Json;

namespace ATTi.TMail.Service.Implementation.Tests
{
	/// <summary>
	/// Tests the TMailService's methods.
	/// </summary>
	[TestClass]
	public class TMailServiceTests
	{

		public TestContext TestContext { get; set; }

		[TestMethod]
		public void CreateMailing()
		{
			var args = new 
				{
					App = "tmail",
					Env = "dev",
					TemplateName = "Mailing.638.5035",
					RecipientList = JsonConvert.DeserializeObject<Recipient[]>("[ { Email: 'pclark@atti.com', MergeData: ['Phillip', '$(UserHostAddress)', '$(ServerTimestamp)' ] } ]")
				};

			var svc = Factory<ITMailService>.CreateInstance();
			Assert.IsNotNull(svc, "ITMailService should be reachable from the factory");

			var id = Guid.NewGuid();
			svc.CreateMailing(id, args.App, args.Env, args.TemplateName, args.RecipientList);
			var tracking = svc.GetMailingStatus(args.App, args.Env, id);
			Assert.IsNotNull(tracking);
			Assert.AreEqual(id, tracking.ID);
		}

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

	}
}
