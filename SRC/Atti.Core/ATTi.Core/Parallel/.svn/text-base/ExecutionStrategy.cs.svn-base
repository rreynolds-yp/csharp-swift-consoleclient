using System.Threading;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Execution strategy base class. Executuion strategies are used to begin parallel work.
	/// </summary>
	public abstract class ExecutionStrategy
	{
		/// <summary>
		/// Executes the callback according to the execution strategy.
		/// </summary>
		/// <param name="waitCallback"></param>
		/// <param name="state"></param>
		/// <returns>An execution key; this object should be considered opaque.</returns>
		public abstract object BeginExecute(WaitCallback waitCallback, object state);
		/// <summary>
		/// Ends the execution of the callback associated with the execution key given.
		/// </summary>
		/// <param name="executionKey">The opaque execution key returned during a prior call to BeginInvoke()</param>
		public abstract void EndExecute(object executionKey);
	}

	/// <summary>
	/// An execution strategy using a new independent thread to execute the job.
	/// </summary>
	public sealed class IndependentThreadExecutionStrategy : ExecutionStrategy
	{
		/// <summary>
		/// Begins execution of the job (waitCallback) in a new thread.
		/// </summary>
		/// <param name="waitCallback">The callback that executes the job.</param>
		/// <param name="state">A state object handed back to the callback when it is invoked.</param>
		/// <returns>An opaque execution key.</returns>
		public override object BeginExecute(WaitCallback waitCallback, object state)
		{
			Thread result = new Thread(new ParameterizedThreadStart(waitCallback));
			result.Start(state);
			return result;
		}
		public override void EndExecute(object executionKey)
		{
			// This check simply ensures that the thread stayed 
			// alive during the execution and that it is completed before the
			// call returns.
			Contracts.Require.IsInstanceOfType<Thread>("executionKey", executionKey);
			Thread th = (Thread)executionKey;
			th.Join();
		}
	}

	public sealed class BackgroundThreadExecutionStrategy : ExecutionStrategy
	{
		public override object BeginExecute(WaitCallback waitCallback, object state)
		{
			Thread result = new Thread(new ParameterizedThreadStart(waitCallback));
			result.IsBackground = true;
			result.Start(state);
			return result;
		}
		public override void EndExecute(object executionKey)
		{
			Contracts.Require.IsInstanceOfType<Thread>("executionKey", executionKey);
			// This check simply ensures that the thread stayed 
			// alive during the execution.
		}
	}

	public sealed class ThreadPoolExecutionStrategy : ExecutionStrategy
	{
		public override object BeginExecute(WaitCallback waitCallback, object state)
		{
			ThreadPool.QueueUserWorkItem(waitCallback, state);
			return null;
		}
		public override void EndExecute(object executionKey)
		{
			/* nothing to do */
		}
	}
}
