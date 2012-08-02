using System;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Interface for factory bindings.
	/// </summary>
	public interface IFactoryBinding
	{
		/// <summary>
		/// The target type for the binding.
		/// </summary>
		Type Type { get; set; }
		/// <summary>
		/// The factory type that creates instances of the target type.
		/// </summary>
		Type FactoryType { get; set; }		
		/// <summary>
		/// The concrete type used as default.
		/// </summary>
		Type ConcreteType { get; set; }
		/// <summary>
		/// A configuration object used to initialize new instances.
		/// </summary>
		/// <returns></returns>
		object GetInstanceConfig();
	}
}
