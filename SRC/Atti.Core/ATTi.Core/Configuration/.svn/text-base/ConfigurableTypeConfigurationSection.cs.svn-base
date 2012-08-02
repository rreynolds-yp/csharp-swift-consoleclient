namespace ATTi.Core.Configuration
{
	using System.Configuration;

	/// <summary>
	/// Configuration element for configurable types.
	/// </summary>
	public class ConfigurableTypeConfigurationElement : ConfigurationElement
	{
		public const string PropertyName_type = "type";
		public const string PropertyName_typeConfig = "typeConfig";		

		[ConfigurationProperty(ConfigurableTypeConfigurationElement.PropertyName_typeConfig
			, IsRequired = false)]
		public XmlSerializedInstanceConfiguraionElement TypeConfig
		{
			get { return (XmlSerializedInstanceConfiguraionElement)this[PropertyName_typeConfig]; }
			set { this[PropertyName_typeConfig] = value; }
		}

		[ConfigurationProperty(ConfigurableTypeConfigurationElement.PropertyName_type
			, IsKey = true
			, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[PropertyName_type]; }
			set {	this[PropertyName_type] = value; }
		}		

		internal object GetTypeConfig()
		{
			XmlSerializedInstanceConfiguraionElement ielm = this.TypeConfig;
			return (ielm == null) ? null : ielm.Instance;
		}		
	}

	/// <summary>
	/// Collection for configurable type configurations.
	/// </summary>
	public class ConfigurableTypeConfigurationElementCollection : AbstractConfigurationElementCollection<ConfigurableTypeConfigurationElement, string>
	{
		

		/// <summary>
		/// Gets the type name which is used as the element's key.
		/// </summary>
		/// <param name="element">A target element.</param>
		/// <returns>The TypeName property of the element given.</returns>
		protected override string PerformGetElementKey(ConfigurableTypeConfigurationElement element)
		{
			return element.TypeName;
		}

		
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ConfigurableTypeConfigurationSection : ConfigurationSection
	{
		

		public const string PropertyName_types = "types";
		public const string SectionName = "typeConfigurations";

		

		

		[ConfigurationProperty(ConfigurableTypeConfigurationSection.PropertyName_types
			, IsDefaultCollection = true)]
		public ConfigurableTypeConfigurationElementCollection Types
		{
			get { return (ConfigurableTypeConfigurationElementCollection)this[PropertyName_types]; }
		}

		
	}
}