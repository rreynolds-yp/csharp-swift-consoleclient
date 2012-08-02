
namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Represents a context for mementos during capture/restore.
	/// </summary>
	/// <remarks>
	/// The memento framework uses the context as a mechanism for avoiding
	/// cycles when capturing mementos. As mementos are captured they are
	/// placed into the context. As objects are encountered in the graph, the
	/// framework checks the context for an existing memento and if found, uses
	/// that memento. In this way duplicates are also avoided within a MemoryBoundary.
	/// </remarks>
	public interface IMementoContext
	{
		/// <summary>
		/// Tries to get a memento for the given item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="result">A reference to receive the memento.</param>
		/// <returns><em>true</em> if a memento was retrieved for the given item; otherwise <em>false</em>.
		/// If <em>false</em> then <paramref name="result"/>result</paramref> is set to false.</returns>
		bool TryGetMemento(object item, out IMemento result);
		/// <summary>
		/// Puts a memento into the context for the item given.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="m">A memento containing the internal state of the item given.</param>
		/// <returns>The memento.</returns>
		IMemento PutMemento(object item, IMemento m);
	}
}
