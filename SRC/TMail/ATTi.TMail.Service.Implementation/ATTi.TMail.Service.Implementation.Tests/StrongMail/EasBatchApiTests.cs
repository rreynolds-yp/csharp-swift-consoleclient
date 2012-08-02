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

namespace ATTi.TMail.Service.Implementation.Tests.StrongMail
{
	/// <summary>
	/// Summary description for TransactionalMailTests
	/// </summary>
	[TestClass]
	public class EasBatchApiTests
	{
		public TestContext TestContext { get; set; }
						
		[TestMethod]
		public void Monkey()
		{
			var config = StrongMailConfigurationSection.Instance;
			var batchApi = config.CreateBatchApiWebService();
			try
			{
				
			}
			catch
			{ /* this catch is here to inspect the exception during debugging */
				throw;
			}
		}
	}
}
