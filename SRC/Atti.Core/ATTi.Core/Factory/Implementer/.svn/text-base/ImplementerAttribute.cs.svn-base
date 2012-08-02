using System;

namespace ATTi.Core.Factory.Implementer
{
	/// <summary>
	/// Attribute assigning an implementer that will generate a concrete
	/// type for the interface to which the attribute is applied.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public abstract class ImplementerAttribute : Attribute
	{
		internal Type GetImplementationForType(Type targetType)
		{
			return ImplementerInvoke(targetType);
		}

		protected abstract Type ImplementerInvoke(Type targetType);				
	}
}
