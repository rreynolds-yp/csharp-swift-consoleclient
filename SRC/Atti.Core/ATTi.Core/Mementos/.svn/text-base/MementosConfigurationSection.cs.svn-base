using System;
using System.Configuration;
using ATTi.Core.Configuration;

namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Configuration element for mementos.
	/// </summary>
	public class MementoConfigurationElement : ConfigurationElement
	{
		public const string PropertyName_type = "type";
		public const string PropertyName_helper = "helper";

		Type _type;
		Type _helper;

		/// <summary>
		/// Gets or sets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		[ConfigurationProperty(PropertyName_type
			, IsKey = true
			, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[PropertyName_type]; }
			set
			{
				this[PropertyName_type] = value;
				_type = null;
			}
		}

		/// <summary>
		/// Gets the resolved type.
		/// </summary>
		/// <value>The resolved type.</value>
		public Type ResolvedType
		{
			get
			{
				if (_type == null && !String.IsNullOrEmpty(this.TypeName))
				{
					_type = Type.GetType(this.TypeName, false);
				}
				return _type;
			}
		}

		/// <summary>
		/// Gets or sets the name of the helper type.
		/// </summary>
		/// <value>The name of the helper type.</value>
		[ConfigurationProperty(PropertyName_helper, IsRequired = true)]
		public string HelperTypeName
		{
			get { return (string)this[PropertyName_helper]; }
			set
			{
				this[PropertyName_helper] = value;
				_helper = null;
			}
		}

		/// <summary>
		/// Gets the helper type.
		/// </summary>
		/// <value>The helper type.</value>
		public Type ResolvedHelperType
		{
			get
			{
				if (_helper == null && !String.IsNullOrEmpty(this.HelperTypeName))
				{
					_helper = Type.GetType(this.HelperTypeName, false);
				}
				return _helper;
			}
		}
	}

	/// <summary>
	/// Collection of MementoConfigurationElements.
	/// </summary>
	public class MementoConfigurationElementCollection : AbstractConfigurationElementCollection<MementoConfigurationElement, Type>
	{
		protected override Type PerformGetElementKey(MementoConfigurationElement element)
		{
			return element.ResolvedType;
		}
	}

	public sealed class MementosConfigurationSection : ConfigurationSection
	{
		public const string SectionName = "mementos";
		public const string PropertyName_helpers = "helpers";


		[ConfigurationProperty(PropertyName_helpers, IsDefaultCollection = true)]
		public MementoConfigurationElementCollection Helpers
		{
			get { return (MementoConfigurationElementCollection)this[PropertyName_helpers]; }
		}


	}

}
