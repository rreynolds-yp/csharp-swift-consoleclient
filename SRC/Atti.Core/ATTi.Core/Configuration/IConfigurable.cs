namespace ATTi.Core.Configuration
{
	/// <summary>
	/// Interface for classes that are configured by the framework.
	/// </summary>
	public interface IConfigurable<TConfigElement>
	{
		/// <summary>
		/// Called by the configuration framework to configure an instance.
		/// </summary>
		/// <param name="config">Configuration object or null.</param>
		void Configure(TConfigElement config);
	}
}