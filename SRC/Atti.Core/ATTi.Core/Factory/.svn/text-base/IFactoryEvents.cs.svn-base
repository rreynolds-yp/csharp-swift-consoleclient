using System;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Delegate signature used by the framework to notify observers when factories respond 
	/// to CreateInstance calls.
	/// </summary>
	/// <typeparam name="T">Type of the instance being observed.</typeparam>
	/// <param name="requestedType">The type of instance that was requested. This will correspond
	/// with the Factory&lt;U&gt; type on which CreateInstance was called, where U is the requested type.</param>
	/// <param name="instance">The newly issued instance.</param>
	/// <param name="name">Name of the instance if the instance is named; otherwise null.</param>
	/// <param name="action">The action taken by the factory. This is controlled by the InstanceReusePolicy
	/// currently in effect for the type T.</param>
	public delegate void FactoryInstanceEvent<T>(Type requestedType, T instance, string name, FactoryAction action);

}
