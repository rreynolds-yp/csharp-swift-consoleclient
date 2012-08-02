namespace ATTi.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;

	using ATTi.Core.Trace;
	using ATTi.Core.Properties;

	/// <summary>
	/// Utiltiy class for collecting actions and disposable items for cleanup. Actions and 
	/// disposable items are collected and at cleanup will be either disposed (IDisposables) 
	/// or invoked (Actions) in the reverse order in which they are added to the scope.
	/// </summary>
	public sealed class CleanupScope : IDisposable, ITraceable
	{
		

		readonly Stack<StackItem> _items = new Stack<StackItem>();

		const int Status_Disposed = -2;
		const int Status_Disposing = -1;
		const int Status_None = 0;
		const int Status_Writing = 1;

		int _disposers = 1; // Start with one disposer
		int _status = Status_None;
		
		#region Constructors

		/// <summary>
		/// Creates a new scope.
		/// </summary>
		public CleanupScope()
		{
		}

		/// <summary>
		/// Creates a new scope and adds to it the disposable item given.
		/// </summary>
		/// <param name="item">An item to be disposed when the scope is cleaned up.</param>
		public CleanupScope(IDisposable item)
		{
			if (item == null) throw new ArgumentNullException("item");

			_items.Push(new StackItem(item));
		}

		/// <summary>
		/// Creates a new scope and adds to it the disposable item given.
		/// </summary>
		/// <param name="item">Items to be disposed when the scope is cleaned up.</param>
		public CleanupScope(params IDisposable[] items)
		{
			foreach (IDisposable item in items)
			{
				if (item != null)
				{
					_items.Push(new StackItem(item));
				}
			}
		}

		/// <summary>
		/// Creates a new scope and adds an action to be performed when the scope is cleaned up.
		/// </summary>
		/// <param name="action">An action to be performed when the scope is cleaned up.</param>
		public CleanupScope(params Action[] actions)
		{
			foreach (Action action in actions)
			{
				_items.Push(new StackItem(action));
			}
		}

		/// <summary>
		/// Creates a new scope and adds an action to be performed when the scope is cleaned up.
		/// </summary>
		/// <param name="action">An action to be performed when the scope is cleaned up.</param>
		public CleanupScope(Action action)
		{
			if (action == null) throw new ArgumentNullException("action");

			_items.Push(new StackItem(action));
		}

		/// <summary>
		/// Finalizes the cleanup scope.
		/// </summary>
		~CleanupScope()
		{
			this.Dispose(false);
		}

		#endregion Constructors
		
		/// <summary>
		/// Adds a disposable item to the cleanup scope. Actions and disposable items are collected 
		/// and at cleanup whill be either disposed (IDisposables) or invoked (Actions) in the reverse 
		/// order in which they are added.
		/// </summary>
		/// <typeparam name="T">Type of the item being added; ensures IDisposable by inference.</typeparam>
		/// <param name="item">An item to be disposed when the scope is cleaned up.</param>
		/// <returns>Returns the item given.</returns>
		public T Add<T>(T item)
			where T : IDisposable
		{
			if (item == null) throw new ArgumentNullException("item");

			// Spinwait.
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}

			lock (_items) _items.Push(new StackItem(item));
			Thread.VolatileWrite(ref _status, Status_None);
			return item;
		}

		/// <summary>
		/// Adds an action to the cleanup scope. Actions and IDisposables collected in the same queue and
		/// are either disposed (IDisposables) or invoked (Actions) in the reverse order in which they are
		/// added.
		/// </summary>
		/// <param name="action">An action to be performed when the scope is cleaned up.</param>
		public void AddAction(Action action)
		{
			if (action == null) throw new ArgumentNullException("action");

			// Spinwait.
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}

			if (action != null)
			{
				lock (_items) _items.Push(new StackItem(action));
			}
			Thread.VolatileWrite(ref _status, Status_None);
		}

		/// <summary>
		/// Adds a disposable item to the cleanup scope and increments the 
		/// disposer count. The caller is required to call the Dispose() method
		/// for every invocation of this method.
		/// </summary>
		/// <typeparam name="T">Type of the item being added; ensures IDisposable by constraint.</typeparam>
		/// <param name="item">An item to be disposed when the scope is cleaned up.</param>
		/// <returns>Returns the item given.</returns>
		public T AddWithNewDisposer<T>(T item)
			where T : IDisposable
		{
			if (item == null) throw new ArgumentNullException("item");

			T v = Add(item);
			Interlocked.Increment(ref _disposers);
			return v;
		}

		/// <summary>
		/// Disposes the cleanup scope.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && Interlocked.Decrement(ref _disposers) > 0)
			{
				return;
			}
			if (_items != null)
			{
				#region Spinwait
				int status;
				while ((status = Interlocked.CompareExchange(ref _status, Status_Disposing, Status_None)) != Status_None)
				{
					if (status > Status_Writing)
					{
						try
						{
							this.TraceData(TraceEventType.Error, Resources.Error_CleanupScopeDetectedOverlappedDispose);
						}
						catch (Exception) { /* safety net, intentionally eat this error */ }
						// Another thread beat us to the cleanup. This is an error condition. We're out.
						return;
					}
				}
				#endregion

				foreach (StackItem item in _items)
				{
					try
					{
						if (item.Disposable != null) item.Disposable.Dispose();
						else if (item.Action != null) item.Action();
					}
					catch (Exception e)
					{
						if (disposing) throw;

						// We are in the GC, trace as a warning.
						try
						{
							this.TraceData(TraceEventType.Warning, () =>
							{
								return new object[]
									{
										String.Format(Resources.Warning_ErrorWhileDisposingCleanupScope,
										(item.Disposable == null) ? item.Action.GetFullName() : item.Disposable.GetType().FullName),
										e
									};
							});
						}
						catch (Exception) { /* safety net, intentionally eat this error while tracing an error */ }
					}
				}
				Thread.VolatileWrite(ref _status, Status_None);
			}
		}

		

		#region Nested Types

		struct StackItem
		{
			

			public Action Action;
			public IDisposable Disposable;

			

			#region Constructors

			public StackItem(IDisposable d)
			{
				this.Action = null; // keep compiler happy
				this.Disposable = d;
			}

			public StackItem(Action a)
			{
				this.Disposable = null; // keep compiler happy
				this.Action = a;
			}

			#endregion Constructors
		}

		#endregion Nested Types
	}
}