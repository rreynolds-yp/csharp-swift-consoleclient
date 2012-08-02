using System;

namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Marks a field so that it is ignored by the memento capture/restore.
	/// </summary>	
	[AttributeUsage(AttributeTargets.Field)]
	public class MementoIgnoreAttribute : Attribute
	{
	}
}
