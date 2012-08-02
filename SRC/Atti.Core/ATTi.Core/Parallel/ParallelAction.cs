using System;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Base class for parallel actions.
	/// </summary>
	public abstract class ParallelAction<TExecutionStrategy> : StrategizedParallelTask<TExecutionStrategy>
		where TExecutionStrategy : ExecutionStrategy, new()
	{
		public ParallelAction()
			: base(null, null)
		{
		}
		public ParallelAction(AsyncCallback callback, object handback)
			: base(callback, handback)
		{
		}
	}

	/// <summary>
	/// Wrapper class enabling a delegate to participate in the 
	/// parallel execution model established by the ParallelTask
	/// base class.
	/// </summary>
	public class ParallelActionWrapper<TExecutionStrategy> : ParallelAction<TExecutionStrategy>
		where TExecutionStrategy : ExecutionStrategy, new()
	{
		Action _proc;

		public ParallelActionWrapper(Action proc)
			: base()
		{
			Contracts.Require.IsNotNull("proc", proc);

			_proc = proc;
		}
		public ParallelActionWrapper(Action proc, AsyncCallback callback, object handback)
			: base(callback, handback)
		{
			Contracts.Require.IsNotNull("proc", proc);

			_proc = proc;
		}
		public ParallelActionWrapper(AsyncCallback callback, object handback)
			: base(callback, handback)
		{
		}
		protected override string GetTaskName()
		{
			return String.Concat(_proc.Target.GetType(), '#', _proc.Method.Name);
		}
		protected override void ExecuteCallback()
		{
			_proc();
		}
	}

}
