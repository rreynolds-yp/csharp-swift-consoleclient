using System;
using ATTi.Core.Dto.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ATTi.Core.Tests.Dto
{
	[TestClass]
	public class JObjectExtensionTests
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestJObjectExtensions()
		{
			var data = new
			{
				BooleanValue = true,
				ByteValue = (byte)254,
				CharValue = "\u0032",
				DateTimeValue = new DateTime(1997, 6, 12),
				DecimalValue = 1.2M,
				DoubleValue = 1.3,
				ShortValue = (short)22,
				IntValue = 33,
				LongValue = 44L,
			};

			JObject item =
				new JObject(new JProperty("BooleanValue", data.BooleanValue),
					new JProperty("ByteValue", data.ByteValue),
					new JProperty("CharValue", data.CharValue),
					new JProperty("DateTimeValue", data.DateTimeValue)
					);

			bool boolval;
			Assert.IsTrue(item.TryReadNamedValue("BooleanValue", out boolval));
			Assert.AreEqual(data.BooleanValue, boolval);

			byte byteval;
			Assert.IsTrue(item.TryReadNamedValue("ByteValue", out byteval));
			Assert.AreEqual(data.ByteValue, byteval);

			string charval;
			Assert.IsTrue(item.TryReadNamedValue("CharValue", out charval));
			Assert.AreEqual(data.CharValue, charval);

			DateTime dtval;
			Assert.IsTrue(item.TryReadNamedValue("DateTimeValue", out dtval));
			Assert.AreEqual(data.DateTimeValue, dtval);

		}
	}
}
