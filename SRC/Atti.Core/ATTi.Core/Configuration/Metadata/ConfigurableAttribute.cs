namespace ATTi.Core.Configuration.Metadata
{
	using System;

	/// <summary>
	/// Attribute for declaring that an object is configurable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class ConfigurableAttribute : Attribute
	{
		

		private Type _configElementType;
		private string _configSectionName;

		

		#region Constructors

		public ConfigurableAttribute(string configSectionName, Type configElementType)
		{
			Contracts.Require.IsNotNull("configSectionName", configSectionName);
			Contracts.Require.IsNotNull("configElementType", configElementType);

			_configSectionName = configSectionName;
			_configElementType = configElementType;
		}

		#endregion Constructors

		

		public Type ConfigElementType
		{
			get { return _configElementType; }
			set
			{
				Contracts.Require.IsNotNull("ConfigElementType", value);

				_configElementType = value;
			}
		}

		/// <summary>
		/// Gets and sets the name of the config section where the object's 
		/// configuration lives.
		/// </summary>
		public string ConfigSectionName
		{
			get { return _configSectionName; }
			set
			{
				Contracts.Require.IsNotNull("ConfigSectionName", value);

				_configSectionName = value;
			}
		}

		
	}
}