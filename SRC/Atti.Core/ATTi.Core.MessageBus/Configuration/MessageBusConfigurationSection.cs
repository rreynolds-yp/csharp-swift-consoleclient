using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ATTi.Core.Trace;
using System.Diagnostics;

namespace ATTi.Core.MessageBus.Configuration
{
	public sealed class MessageBusConfigurationSection : ConfigurationSection, ITraceable
	{
		public const string SectionName = "messagebus";
		public const string PropertyName_connections = "connections";
		public const string PropertyName_exchanges = "exchanges";
		public const string PropertyName_queues = "queues";

		[ConfigurationProperty(PropertyName_connections, IsDefaultCollection = true)]
		public AmqpConnectionConfigurationElementCollection Connections
		{
			get { return (AmqpConnectionConfigurationElementCollection)this[PropertyName_connections]; }
		}

		[ConfigurationProperty(PropertyName_exchanges, IsDefaultCollection = false)]
		public AmqpExchangeConfigurationElementCollection Exchanges
		{
			get { return (AmqpExchangeConfigurationElementCollection)this[PropertyName_exchanges]; }
		}

		[ConfigurationProperty(PropertyName_queues, IsDefaultCollection = false)]
		public AmqpQueueConfigurationElementCollection Queues
		{
			get { return (AmqpQueueConfigurationElementCollection)this[PropertyName_queues]; }
		}

		static Object __lock = new Object();
		static WeakReference __instance;
		public static MessageBusConfigurationSection Instance
		{
			get
			{
				MessageBusConfigurationSection config;
				lock (__lock)
				{
					if (__instance == null || __instance.IsAlive == false)
					{
						config = ConfigurationManager.GetSection(SectionName) as MessageBusConfigurationSection;
						if (config == null)
						{
							throw new ConfigurationErrorsException(String.Concat("Configuration section not found:", SectionName));
						}
						__instance = new WeakReference(config);
					}
					else
					{
						config = (MessageBusConfigurationSection)__instance.Target;
					}
				}
				return config;				
			}
		}
	}
	
}
