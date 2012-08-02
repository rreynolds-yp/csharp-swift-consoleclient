using System;
using System.Threading;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Basic implementation of the ITaskCompletion interface.
	/// </summary>
	public class TaskCompletion : ITaskCompletion, IDisposable
	{
		#region Declarations

		private const Int32 StatePending = 0;
		private const Int32 StateCompletingNow = 1;
		private const Int32 StateCompletedSynchronously = 2;
		private const Int32 StateCompletedAsynchronously = 3;

		private readonly DateTime _startTime;
		private DateTime _completeTime;
		private readonly AsyncCallback _asyncCallback;
		private readonly Object _asyncHandback;
		private Object _asyncState;

		private int _completedState = StatePending;

		private ManualResetEvent _asyncWaitHandle;
		private Exception _exceptionResult;
		private bool _disposed = false;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public TaskCompletion()
			: this(null, null, null)
		{
		}

		/// <summary>
		/// Creates a new instance and initializes the AsyncCallback.
		/// </summary>
		/// <param name="asyncCallback">A delegate to be called when the async operation completes.</param>
		/// <param name="asyncHandback">A handback object passed to the AsyncCallback when the operation completes.</param>
		public TaskCompletion(AsyncCallback asyncCallback, Object asyncHandback)
			: this(asyncCallback, asyncHandback, null)
		{
		}

		/// <summary>
		/// Creates a new instance and initializes the AsyncCallback.
		/// </summary>
		/// <param name="asyncCallback">A delegate to be called when the async operation completes.</param>
		/// <param name="asyncHandback">A handback object passed to the AsyncCallback when the operation completes.</param>
		/// <param name="asyncState">A state object for use as a handback for the creator.</param>
		public TaskCompletion(AsyncCallback asyncCallback, Object asyncHandback, Object asyncState)
		{
			_asyncCallback = asyncCallback;
			_asyncHandback = asyncHandback;
			_asyncState = asyncState;
			_startTime = DateTime.UtcNow;
			TaskBoundary.AddAsyncResult(this);
		}
		#endregion

		#region IAsyncResult implementation
		public Object AsyncState { get { return _asyncState; } }

		public bool CompletedSynchronously
		{
			get
			{
				return Thread.VolatileRead(ref _completedState) == StateCompletedSynchronously;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (_disposed) throw new ObjectDisposedException(this.GetType().Name);
				if (_asyncWaitHandle == null)
				{
					Boolean done = IsCompleted;
					ManualResetEvent evt = new ManualResetEvent(done);
					if (Interlocked.CompareExchange(ref _asyncWaitHandle,
						 evt, null) != null)
					{
						// Another thread beat us too it, dispose the event...
						evt.Close();
					}
					else
					{
						if (!done && IsCompleted)
						{
							// If the operation was completed during signal creation, set the signal...							
							_asyncWaitHandle.Set();
						}
					}
				}
				return _asyncWaitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return Thread.VolatileRead(ref _completedState) > StateCompletingNow;
			}
		}
		#endregion

		#region ITaskCompletion Members

		void ITaskCompletion.MarkCompleted(bool completedSyncronously)
		{
			PerformMarkCompleted<TaskCompletion>(this, null, completedSyncronously);
		}

		void ITaskCompletion.MarkException(Exception ex, bool completedSynchronously)
		{
			PerformMarkCompleted<TaskCompletion>(this, tthis => { tthis._exceptionResult = ex; }, completedSynchronously);
		}

		void ITaskCompletion.EndInvoke()
		{
			if (!this.IsCompleted)
			{
				bool signalReceived = this.AsyncWaitHandle.WaitOne();
				// The following field refs are guaranteed by the getter above.
				_asyncWaitHandle.Close();
				Util.Dispose<ManualResetEvent>(ref _asyncWaitHandle);
				if (!signalReceived) throw new ParallelTimeoutException();
			}
			// If an exception occured, rethrow...
			if (_exceptionResult != null) throw _exceptionResult;
		}

		void ITaskCompletion.EndInvoke(TimeSpan timeout, bool exitContext)
		{
			if (!this.IsCompleted)
			{
				bool signalReceived = this.AsyncWaitHandle.WaitOne(timeout, exitContext);
				// The following field refs are guaranteed by the getter above.
				_asyncWaitHandle.Close();
				Util.Dispose<ManualResetEvent>(ref _asyncWaitHandle);
				if (!signalReceived) throw new ParallelTimeoutException();
			}
			// If an exception occured, rethrow...
			if (_exceptionResult != null) throw _exceptionResult;
		}

		void ITaskCompletion.EndInvoke(int millisecondsTimeout, bool exitContext)
		{
			if (!this.IsCompleted)
			{
				bool signalReceived = this.AsyncWaitHandle.WaitOne(millisecondsTimeout, exitContext);
				// The following field refs are guaranteed by the getter above.
				_asyncWaitHandle.Close();
				Util.Dispose<ManualResetEvent>(ref _asyncWaitHandle);
				if (!signalReceived) throw new ParallelTimeoutException();
			}
			// If an exception occured, rethrow...
			if (_exceptionResult != null) throw _exceptionResult;
		}

		#endregion

		#region IDisposable Members

		~TaskCompletion()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_asyncWaitHandle != null)
				Util.Dispose<ManualResetEvent>(ref this._asyncWaitHandle);
			if (disposing) _disposed = true;
		}

		#endregion

		#region Utility methods
		protected static void PerformMarkCompleted<TReceiver>(TReceiver r, Action<TReceiver> setter, bool completedSynchronously)
			where TReceiver : TaskCompletion
		{
			int prevState = Interlocked.Exchange(
							ref r._completedState, StateCompletingNow
							);
			if (prevState != StatePending)
				throw new InvalidOperationException(Properties.Resources.Error_AsyncResultAlreadySet);

			if (setter != null) setter(r);
			r._completeTime = DateTime.UtcNow;
			Interlocked.Exchange(
							ref r._completedState,
							completedSynchronously ? StateCompletedSynchronously : StateCompletedAsynchronously
							);

			if (r._asyncWaitHandle != null) r._asyncWaitHandle.Set();
			if (r._asyncCallback != null) r._asyncCallback(r);
		}
		#endregion
	}

	public class TaskCompletion<R> : TaskCompletion, ITaskCompletion<R>
	{
		#region Declarations
		private R _result;
		#endregion

		#region Constructors
		public TaskCompletion()
			: base()
		{
		}

		public TaskCompletion(AsyncCallback asyncCallback, Object asyncHandback)
			: base(asyncCallback, asyncHandback, null)
		{
		}

		public TaskCompletion(AsyncCallback asyncCallback, Object asyncHandback, Object asyncState)
			: base(asyncCallback, asyncHandback, asyncState)
		{
		}
		#endregion

		#region ITaskCompletion<R> Members

		void ITaskCompletion<R>.MarkCompleted(R result, bool completedSyncronously)
		{
			PerformMarkCompleted(this, tthis => { tthis._result = result; }, completedSyncronously);
		}

		#endregion

		#region IFuture<R> Members

		bool IFuture<R>.HasValue { get { return this.IsCompleted; } }

		R IFuture<R>.Value
		{
			get
			{
				return WaitForValue();
			}
		}

		public bool TryReadValue(out R value)
		{
			if (this.IsCompleted)
			{
				((ITaskCompletion)this).EndInvoke();
				value = _result;
				return true;
			}

			value = default(R);
			return false;
		}

		public bool TryReadValue(int millisecondsTimeout, out R value)
		{
			if (millisecondsTimeout == 0) return TryReadValue(out value);
			else return TryReadValue(TimeSpan.FromMilliseconds(millisecondsTimeout), out value);
		}

		public bool TryReadValue(TimeSpan timeout, out R value)
		{
			long ticks = DateTime.Now.Ticks;
			long timeoutExpiration = ticks + timeout.Ticks;
			long remainingTicks;

			if (this.IsCompleted)
			{
				((ITaskCompletion)this).EndInvoke();
				value = _result;
				return true;
			}
			else
			{
				WaitHandle wait = this.AsyncWaitHandle;
				remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
				do
				{
					if (remainingTicks > 0
						&& wait.WaitOne(new TimeSpan(remainingTicks))
						&& this.IsCompleted)
					{
						((ITaskCompletion)this).EndInvoke();
						value = _result;
						return true;
					}
					remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
				} while (remainingTicks > 0);
			}

			value = default(R);
			return false;
		}

		public R WaitForValue()
		{
			if (!this.IsCompleted)
			{
				this.AsyncWaitHandle.WaitOne();
			}
			((ITaskCompletion)this).EndInvoke();
			return _result;
		}

		public R WaitForValue(int millisecondsTimeout)
		{
			if (millisecondsTimeout == 0) return WaitForValue();
			else return WaitForValue(TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		public R WaitForValue(TimeSpan timeout)
		{
			long ticks = DateTime.Now.Ticks;
			long timeoutExpiration = ticks + timeout.Ticks;
			long remainingTicks;

			if (this.IsCompleted)
			{
				((ITaskCompletion)this).EndInvoke();
				return _result;
			}
			else
			{
				WaitHandle wait = this.AsyncWaitHandle;
				remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
				do
				{
					if (remainingTicks > 0
						&& wait.WaitOne(new TimeSpan(remainingTicks))
						&& this.IsCompleted)
					{
						((ITaskCompletion)this).EndInvoke();
						return _result;
					}
					remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
				} while (remainingTicks > 0);
			}

			throw new ParallelTimeoutException();
		}

		#endregion
	}
}
