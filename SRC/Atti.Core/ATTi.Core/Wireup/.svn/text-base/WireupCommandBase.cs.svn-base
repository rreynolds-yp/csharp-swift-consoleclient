namespace ATTi.Core.Wireup
{
	/// <summary>
	/// Abstract wireup command.
	/// </summary>
	public abstract class WireupCommand : IWireupCommand
	{
		

		/// <summary>
		/// Executes the wireup command.
		/// </summary>
		void IWireupCommand.Execute()
		{
			PerformWireup();
		}

		/// <summary>
		/// Called by the base class upon execute. Derived classes should 
		/// provide an implementation that performs the wireup logic.
		/// </summary>
		protected abstract void PerformWireup();

		
	}
}