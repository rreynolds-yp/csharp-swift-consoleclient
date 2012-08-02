using System.Reflection;



namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Interface for field references.
	/// </summary>
	public interface IFieldRef : IValueRef
	{
		/// <summary>
		/// Gets the FieldInfo for the target field.
		/// </summary>
		/// <returns>FieldInfo metadata for the underlying field.</returns>
		FieldInfo GetFieldInfo();
	}

}
