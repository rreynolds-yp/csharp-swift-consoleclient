using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Boundary for asynchronous jobs, provides convenience of the using clause 
	/// as a synchronization point.
	/// </summary>
	public sealed class TaskBoundary : IDisposable
	{
		#region Declarations
		private const int StateBoundaryNotCompleted = 0;
		private const int StateBoundaryCompleted = 1;
		[ThreadStatic]
		private static Stack<TaskBoundary> __boundary;

		private sealed class TaskBoundaryCollection : System.Collections.ObjectModel.KeyedCollection<int, TaskBoundary>
		{
			protected override int GetKeyForItem(TaskBoundary item)
			{
				return item._localBoundaryID;
			}
		}
		private sealed class ParallelTaskCollection : System.Collections.ObjectModel.KeyedCollection<int, ParallelTask>
		{
			protected override int GetKeyForItem(ParallelTask item)
			{
				return item.LocalTaskID;
			}
		}

		private static int __LocalBoundaryIDSeed = 0;

		private readonly int _localBoundaryID;
		private volatile TaskBoundary _parent;
		private object _children;
		private object _jobs;
		private object _asyncResults;
		private object _waitHandles;
		private volatile bool _disposed;
		private int _completed = StateBoundaryNotCompleted;

		#endregion

		#region Constructors
		public TaskBoundary()
		{
			_localBoundaryID = Interlocked.Increment(ref __LocalBoundaryIDSeed);

			if (__boundary == null) __boundary = new Stack<TaskBoundary>();
			else if (__boundary.Count > 0) __boundary.Peek().AddChild(this);
			__boundary.Push(this);
		}
		#endregion

		internal static void SetRootBoundaryForThread(TaskBoundary boundary)
		{
			if (boundary == null)
			{
				Debug.Assert(__boundary != null, "Root TaskBoundary not established for thread");
				Debug.Assert(__boundary.Count == 1, "TaskBoundary scope missmatch");
				__boundary.Pop(); __boundary = null;
			}
			else
			{
				if (__boundary == null) __boundary = new Stack<TaskBoundary>();
				Debug.Assert(__boundary.Count == 0, "Root TaskBoundary already set for thread");
				__boundary.Push(boundary);
			}
		}

		internal static TaskBoundary AddTask(ParallelTask job)
		{
			if (__boundary != null && __boundary.Count > 0)
			{
				TaskBoundary result = __boundary.Peek();
				result.Add(job);
				return result;
			}
			else return null;
		}

		internal static void AddAsyncResult(IAsyncResult ar)
		{
			if (__boundary != null && __boundary.Count > 0)
			{
				TaskBoundary boundary = __boundary.Peek();
				if (ar.AsyncState is ParallelTask
					&& boundary.FindTask((ParallelTask)ar.AsyncState))
					return;

				boundary.Add(ar);
			}
		}

		private bool FindTask(ParallelTask job)
		{
			Contracts.Invariant.AssertState(!_disposed, () => String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			ParallelTaskCollection jobsCollection = (ParallelTaskCollection)Thread.VolatileRead(ref _jobs);
			if (jobsCollection != null)
			{
				lock (jobsCollection)
				{
					if (jobsCollection.Contains(job))
						return true;
				}
			}

			TaskBoundaryCollection childCollection = (TaskBoundaryCollection)Thread.VolatileRead(ref _children);
			if (childCollection != null)
			{
				lock (childCollection)
				{
					foreach (TaskBoundary child in childCollection)
					{
						if (child.FindTask(job))
							return true;
					}
				}
			}
			return false;
		}

		private void Add(WaitHandle waitHandle)
		{
			if (_disposed) throw new ObjectDisposedException(String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			List<WaitHandle> handles = Util
				.NonBlockingLazyInitializeVolatile<List<WaitHandle>>(ref _waitHandles);

			lock (handles)
			{
				handles.Add(waitHandle);
			}
		}

		private void Add(IAsyncResult ar)
		{
			if (_disposed) throw new ObjectDisposedException(String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			List<IAsyncResult> asyncResults = Util
				.NonBlockingLazyInitializeVolatile<List<IAsyncResult>>(ref _asyncResults);

			lock (asyncResults)
			{
				asyncResults.Add(ar);
			}
		}

		private void Add(ParallelTask job)
		{
			if (_disposed) throw new ObjectDisposedException(String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			ParallelTaskCollection jobsCollection = Util
				.NonBlockingLazyInitializeVolatile<ParallelTaskCollection>(ref _jobs);

			lock (jobsCollection)
			{
				jobsCollection.Add(job);
			}
		}

		private void AddChild(TaskBoundary child)
		{
			if (_disposed) throw new ObjectDisposedException(String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			Debug.Assert(child != null, "Child must be given");

			TaskBoundaryCollection childCollection = Util
				.NonBlockingLazyInitializeVolatile<TaskBoundaryCollection>(ref _children);

			child.Parent = this;
			lock (childCollection)
			{
				childCollection.Add(child);
			}
		}

		private void RemoveChild(TaskBoundary child)
		{
			if (_disposed) throw new ObjectDisposedException(String.Format(Properties.Resources.TaskBoundaryName, _localBoundaryID));

			TaskBoundaryCollection childCollection = (TaskBoundaryCollection)Thread.VolatileRead(ref _children);
			if (childCollection != null)
			{
				bool removed = childCollection.Remove(child);
				Debug.Assert(removed, "Invalid child");
			}
		}

		private TaskBoundary Parent
		{
			get { return _parent; }
			set
			{
				Debug.Assert(_parent == null, "Parent boundary already set");
				_parent = value;
			}
		}

		#region IDisposable Members

		~TaskBoundary()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
			_disposed = true;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				List<WaitHandle> waitHandles = new List<WaitHandle>();

				bool completed = Thread.VolatileRead(ref _completed) == StateBoundaryCompleted;
				this.GetAllNonCompletedWaitHandles(waitHandles, completed);

				if (completed)
				{
					while (waitHandles.Count > 0)
					{
						if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
						{
							WaitHandle[] finalListOfHandles = waitHandles.Take(64).ToArray();
							WaitHandle.WaitAll(finalListOfHandles);
							waitHandles = new List<WaitHandle>(waitHandles.Except(finalListOfHandles));
						}
						else
						{
							foreach (WaitHandle h in waitHandles)
							{
								h.WaitOne();
							}
							waitHandles.Clear();
						}
					}
				}
			}
			if (_parent != null)
			{
				_parent.RemoveChild(this);
			}
			TaskBoundary poptop = __boundary.Pop();
			Debug.Assert(poptop == this, "TaskBoundary scope mismatch");
		}

		private void GetAllNonCompletedWaitHandles(List<WaitHandle> waitHandles, bool completed)
		{
			ParallelTaskCollection jobCollection = (ParallelTaskCollection)Thread.VolatileRead(ref _jobs);
			TaskBoundaryCollection childCollection = (TaskBoundaryCollection)Thread.VolatileRead(ref _children);
			List<IAsyncResult> asyncResults = (List<IAsyncResult>)Thread.VolatileRead(ref _asyncResults);
			List<WaitHandle> handles = (List<WaitHandle>)Thread.VolatileRead(ref _waitHandles);
			if (childCollection != null)
			{
				lock (childCollection)
				{
					foreach (TaskBoundary child in childCollection)
					{
						child.GetAllNonCompletedWaitHandles(waitHandles, completed);
					}
				}
			}

			if (jobCollection != null)
			{
				lock (jobCollection)
				{
					waitHandles.AddRange(from job in jobCollection
															 where (completed && job.Cancel()) || job.IsCompleted == false
															 select job.WaitHandle);
					//foreach (ParallelTask job in jobCollection)
					//{
					//  if (!completed)
					//    job.Cancel();
					//  else if (!job.IsCompleted)
					//    waitHandles.Add(job.WaitHandle);
					//}
				}
			}
			if (completed)
			{
				if (asyncResults != null)
				{
					lock (asyncResults)
					{
						waitHandles.AddRange(from item in asyncResults
																 where item.IsCompleted == false
																 select item.AsyncWaitHandle);
					}
				}

				if (handles != null)
				{
					lock (handles)
					{
						waitHandles.AddRange(handles);
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Communicates that the boundary reached a normal completion and 
		/// should wait for all jobs to complete.
		/// </summary>
		public void MarkCompleted()
		{
			Thread.VolatileWrite(ref _completed, StateBoundaryCompleted);
		}
	}

}
