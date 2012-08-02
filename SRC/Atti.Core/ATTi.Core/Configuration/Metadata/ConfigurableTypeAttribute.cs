namespace ATTi.Core.Configuration.Metadata
{
	using System;

	/// <summary>
	/// Attribute for declaring that an object is configurable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigurableTypeAttribute : Attribute
	{
		

		private Type _configElementType;

		

		#region Constructors

		public ConfigurableTypeAttribute(Type configElementType)
		{
			Contracts.Require.IsNotNull("configElementType", configElementType);

			_configElementType = configElementType;
		}

		#endregion Constructors

		

		public Type ConfigElementType
		{
			get { return _configElementType; }
			set
			{
				Contracts.Require.IsNotNull("value", value);

				_configElementType = value;
			}
		}

		
	}
}