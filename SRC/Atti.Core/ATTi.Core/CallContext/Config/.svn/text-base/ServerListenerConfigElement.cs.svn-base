using System;
using System.Configuration;
using System.Xml;
using ATTi.Core.Configuration;

namespace ATTi.Core.CallContext.Config
{
	public sealed class ServerListenerConfigElement : ConfigurationElement
	{
		const string PropertyName_type = "name";
				
		string _itemConfig;
		Type _listenerTypeResolved;

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (String.Equals(elementName, "itemConfig"))
			{
				_itemConfig = reader.ReadOuterXml();
				return true;
			}
			else
			{
				return base.OnDeserializeUnrecognizedElement(elementName, reader);
			}
		}
		
		[ConfigurationProperty(PropertyName_type, IsRequired = true, DefaultValue = "")]
		public string TypeName
		{
			get { return (string)base[PropertyName_type]; }
			set { base[PropertyName_type] = value; }
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

	[ConfigurationCollection(typeof(ServerListenerConfigElement))]
	public sealed class ServerListenersConfigSectionCollection : AbstractConfigurationElementCollection<ServerListenerConfigElement, string>
	{
		protected override string PerformGetElementKey(ServerListenerConfigElement element)
		{
			return element.TypeName;
		}
	}
}
