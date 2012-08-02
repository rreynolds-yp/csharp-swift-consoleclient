using System;
using System.Configuration;
using System.Xml;
using ATTi.Core.Configuration;

namespace ATTi.Core.CallContext.Config
{
	public sealed class ClientListenerConfigElement : ConfigurationElement
	{
		const string PropertyName_name = "name";

		private string _itemConfig;
		private Type _listenerTypeResolved;

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (string.Equals(elementName, "itemConfig"))
			{
				_itemConfig = reader.ReadOuterXml();
				return true;
			}
			else
			{
				return base.OnDeserializeUnrecognizedElement(elementName, reader);
			}
		}

		[ConfigurationProperty(PropertyName_name, IsRequired = true, DefaultValue = "")]
		public string TypeName
		{
			get { return (string)base[PropertyName_name]; }
			set { base[PropertyName_name] = value; }
		}

		internal string ItemConfig
		{
			get { return _itemConfig; }
			set { _itemConfig = value; }
		}

		internal Type ListenerTypeResolved
		{
			get
			{
				if (_listenerTypeResolved == null)
				{
					string typeName = this.TypeName;
					if (typeName != null && !string.IsNullOrEmpty(typeName))
					{
						lock (this)
						{
							if (_listenerTypeResolved == null)
							{
								this._listenerTypeResolved = System.Type.GetType(typeName);
							}
						}
					}
				}
				return this._listenerTypeResolved;
			}
		}

	}

	[ConfigurationCollection(typeof(ClientListenerConfigElement))]
	public sealed class ClientListenersConfigSectionCollection : AbstractConfigurationElementCollection<ClientListenerConfigElement, string>
	{

		protected override string PerformGetElementKey(ClientListenerConfigElement element)
		{
			return element.TypeName;
		}
	}
}
