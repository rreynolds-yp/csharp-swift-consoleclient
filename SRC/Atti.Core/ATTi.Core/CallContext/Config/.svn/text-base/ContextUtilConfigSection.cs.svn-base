using System.Configuration;

namespace ATTi.Core.CallContext.Config
{
	public sealed class ContextUtilConfigSection : ConfigurationSection
	{
		#region Declarations
		public const string ConfigSectionName = "contextUtil";
		const string PropertyName_clientListeners = "clientListeners";
		const string PropertyName_serverListeners = "serverListeners";
		#endregion

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public ContextUtilConfigSection() { }

		[ConfigurationProperty(PropertyName_clientListeners, IsDefaultCollection = true)]
		public ClientListenersConfigSectionCollection ClientListeners
		{
			get { return (ClientListenersConfigSectionCollection)this[PropertyName_clientListeners]; }
		}

		[ConfigurationProperty(PropertyName_serverListeners, IsDefaultCollection = false)]
		public ServerListenersConfigSectionCollection ServerListeners
		{
			get { return (ServerListenersConfigSectionCollection)this[PropertyName_serverListeners]; }
		}
	}

}
