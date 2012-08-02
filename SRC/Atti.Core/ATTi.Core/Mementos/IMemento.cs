
namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Interface for mementos.
	/// </summary>
	public interface IMemento
	{
		/// <summary>
		/// The object from which the memento was captured.
		/// </summary>
		object Target { get; }
		/// <summary>
		/// Indicates whether the memento has been restored.
		/// </summary>
		/// <remarks>Used by the framework to avoid restoring
		/// mementos more than once when there are cycles in the
		/// object graph.</remarks>
		bool IsRestored { get; set; }
	}
}
