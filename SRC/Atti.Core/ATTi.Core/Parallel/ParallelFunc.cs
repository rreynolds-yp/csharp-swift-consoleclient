using System;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Base class for parallel functions.
	/// </summary>
	public abstract class ParallelFunc<TExecutionStrategy, R> : StrategizedParallelTask<TExecutionStrategy>
		where TExecutionStrategy : ExecutionStrategy, new()
	{
		#region Constructors
		public ParallelFunc() : base() { }
		public ParallelFunc(AsyncCallback callback, object handback) : base(callback, handback) { }
		#endregion

		protected override void HandleExecuteCallback()
		{
			R result = this.ExecuteFunctionCallback();
			SetTaskState(TaskStates.CompletedSuccess);
			((ITaskCompletion<R>)Completion)
				.MarkCompleted(result, false);
		}

		protected override ITaskCompletion TaskCompletionFactory(AsyncCallback callback, object handback)
		{
			return new TaskCompletion<R>(callback, handback, this);
		}
		/// <summary>
		/// Required by abstract base class. Not intended to be called.
		/// </summary>
		protected override void ExecuteCallback()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// New abstract base method. For parallel functions, this method replaces
		/// the ExecuteCallback method.
		/// </summary>
		/// <returns></returns>
		protected abstract R ExecuteFunctionCallback();

		/// <summary>
		/// Gets the result of the function. THIS WILL BLOCK THE CURRENT THREAD UNTIL THE FUNCTION COMPLETES.
		/// </summary>
		public IFuture<R> FutureResult { get { return (IFuture<R>)this.Completion; } }
	}

	/// <summary>
	/// Wrapper class enabling a delegate to participate in the 
	/// asynchronous execution model established by the AsyncJob
	/// base class.
	/// </summary>
	public class ParallelFuncWrapper<TExecutionStrategy, R> : ParallelFunc<TExecutionStrategy, R>
		where TExecutionStrategy : ExecutionStrategy, new()
	{
		Func<R> _function;

		public ParallelFuncWrapper(Func<R> func)
			: base()
		{
			Contracts.Require.IsNotNull("func", func);

			_function = func;
		}
		public ParallelFuncWrapper(Func<R> func, AsyncCallback callback, object handback)
			: base(callback, handback)
		{
			Contracts.Require.IsNotNull("func", func);

			_function = func;
		}

		protected override string GetTaskName()
		{
			return String.Concat(_function.Target.GetType(), '#', _function.Method.Name);
		}

		protected override R ExecuteFunctionCallback()
		{
			return _function();
		}
	}
}
