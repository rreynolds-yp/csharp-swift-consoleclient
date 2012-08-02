using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.TMail.Service.Implementation.Agent;
using ATTi.Core.Factory;

namespace ATTi.TMail.Service.Implementation.Tests
{
	[TestClass]
	public class AgentProtocolTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void Monkey()
		{
			var agent = Factory<AgentProtocol>.CreateInstance();
			Assert.IsNotNull(agent);


		}
	}
}
