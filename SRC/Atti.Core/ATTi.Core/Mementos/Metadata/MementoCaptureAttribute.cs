using System;

namespace ATTi.Core.Mementos
{
	/// <summary>
	/// For objects that implement their own memento behavior, 
	/// marks a static method for capturing the memento.
	/// </summary>	
	[AttributeUsage(AttributeTargets.Method)]
	public class MementoCaptureAttribute : Attribute
	{
	}
}
