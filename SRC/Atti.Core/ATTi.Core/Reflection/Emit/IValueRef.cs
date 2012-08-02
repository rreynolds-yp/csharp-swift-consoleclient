
namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Interface for objects that have a value that can be loaded on the stack.
	/// </summary>
	public interface IValueRef
	{
		/// <summary>
		/// Name of the value.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Loads the value by pushing it onto the stack.
		/// </summary>
		/// <param name="il">An IL helper for loading the value.</param>
		void LoadValue(ILHelper il);
		/// <summary>
		/// Stores the value by popping it off of the stack.
		/// </summary>
		/// <param name="il">IL Helper for storing the value.</param>
		void StoreValue(ILHelper il);
		/// <summary>
		/// Loads the address of the value by pushing it onto the stack.
		/// </summary>
		/// <param name="il">An IL helper for loading the address.</param>
		void LoadAddress(ILHelper il);
	}
}
