using System;
using System.Collections;
using System.Configuration;
using ATTi.Core.Configuration;
using ATTi.Core.Contracts;
using RabbitMQ.Client;

namespace ATTi.Core.MessageBus.Configuration
{	
	public class AmqpQueueConfigurationElement : AmqpItemConfigurationElement
	{
		object _declareLock = new Object();
		string _declaredQueueName;

		const string PropertyName_isExclusive = "isExclusive";
		[ConfigurationProperty(PropertyName_isExclusive, IsRequired = false, DefaultValue = false)]
		public bool IsExclusive
		{
			get { return (bool)this[PropertyName_isExclusive]; }
			set { this[PropertyName_isExclusive] = value; }
		}

		const string PropertyName_queueName = "queueName";
		[ConfigurationProperty(PropertyName_queueName, IsKey = true, IsRequired = false)]
		public string QueueName
		{
			get { return (string)this[PropertyName_queueName]; }
			set { this[PropertyName_queueName] = value; }
		}

		const string PropertyName_exchangeName = "exchangeName";
		[ConfigurationProperty(PropertyName_exchangeName, IsKey = true, IsRequired = false)]
		public string ExchangeName
		{
			get { return (string)this[PropertyName_exchangeName]; }
			set { this[PropertyName_exchangeName] = value; }
		}

		internal string DeclareQueue(IModel model)
		{
			Require.IsNotNull("model", model);

			_declaredQueueName = model.QueueDeclare(QueueName, IsPassive, IsDurable, IsExclusive, IsAutoDelete, IsNoWait, null);
			return _declaredQueueName;
		}

		internal string ActualQueueName
		{
			get { return _declaredQueueName ?? QueueName; }
		}

		internal void BindQueue(IModel model, string routingKey, IDictionary arguments)
		{
			Require.IsNotNull("model", model);

			var qname = _declaredQueueName ?? QueueName;
			if (String.IsNullOrEmpty(qname))
				throw new InvalidOperationException(String.Concat("QueueName not declared for configured queue: ", Name, ".", Environment.NewLine,
					"Call the configuration element's DeclareQueue method before binding the queue."));

			var xname = ExchangeName ?? qname;

			model.QueueBind(qname, xname, routingKey, IsNoWait, arguments);
			model.TxSelect();
		}
	}

	public class AmqpQueueConfigurationElementCollection : AbstractConfigurationElementCollection<AmqpQueueConfigurationElement, string>
	{
		protected override string PerformGetElementKey(AmqpQueueConfigurationElement element)
		{
			return element.Name;
		}
	}

}
