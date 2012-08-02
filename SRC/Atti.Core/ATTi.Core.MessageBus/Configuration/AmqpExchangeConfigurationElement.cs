using System.Configuration;
using ATTi.Core.Configuration;
using ATTi.Core.Contracts;
using RabbitMQ.Client;

namespace ATTi.Core.MessageBus.Configuration
{		
	public class AmqpExchangeConfigurationElement : AmqpItemConfigurationElement
	{
		static string[] __ExchangeTypes = new string[] { RabbitMQ.Client.ExchangeType.Direct, RabbitMQ.Client.ExchangeType.Fanout, RabbitMQ.Client.ExchangeType.Topic };

		const string PropertyName_exchangeType = "exchangeType";
		[ConfigurationProperty(PropertyName_exchangeType, IsKey = true, IsRequired = true)]
		public string ExchangeType
		{
			get { return (string)this[PropertyName_exchangeType]; }
			set
			{
				Require.ExpectNotOneOf("ExchangeType", value, __ExchangeTypes);

				this[PropertyName_exchangeType] = value;
			}
		}

		const string PropertyName_isInternal = "isInternal";
		[ConfigurationProperty(PropertyName_isInternal, IsRequired = false, DefaultValue = false)]
		public bool IsInternal
		{
			get { return (bool)this[PropertyName_isInternal]; }
			set { this[PropertyName_isInternal] = value; }
		}

		internal void DeclareExchange(IModel model)
		{
			Require.IsNotNull("model", model);

			model.ExchangeDeclare(Name, ExchangeType, IsPassive, IsDurable, IsAutoDelete, IsInternal, IsNoWait, null);
		}
	}

	public class AmqpExchangeConfigurationElementCollection : AbstractConfigurationElementCollection<AmqpExchangeConfigurationElement, string>
	{
		protected override string PerformGetElementKey(AmqpExchangeConfigurationElement element)
		{
			return element.Name;
		}
	}

}
