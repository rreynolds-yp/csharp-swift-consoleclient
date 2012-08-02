using System;
using System.Configuration;
using ATTi.Core.Configuration;
using System.Diagnostics;
using ATTi.Core.Properties;

namespace ATTi.Core.Factory
{	
	public class FactoryBindingElement : ConfigurationElement, IFactoryBinding
	{
		Type _type;
		Type _factoryType;		
		Type _implementationType;

		[ConfigurationProperty(FactoryBinding.PropertyName_type
			, IsKey = true
			, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[FactoryBinding.PropertyName_type]; }
			set
			{
				this[FactoryBinding.PropertyName_type] = value; 
				_type = null;
			}
		}

		[ConfigurationProperty(FactoryBinding.PropertyName_factoryType			
			, IsRequired = false)]
		public string FactoryTypeName
		{
			get { return (string)this[FactoryBinding.PropertyName_factoryType]; }
			set
			{
				this[FactoryBinding.PropertyName_factoryType] = value;
				_factoryType = null;
			}
		}

		[ConfigurationProperty(FactoryBinding.PropertyName_implementationType, IsRequired = false)]
		public string ImplementationTypeName
		{
			get { return (string)this[FactoryBinding.PropertyName_implementationType]; }
			set
			{
				this[FactoryBinding.PropertyName_implementationType] = value;
				_implementationType = null;
			}
		}

		[ConfigurationProperty(FactoryBinding.PropertyName_instanceConfig
					, IsRequired = false)]
		public XmlSerializedInstanceConfiguraionElement InstanceConfig
		{
			get { return (XmlSerializedInstanceConfiguraionElement)this[FactoryBinding.PropertyName_instanceConfig]; }
			set { this[FactoryBinding.PropertyName_instanceConfig] = value; }
		}

		#region IFactoryBinding Members

		Type IFactoryBinding.Type
		{
			get
			{
				if (_type == null && !String.IsNullOrEmpty(this.TypeName))
				{
					_type = Type.GetType(this.TypeName, false);
				}
				return _type;
			}
			set { _type = value; }
		}

		Type IFactoryBinding.FactoryType
		{
			get
			{
				if (_factoryType == null && !String.IsNullOrEmpty(this.FactoryTypeName))
				{
					_factoryType = Type.GetType(this.FactoryTypeName, false);
				}
				return _factoryType;
			}
			set { _factoryType = value; }
		}

		Type IFactoryBinding.ConcreteType
		{
			get
			{
				if (_implementationType == null && !String.IsNullOrEmpty(this.ImplementationTypeName))
				{
					_implementationType = Type.GetType(this.ImplementationTypeName, false);
				}
				return _implementationType;
			}
			set { _implementationType = value; }
		}


		object IFactoryBinding.GetInstanceConfig()
		{
			XmlSerializedInstanceConfiguraionElement ielm = this.InstanceConfig;
			return (ielm == null) ? null : ielm.Instance;
		}
		#endregion
	}

	public class FactoryConfigurationElementCollection : AbstractConfigurationElementCollection<FactoryBindingElement, string>
	{
		protected override string PerformGetElementKey(FactoryBindingElement element)
		{
			return element.TypeName;
		}
	}

	public sealed class FactoryConfigurationSection : ConfigurationSection
	{
		public const string SectionName = "factory";
		public const string PropertyName_bindings = "bindings";
		public const string PropertyName_externalBindingLocation = "externalBindingLocation";


		[ConfigurationProperty(FactoryConfigurationSection.PropertyName_externalBindingLocation
			, IsRequired = false)]
		public string ExternalBindingLocation
		{
			get { return (string)this[PropertyName_externalBindingLocation]; }
			set	{	this[PropertyName_externalBindingLocation] = value; }
		}

		[ConfigurationProperty(FactoryConfigurationSection.PropertyName_bindings, IsDefaultCollection = true)]
		public FactoryConfigurationElementCollection Bindings
		{
			get { return (FactoryConfigurationElementCollection)this[PropertyName_bindings]; }
		}
	}
}
