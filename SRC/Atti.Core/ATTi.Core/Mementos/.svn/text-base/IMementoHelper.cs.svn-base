
namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Helper class for creating mementos for target type T.
	/// </summary>
	/// <typeparam name="T">target type T</typeparam>
	public interface IMementoHelper<T>
	{
		/// <summary>
		/// Captures a memento of target type T.
		/// </summary>
		/// <param name="ctx">The memento context.</param>
		/// <param name="item">The item being captured.</param>
		/// <returns>A memento containing the captured internal state of <paramref name="item"/></returns>
		IMemento CaptureMemento(IMementoContext ctx, T item);
		/// <summary>
		/// Restores an item to its captured state (from the memento given).
		/// </summary>
		/// <param name="ctx">The memento context.</param>
		/// <param name="item">A reference to the item being restored.</param>
		/// <param name="m">The memento containing the item's captured state.</param>
		void RestoreMemento(IMementoContext ctx, ref T item, IMemento m);
	}
}
