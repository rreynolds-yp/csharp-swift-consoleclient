using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.MessageBus.Configuration
{
	/// <summary>
	/// Indicates the method used to resolve a protocol name.
	/// </summary>
	public enum ProtocolResolutionKind
	{
		/// <summary>
		/// Resolved by protocol name.
		/// </summary>
		ByName = 0,
		/// <summary>
		/// Resolved by configuration setting.
		/// </summary>
		FromConfiguration,
		/// <summary>
		/// Resolved by environment variable.
		/// </summary>
		FromEnvironment,
	}

}
