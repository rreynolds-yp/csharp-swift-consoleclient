using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.Core.MessageBus.Configuration;

namespace ATTi.Core.MessageBus.Tests
{
	/// <summary>
	/// Summary description for ConfigTests
	/// </summary>
	[TestClass]
	public class ConfigTests
	{		
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void Instance()
		{
			var config = MessageBusConfigurationSection.Instance;
			using (var cn = config.Connections["default"].CreateConnection())
			{				
				Assert.IsNotNull(cn);
			}
		}
	}
}
