using System;
using System.Threading;

namespace ATTi.Core.Parallel
{
	public abstract class StrategizedParallelTask<TExecutionStrategy> : ParallelTask
	where TExecutionStrategy : ExecutionStrategy, new()
	{
		private TExecutionStrategy _strategy = new TExecutionStrategy();

		public StrategizedParallelTask()
			: base(null, null)
		{
		}
		public StrategizedParallelTask(AsyncCallback callback, object handback)
			: base(callback, handback)
		{
		}
		protected override object BeginExecutionStrategy(WaitCallback waitCallback, object state)
		{
			return _strategy.BeginExecute(waitCallback, state);
		}
		protected override void EndExecutionStrategy(object executionKey)
		{
			_strategy.EndExecute(executionKey);
		}
	}

}
