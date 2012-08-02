using ATTi.Core.Dto;
using ATTi.Core.Factory;
using ATTi.Core.Wireup;
using ATTi.Core.Wireup.Metadata;
using ATTi.TMail.Common;
using ATTi.TMail.Model;
using ATTi.TMail.Service.Implementation;

[assembly: Wireup(typeof(LocalWireupCommand))]

namespace ATTi.TMail.Service.Implementation
{
	/// <summary>
	/// Wires up this assembly.
	/// </summary>
	public sealed class LocalWireupCommand : WireupCommand
	{
		protected override void PerformWireup()
		{
			// Establish the TMailService as the default implementation for ITMailService interface,
			// And since it is stateless make it a singleton.
			Factory<ITMailService>.Wireup
				.SetDefaultImplementation<TMailService>()
				.SetInstanceReusePolicy(InstanceReusePolicy.None);
		}
	}
}
