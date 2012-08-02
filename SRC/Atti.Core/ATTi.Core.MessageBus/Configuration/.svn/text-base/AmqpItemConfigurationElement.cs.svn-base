using System.Configuration;
using ATTi.Core.Configuration;

namespace ATTi.Core.MessageBus.Configuration
{		
	/// <summary>
	/// Base class for AmqpQueueConfigurationElement and AmqpExchangeConfigurationElement
	/// </summary>
	public class AmqpItemConfigurationElement : ConfigurationElement
	{
		const string PropertyName_name = "name";
		const string PropertyName_isPassive = "isPassive";
		const string PropertyName_isDurable = "isDurable";
		const string PropertyName_isAutoDelete = "isAutoDelete";
		const string PropertyName_isNoWait = "noWait";
		
		/// <summary>
		/// Gets or sets the connection's name.
		/// </summary>
		/// <value>The connection's name.</value>
		[ConfigurationProperty(PropertyName_name, IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set { this[PropertyName_name] = value; }
		}

		[ConfigurationProperty(PropertyName_isPassive, IsRequired = false, DefaultValue = false)]
		public bool IsPassive
		{
			get { return (bool)this[PropertyName_isPassive]; }
			set { this[PropertyName_isPassive] = value; }
		}

		[ConfigurationProperty(PropertyName_isDurable, IsRequired = false, DefaultValue = false)]
		public bool IsDurable
		{
			get { return (bool)this[PropertyName_isDurable]; }
			set { this[PropertyName_isDurable] = value; }
		}

		[ConfigurationProperty(PropertyName_isAutoDelete, IsRequired = false, DefaultValue = false)]
		public bool IsAutoDelete
		{
			get { return (bool)this[PropertyName_isAutoDelete]; }
			set { this[PropertyName_isAutoDelete] = value; }
		}

		[ConfigurationProperty(PropertyName_isNoWait, IsRequired = false, DefaultValue = false)]
		public bool IsNoWait
		{
			get { return (bool)this[PropertyName_isNoWait]; }
			set { this[PropertyName_isNoWait] = value; }
		}		
	}
		
}
