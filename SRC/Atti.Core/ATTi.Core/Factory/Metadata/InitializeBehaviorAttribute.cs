using System;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Indicates when initialization of the object should occur. The class factory
	/// will ensure this timing policy is used when initializing intances of the type.
	/// </summary>
	public enum InitializeTiming
	{
		/// <summary>
		/// Indicates the default initialize timing will be employed.
		/// </summary>
		Default = 0,
		/// <summary>
		/// Indicates frameworks should attempt to auto-initialize before 
		/// factory notification.
		/// </summary>
		AutoBeforeNotification = 0,
		/// <summary>
		/// Indicates frameworks should attempt to auto-initialize after
		/// factory notification.
		/// </summary>
		AutoAfterNotification = 1,
		/// <summary>
		/// Indicates frameworks should not auto-initialize.
		/// </summary>
		NoAutoInitialize = 2
	}

	/// <summary>
	/// Attribute indicating when an initialization method should be called for a class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InitializeBehaviorAttribute : Attribute
	{
		/// <summary>
		/// Gets and sets the initialization timing.
		/// </summary>
		public InitializeTiming Timing { get; set; }
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public InitializeBehaviorAttribute() { }
		/// <summary>
		/// Constructs a new instance with the timing given.
		/// </summary>
		/// <param name="timing">The timing to be used for initialization.</param>
		public InitializeBehaviorAttribute(InitializeTiming timing)
		{
			this.Timing = timing;
		}
	}
}
