using System;
using System.Diagnostics;
using System.Threading;
using ATTi.Core.Trace;
using ATTi.Core.Contracts;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Base class for parallel tasks
	/// </summary>
	public abstract class ParallelTask: ITraceable
	{
		#region Declarations

		private Status<TaskStates> _taskState = new Status<TaskStates>(TaskStates.Idle);
		private object _dependencyHandle;
		private ITaskCompletion _completion;
		private string _name;
		private object _executionKey;

		private AsyncCallback _callback;
		private object _handback;

		private static int __localTaskID = 0;
		private readonly int _localTaskID = Interlocked.Increment(ref ParallelTask.__localTaskID);
		private readonly TaskBoundary _boundary;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new ParallelTask.
		/// </summary>
		public ParallelTask()
			: this(null, null)
		{
		}
		public ParallelTask(AsyncCallback callback, object handback)
		{
			_callback = callback;
			_handback = handback;
			_boundary = TaskBoundary.AddTask(this);
		}
		#endregion

		/// <summary>
		/// Gets the name of the task. Names are intented
		/// to uniquely identify a task.
		/// </summary>
		public string Name
		{
			get
			{
				if (_name == null)
				{
					_name = this.GetTaskName();
				}
				return _name;
			}
		}
		/// <summary>
		/// Virtual method used by the Name property. Provided for
		/// specialization by subclasses.
		/// </summary>
		/// <returns></returns>
		protected virtual string GetTaskName()
		{
			return String.Format("{0} #{1}", this.GetType().FullName, _localTaskID);
		}
		/// <summary>
		/// Gets the local task identity. Used by the framework.
		/// </summary>
		internal int LocalTaskID { get { return _localTaskID; } }

		/// <summary>
		/// Indicates whether the task is idle. Tasks are idle from the time they are created until
		/// they are scheduled for execution or queued in the thread pool.
		/// </summary>
		public bool IsIdle { get { return _taskState.HasState(TaskStates.Idle); } }
		/// <summary>
		/// Indicates whether the task is pending execution either by virtue of being scheduled or
		/// being queued in the thread pool.
		/// </summary>
		public bool IsPending { get { return _taskState.HasState(TaskStates.Pending); } }
		/// <summary>
		/// Indicates whether a task is scheduled for execution. A scheduled task is one that is dependent
		/// on another task's completion, or a wait handle's becoming signalled.
		/// </summary>
		public bool IsScheduled { get { return _taskState.HasState(TaskStates.PendingScheduled); } }
		/// <summary>
		/// Indicates whether a task is canceling. Tasks that have not begun execution may be canceled.
		/// </summary>
		public bool IsCanceling { get { return _taskState.HasState(TaskStates.PendingCancel); } }
		/// <summary>
		/// Indicates whether the task is executing.
		/// </summary>
		public bool IsExecuting { get { return _taskState.HasState(TaskStates.Executing); } }
		/// <summary>
		/// Indicates whether the task is yielding. Tasks that are yielding become scheduled pending
		/// the completion of another task or a wait handle's becoming signalled.
		/// </summary>
		public bool IsYielding { get { return _taskState.HasState(TaskStates.Yielding); } }
		/// <summary>
		/// Indicates whether the task has completed. Completion DOES NOT mean success! It is the
		/// responsibility of the caller to check IsCompletedSuccess, HasError, and/or IsCanceled
		/// to determine the nature of the completion.
		/// </summary>
		public bool IsCompleted { get { return _taskState.HasState(TaskStates.Completed); } }
		/// <summary>
		/// Indicates whether the task has completed successfully.
		/// </summary>
		public bool IsCompletedSuccess { get { return _taskState.HasState(TaskStates.CompletedSuccess); } }
		/// <summary>
		/// Indicates whether the task completed with an error.
		/// </summary>
		public bool HasError { get { return _taskState.HasState(TaskStates.CompletedWithError); } }
		/// <summary>
		/// Indicates whether the task was canceled.
		/// </summary>
		public bool IsCanceled { get { return _taskState.HasState(TaskStates.CompletedCanceled); } }
		/// <summary>
		/// Cancels the task if it has not begun execution.
		/// </summary>
		/// <returns><em>true</em> if the task was marked as canceling before it began execution; otherwise <em>false</em>.</returns>
		public bool Cancel()
		{			
			return _taskState.CompareExchange(TaskStates.PendingCancel, TaskStates.Pending) == TaskStates.Pending;
		}
		/// <summary>
		/// Gets a wait handle that will become signaled upon the task's completion.
		/// </summary>
		public WaitHandle WaitHandle
		{
			get { return Completion.AsyncWaitHandle; }
		}
		/// <summary>
		/// Gets the IAsycnResult associated with the task.
		/// </summary>
		public IAsyncResult AsyncResult
		{
			get { return Completion; }
		}
		/// <summary>
		/// Schedules execution pending the completion of another task. This call uses overlapped
		/// I/O for scheduling.
		/// </summary>
		/// <param name="waitForTask">A task upon which this task is dependent. If null is given then
		/// this task is queued to the thread pool for immediate asynchronous execution.</param>
		/// <param name="timeout">A timeout period for the wait. If the task does not start before the
		/// timeout period then a TimeoutException will be thrown when the IAsyncResult is read. The
		/// value of Timeout.Infinite may be used to wait forever.</param>
		public void ScheduledExecute(ParallelTask waitForTask, TimeSpan timeout)
		{
			if (waitForTask == null || waitForTask.IsCompleted) AsyncExecute();
			else this.ScheduledExecute(waitForTask.WaitHandle, timeout);
		}
		/// <summary>
		/// Schedules execution pending the completion of another task. This call uses overlapped
		/// I/O for scheduling.
		/// </summary>
		/// <param name="waitForTask">A task upon which this task is dependent. If null is given then
		/// this task is queued to the thread pool for immediate asynchronous execution.</param>
		/// <param name="timeoutMilliseconds">A timeout period for the wait. If the task does not start before the
		/// timeout period then a TimeoutException will be thrown when the IAsyncResult is read. The
		/// value of Timeout.Infinite may be used to wait forever.</param>
		public void ScheduledExecute(ParallelTask waitForTask, int timeoutMilliseconds)
		{
			if (waitForTask == null || waitForTask.IsCompleted) AsyncExecute();
			else this.ScheduledExecute(waitForTask.WaitHandle, timeoutMilliseconds);
		}
		/// <summary>
		/// Schedules an ParallelTask to run when another WaitHandle is signaled.
		/// </summary>
		/// <param name="waitHandle">The handle that will be signaled when the task is ready to run.</param>
		/// <param name="timeout">A timeout period for the wait. If the task does not start before the
		/// timeout period then a TimeoutException will be thrown when the IAsyncResult is read. The
		/// value of Timeout.Infinite may be used to wait forever.</param>
		public void ScheduledExecute(WaitHandle waitHandle, TimeSpan timeout)
		{
			Invariant.AssertState(_taskState.CurrentState == TaskStates.Idle, "TaskState must be idle");

			if (waitHandle == null) AsyncExecute();
			else
			{
				SetTaskState(TaskStates.PendingScheduled);
				_dependencyHandle = ThreadPool.RegisterWaitForSingleObject(waitHandle, new WaitOrTimerCallback(this.HandleScheduledExecution), null, timeout, true);
			}
		}
		/// <summary>
		/// Schedules an ParallelTask to run when another WaitHandle is signaled.
		/// </summary>
		/// <param name="waitHandle">The handle that will be signaled when the task is ready to run.</param>
		/// <param name="timeout">A timeout period for the wait. If the task does not start before the
		/// timeout period then a TimeoutException will be thrown when the IAsyncResult is read. The
		/// value of Timeout.Infinite may be used to wait forever.</param>
		public void ScheduledExecute(WaitHandle waitHandle, int timeoutMilliseconds)
		{
			AssertState(TaskStates.Idle);
			if (waitHandle == null) AsyncExecute();
			else
			{
				SetTaskState(TaskStates.PendingScheduled);
				_dependencyHandle = ThreadPool.RegisterWaitForSingleObject(waitHandle, new WaitOrTimerCallback(this.HandleScheduledExecution), null, timeoutMilliseconds, true);
			}
		}
		/// <summary>
		/// Begins asynchronous execution of the task.
		/// </summary>
		public void AsyncExecute()
		{
			AssertState(TaskStates.Idle);
			SetTaskState(TaskStates.Pending);
			_executionKey = BeginExecutionStrategy(new WaitCallback(this.HandleAsyncExecution), null);
		}

		/// <summary>
		/// Abstract method implemented by subclasses to specialize how a task begins execution.
		/// </summary>
		/// <param name="waitCallback">A delegate that holds the logic of the task to be executed.</param>
		/// <param name="state">State object for the delegate.</param>
		/// <returns>An opaque object used as an execution key. The abstract base class uses this
		/// object as an execution key and will hand it back to the implementing class in a call
		/// to EndExecutionStrategy when the execution is complete.</returns>
		protected abstract object BeginExecutionStrategy(WaitCallback waitCallback, object state);
		protected abstract void EndExecutionStrategy(object executionKey);


		/// <summary>
		/// Callback method for tasks that are scheduled to run when a WaitHandle is signaled.
		/// </summary>
		/// <param name="unusedState">Unused state object; will be null.</param>
		/// <param name="timedOut">Indicates whether the timeout expired before the
		/// WaitHandle became signaled.</param>
		private void HandleScheduledExecution(object unusedState, bool timedOut)
		{
			if (_boundary != null) TaskBoundary.SetRootBoundaryForThread(_boundary);
			try
			{
				if (IsCanceling)
				{
					SetTaskState(TaskStates.CompletedCanceled);
					Completion.MarkCompleted(false);
				}
				else if (timedOut)
				{
					SetTaskState(TaskStates.CompletedWithError);
					Completion.MarkException(new ParallelTimeoutException(), false);
				}
				else
				{
					this.HandleAsyncExecutionLogic();
				}
			}
			finally
			{
				if (_boundary != null) TaskBoundary.SetRootBoundaryForThread(null);
			}
		}
		/// <summary>
		/// Callback method called when the task is ready to run. 
		/// WARNING: Except in rare scheduling cases, this method is called
		/// from within a background threadpool thread!
		/// </summary>
		/// <param name="unusedState"></param>
		private void HandleAsyncExecution(object unusedState)
		{
			if (_boundary != null) TaskBoundary.SetRootBoundaryForThread(_boundary);
			try
			{
				this.HandleAsyncExecutionLogic();
			}
			finally
			{
				if (_boundary != null) TaskBoundary.SetRootBoundaryForThread(null);
			}
		}
		/// <summary>
		/// Performs the asynchronous execution logic and manages (most) state transitions.
		/// </summary>
		private void HandleAsyncExecutionLogic()
		{
			if (IsCanceling)
			{
				SetTaskState(TaskStates.CompletedCanceled);
				this.TraceData(TraceEventType.Information, () => { return new object[] { String.Format("ParallelTask canceled: {0} #{1}", this.Name, this.LocalTaskID) }; });
				Completion.MarkCompleted(false);
			}
			else
			{
				this.TraceData(TraceEventType.Information, () => { return new object[] { String.Format("ParallelTask executing: {0} #{1}", this.Name, this.LocalTaskID) }; });
				SetTaskState(TaskStates.Executing);
				try
				{
					this.HandleExecuteCallback();
					this.TraceData(TraceEventType.Information, () => { return new object[] { String.Format("ParallelTask completed: {0} #{1}", this.Name, this.LocalTaskID) }; });
				}
				catch (Exception e)
				{
					SetTaskState(TaskStates.CompletedWithError);
					this.TraceData(TraceEventType.Warning, () => { return new object[] { String.Format("ParallelTask completed with error: {0} #{1}", this.Name, this.LocalTaskID), e }; });
					Completion.MarkException(new ParallelException(Properties.Resources.Error_TaskUnhandledException, e), false);
				}
			}
			if (_executionKey != null) // Null if scheduled execution
				EndExecutionStrategy(_executionKey);
		}

		/// <summary>
		/// Outer execution callback method. A derived class may override this 
		/// behavior to provide custom completion information. 
		/// For example, see the AsyncFunction<> derived class.
		/// </summary>
		protected virtual void HandleExecuteCallback()
		{
			this.ExecuteCallback();
			SetTaskState(TaskStates.CompletedSuccess);
			Completion.MarkCompleted(false);
		}
		/// <summary>
		/// Inner execution callback method. Concrete derived classes must
		/// override this method to provide a task's logic.
		/// For example, see the AsyncMethod or AsyncFunction<> derived classes.
		/// </summary>
		protected abstract void ExecuteCallback();
		/// <summary>
		/// Waits for the task to complete. This call will block the calling
		/// thread (potentially for infinity) until the task completes. It
		/// is the caller's responsibility to check the type of completion
		/// by checking the IsCompleted, IsCompletedSuccess, HasError, and 
		/// IsCanceled properties of this task.
		/// </summary>
		public void WaitForCompletion()
		{
			Completion.EndInvoke();
		}

		/// <summary>
		/// Waits for the task to complete. This call will block the calling
		/// thread until the timeout expires or the task completes. If timeout
		/// is Timeout.Infinite then the calling thread may block indefinitely.
		/// It is the caller's responsibility to check the type of completion
		/// by checking the IsCompleted, IsCompletedSuccess, HasError, and 
		/// IsCanceled properties of this task.
		/// </summary>
		/// <param name="timeout">The maximum time to wait for completion.</param>
		/// <param name="exitContext">Indicates whether the task should release
		/// synchronization locks in the current ExecutionContext before entering
		/// the wait/sleep/join state. See WaitHandle.WaitOne for more info.</param>
		public void WaitForCompletion(TimeSpan timeout, bool exitContext)
		{
			Completion.EndInvoke(timeout, exitContext);
		}

		/// Waits for the task to complete. This call will block the calling
		/// thread until the timeout expires or the task completes. If timeout
		/// is Timeout.Infinite then the calling thread may block indefinitely.
		/// It is the caller's responsibility to check the type of completion
		/// by checking the IsCompleted, IsCompletedSuccess, HasError, and 
		/// IsCanceled properties of this task.
		/// </summary>
		/// <param name="timeout">The maximum time to wait for completion.</param>
		/// <param name="exitContext">Indicates whether the task should release
		/// synchronization locks in the current ExecutionContext before entering
		/// the wait/sleep/join state. See WaitHandle.WaitOne for more info.</param>
		public void WaitForCompletion(int millisecondsTimeout, bool exitContext)
		{
			Completion.EndInvoke(millisecondsTimeout, exitContext);
		}
		
		/// <summary>
		/// Asserts that the task is in a particular state.
		/// </summary>
		/// <param name="state"></param>
		/// <exception cref="AsyncTaskStateExcepiton">thrown if the task is not in
		/// one of the expected task states</exception>
		protected void AssertState(params TaskStates[] expected)
		{
			Contracts.Require.AtLeastOneParam<TaskStates>("expected", expected);

			TaskStates current = _taskState.CurrentState;
			switch (expected.Length)
			{
				case 1:
					if (current == expected[0]) return;
					break;
				case 2:
					if (current == expected[0] || current == expected[1])
						return;
					break;
				case 3:
					if (current == expected[0] || current == expected[1] || current == expected[2])
						return;
					break;
				case 4:
					if (current == expected[0] || current == expected[1] || current == expected[2] || current == expected[3])
						return;
					break;
				case 5:
					if (current == expected[0] || current == expected[1] || current == expected[2] || current == expected[3] || current == expected[4])
						return;
					break;
				default:
					if (current == expected[0] || current == expected[1] || current == expected[2] || current == expected[3] || current == expected[4] || current == expected[5])
						return;
					for (int i = 6; i < expected.Length; ++i)
					{
						if (current == expected[i]) return;
					}
					break;
			}

			throw new TaskStateInvalidException(current, expected);
		}
		/// <summary>
		/// Gets a completion object for the task.
		/// </summary>
		protected ITaskCompletion Completion
		{
			get
			{
				return Util.NonBlockingLazyInitializeVolatile<ITaskCompletion>(ref _completion,
					() => { return TaskCompletionFactory(_callback, _handback); });
			}
		}
		/// <summary>
		/// Creates a new completion object for the task. This method may be
		/// overriden in derived classes that have special completion needs
		/// such as a completion result.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="handback"></param>
		/// <returns></returns>
		protected virtual ITaskCompletion TaskCompletionFactory(AsyncCallback callback, object handback)
		{
			return new TaskCompletion(callback, handback, this);
		}
		/// <summary>
		/// Performs a state transition on the task.
		/// </summary>
		/// <param name="state">The task's new state.</param>
		/// <exception cref="TaskStateInvalidException">
		/// thrown if the task is in a state that makes the state transition
		/// invalid.</exception>
		protected void SetTaskState(TaskStates state)
		{
			switch (state)
			{
				case TaskStates.Pending:
					AssertState(TaskStates.Idle);
					break;
				case TaskStates.PendingScheduled:
					AssertState(TaskStates.Idle);
					break;
				case TaskStates.PendingCancel:
				case TaskStates.Executing:
					AssertState(TaskStates.Pending, TaskStates.PendingScheduled);
					break;
				case TaskStates.Yielding:
				case TaskStates.Completed:
				case TaskStates.CompletedSuccess:
					AssertState(TaskStates.Executing);
					break;
				case TaskStates.CompletedWithError:
					AssertState(TaskStates.PendingScheduled, TaskStates.Yielding, TaskStates.Executing);
					break;
				case TaskStates.CompletedCanceled:
					AssertState(TaskStates.PendingCancel);
					break;
				case TaskStates.Idle:
				default:
					throw new ArgumentOutOfRangeException();
			}
			_taskState.SetState(state);
		}
	}


}
