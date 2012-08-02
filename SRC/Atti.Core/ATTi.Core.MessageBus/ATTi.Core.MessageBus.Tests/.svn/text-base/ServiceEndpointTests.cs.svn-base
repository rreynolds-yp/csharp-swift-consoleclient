using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATTi.Core.MessageBus.Tests
{
	/// <summary>
	/// Tests the service endpoint.
	/// </summary>
	[TestClass]
	public class ServiceEndpointTests
	{

		/// <summary>
		/// Gets or sets the test context.
		/// </summary>
		/// <value>The test context.</value>
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void ConsumeAndProduce()
		{
			var server = "default";
			var queueName = "default";

			using (var cn = MessageBus.Connect(server))
			{
				using (var endpoint = cn.OpenQueueAsConsumer(queueName, String.Empty))
				{
					endpoint.MatchMessagesWithProperties(p => p.ContentType == "application/json")
						.UseTransactions()
						.TransformMessageBody<string>((p, b) =>
							{
								var enc = (String.IsNullOrEmpty(p.ContentEncoding))
									? UTF8Encoding.UTF8
									: Encoding.GetEncoding(p.ContentEncoding);
								return new String(enc.GetChars(b));
							})
						.HandleMessages<string>(
							(p, r, json) =>
							{
								Console.WriteLine(json);							
								return default(MessageReturnAction);
							});

					endpoint.Startup();

					using (var cn2 = MessageBus.Connect(server))
					{
						using (var sendpoint = cn.OpenExchangeAsProducer(queueName, "test"))
						{
							sendpoint.TransformProgrammaticType<string>((p, s) =>
									{
										p.ContentEncoding = UTF8Encoding.UTF8.BodyName;
										p.ContentType = "application/json";
										return UTF8Encoding.UTF8.GetBytes(s);
									})
								.PublishMessage("{ Message: 'this is a message from the producer' }");
						}
					}

					endpoint.WaitForShutdown(TimeSpan.FromSeconds(30));
				}
			}
		}
	}
}
