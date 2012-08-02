namespace ATTi.Core.Wireup
{
	/// <summary>
	/// Interface for commands executed at wireup time.
	/// </summary>
	public interface IWireupCommand
	{
		/// <summary>
		/// Executes the command.
		/// </summary>
		void Execute();
	}
}