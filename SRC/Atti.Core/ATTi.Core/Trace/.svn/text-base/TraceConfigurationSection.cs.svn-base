
namespace ATTi.Core.Trace
{
	using System.Configuration;

	/// <summary>
	/// Configuration section for diagnostics settings.
	/// </summary>
	public sealed class TraceConfigurationSection : ConfigurationSection
	{
		

		/// <summary>
		/// Configuration section name for trace settings.
		/// </summary>
		public static readonly string SectionName = "atti-core-trace";

		/// <summary>
		/// Default value indicating unknown.
		/// </summary>
		public const string DefaultValue_unknown = "unknown";

		/// <summary>
		/// Property name for component.
		/// </summary>
		public const string PropertyName_component = "component";

		/// <summary>
		/// Property name for defaultTraceSource.
		/// </summary>
		public const string PropertyName_defaultTraceSource = "defaultTraceSource";

		/// <summary>
		/// Property name for environment.
		/// </summary>
		public const string PropertyName_environment = "environment";

		

		

		/// <summary>
		/// Indicates the name of the component that the current application represents.
		/// The meaning of "component" is up to the user but in general indicates a
		/// role that an application performs within a system.
		/// </summary>
		[ConfigurationProperty(PropertyName_component
			, DefaultValue = DefaultValue_unknown)]
		public string Component
		{
			get { return (string)this[PropertyName_component]; }
			set { this[PropertyName_component] = value; }
		}

		/// <summary>
		/// For systems using the LWES Diagnostics utility class, determines the default
		/// trace source.
		/// </summary>
		/// <seealso cref="System.Diagnostics.TraceSource"/>
		[ConfigurationProperty(PropertyName_defaultTraceSource
			, DefaultValue = DefaultValue_unknown)]
		public string DefaultTraceSource
		{
			get { return (string)this[PropertyName_defaultTraceSource]; }
			set { this[PropertyName_defaultTraceSource] = value; }
		}

		/// <summary>
		/// Indicates the name of the environment in which the application is executing.
		/// The meaning of "environment" is up to the user but in general indicates an
		/// environment such as: { dev | test | staging | production }. In cases where
		/// events in one environment can be heard by journalers in another environment
		/// the presence of this value in an event helps with filtering.
		/// </summary>
		[ConfigurationProperty(PropertyName_environment
			, DefaultValue = DefaultValue_unknown)]
		public string Environment
		{
			get { return (string)this[PropertyName_environment]; }
			set { this[PropertyName_environment] = value; }
		}

		internal static TraceConfigurationSection Current
		{
			get
			{
				TraceConfigurationSection config = ConfigurationManager.GetSection(
					TraceConfigurationSection.SectionName) as TraceConfigurationSection;
				return config ?? new TraceConfigurationSection();
			}
		}

		
	}
}