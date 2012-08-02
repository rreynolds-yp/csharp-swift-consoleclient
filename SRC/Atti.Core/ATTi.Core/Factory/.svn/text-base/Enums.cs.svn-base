
namespace ATTi.Core.Factory
{
	/// <summary>
	/// Enumeration indicating an action type performed during
	/// a call to the CreateInstance method on a FactoryStrategy.
	/// </summary>
	public enum FactoryAction
	{
		Unknown = 0,
		/// <summary>
		/// Indicates the strategy provided new instance of the type.
		/// </summary>
		NewInstance,
		/// <summary>
		/// Indicates the strategy provided a shared instance.
		/// </summary>
		SharedInstance,
		/// <summary>
		/// Indicates the strategy provided a cached instance.
		/// </summary>
		CachedInstance,
		/// <summary>
		/// Indicates the instance was created via a constructor call not under
		/// factory control.
		/// </summary>
		ExternalConstruction
	}

	public enum InstanceReusePolicy
	{
		/// <summary>
		/// Indicates instances are not reused. Instances are PerCall
		/// </summary>
		None = 0,
		/// <summary>
		/// Indicates that the type should only allow a singleton instance. 
		/// Instance is reused according to InstanceReuseScope.
		/// </summary>
		Singleton,
		/// <summary>
		/// Instances are reused through caching.
		/// </summary>
		Cached
	}

	public enum SingletonReuseScope
	{
		LocalSharedByAll = 0,
		LocalSharedByThread,
		LocalSharedByUser,
		LocalHttpContext,
		SharedInCallContext		
	}

	public enum InstanceCacheStrategy
	{
	}
}
