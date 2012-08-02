using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ATTi.Core.Mementos;
using ATTi.Core.Properties;
using ATTi.Core.Reflection;
using ATTi.Core.Trace;

namespace ATTi.Core
{
	/// <summary>
	/// Creates a memory boundary. Any captured object will be restored to it's captured 
	/// state if the boundary doesn't complete.
	/// </summary>
	public sealed class MemoryBoundary : IMementoContext, IDisposable, ITraceable
	{
		#region Declarations
		const int Status_None = 0;
		const int Status_Writing = 1;
		const int Status_Disposing = -1;
		const int Status_Disposed = -2;

		internal interface IRestorable
		{
			void Restore(IMementoContext ctx);
			string GetErrorDescription();
		}
		enum BoundaryRecType
		{
			Restorable,
			CompensatingAction
		}
		struct MementoRestorer<T> : IRestorable
		{
			IMemento M;
			public MementoRestorer(IMemento m)
			{
				M = m;
			}
			void IRestorable.Restore(IMementoContext ctx)
			{
				T target = (T)M.Target;
				Memento.RestoreMemento<T>(ctx, ref target, M);
			}
			string IRestorable.GetErrorDescription()
			{
				return typeof(T).GetReadableFullName();
			}
		}
		struct BoundaryRec : IRestorable
		{
			private BoundaryRecType Type;
			private IRestorable R;
			private Action<IMementoContext> Compensate;

			internal void MementoOnRestore<T>(T item, IMemento m)
			{
				Type = BoundaryRecType.Restorable;
				R = new MementoRestorer<T>(m);
			}

			internal void CompensateOnRestore(Action<IMementoContext> c)
			{
				Type = BoundaryRecType.CompensatingAction;
				Compensate = c;
			}

			void IRestorable.Restore(IMementoContext ctx)
			{
				if (Type == BoundaryRecType.Restorable) R.Restore(ctx);
				else Compensate(ctx);
			}
			string IRestorable.GetErrorDescription()
			{
				if (Type == BoundaryRecType.Restorable) return R.GetErrorDescription();
				else return Compensate.GetFullName();
			}
		}
		readonly Object _sync = new Object();
		readonly Stack<BoundaryRec> _items = new Stack<BoundaryRec>();
		readonly Dictionary<object, IMemento> _mementos = new Dictionary<object, IMemento>();
		int _status = Status_None;
		int _disposers = 1; // Start with one disposer
		bool _completed;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public MemoryBoundary()
		{
		}
		#endregion

		/// <summary>
		/// Captures the current state of the instance given.
		/// </summary>
		/// <typeparam name="T">Item type T</typeparam>
		/// <param name="item">An instance to capture.</param>
		/// <returns>The item given.</returns>
		public T Capture<T>(T item)
		{
			Contracts.Require.IsNotNull("item", item);

			BoundaryRec r = new BoundaryRec();
			r.MementoOnRestore(item, Memento.CaptureMemento(this, item));

			// Spinwait.
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}
			if (item != null)
			{
				lock (_sync) _items.Push(r);
			}
			Thread.VolatileWrite(ref _status, Status_None);
			return item;
		}

		/// <summary>
		/// Uses the helper given to capture a memento of the instance given.
		/// </summary>
		/// <typeparam name="T">Item type T</typeparam>
		/// <typeparam name="H">Helper type H</typeparam>
		/// <param name="item">An instance to capture.</param>
		/// <param name="helper">A helper used to capture the memento of the item given.</param>
		/// <returns>The item given.</returns>
		public T Capture<T, H>(T item, H helper)
			where H : IMementoHelper<T>
		{
			Contracts.Require.IsNotNull("item", item);
			Contracts.Require.IsNotNull("helper", helper);

			IMemento m = helper.CaptureMemento(this, item);
			BoundaryRec r = new BoundaryRec();
			r.CompensateOnRestore(c =>
				{
					T i = item;
					helper.RestoreMemento(c, ref i, m);
				});

			// Spinwait.
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}
			lock (_sync) _items.Push(r);
			Thread.VolatileWrite(ref _status, Status_None);
			return item;
		}

		/// <summary>
		/// Adds a compensating action to the boundary. If the boundary 
		/// is not committed then this compensating action is guaranteed 
		/// to be invoked.
		/// </summary>
		/// <param name="action">A compensating action.</param>
		public void AddCompensatingAction(Action<IMementoContext> action)
		{
			Contracts.Require.IsNotNull("action", action);

			BoundaryRec r = new BoundaryRec();
			r.CompensateOnRestore(action);

			// Spinwait.
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}
			lock (_sync) _items.Push(r);
			Thread.VolatileWrite(ref _status, Status_None);
		}

		/// <summary>
		/// Marks the memory boundary complete. Uncompleted boundaries will
		/// cause captured objects to be restored to their captured state.
		/// </summary>
		public void MarkComplete()
		{
			int status;
			while ((status = Interlocked.CompareExchange(ref _status, Status_Writing, Status_None)) != Status_None)
			{
				if (status > Status_Writing) throw new ObjectDisposedException(this.GetType().FullName);
			}
			_completed = true;
			Thread.VolatileWrite(ref _status, Status_None);
		}

		#region IDisposable Members

		/// <summary>
		/// Finalizes the cleanup scope.
		/// </summary>
		~MemoryBoundary()
		{
			this.Dispose(false);
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
							this.TraceData(TraceEventType.Error, Resources.Error_MemoryBoundaryDetectedOverlappedDispose);
						}
						catch (Exception) { /* safety net, intentionally eat this error */ }
						// Another thread beat us to the disposal. This is an error condition. We're out.
						return;
					}
				}
				#endregion

				if (!_completed)
				{
					foreach (IRestorable item in _items)
					{
						try
						{
							item.Restore(this);
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
										String.Format(Resources.Error_UnhandledErrorWhileDisposingMemoryBoundary, item.GetErrorDescription())
										, e };
								});
							}
							catch (Exception) { /* safety net, intentionally eat this error while tracing an error */ }
						}
					}
				}
				Thread.VolatileWrite(ref _status, Status_None);
			}
		}

		/// <summary>
		/// Disposes the boundary.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region IMementoContext Members

		bool IMementoContext.TryGetMemento(object item, out IMemento result)
		{
			lock (_mementos) return _mementos.TryGetValue(item, out result);
		}

		IMemento IMementoContext.PutMemento(object item, IMemento m)
		{
			lock (_mementos) _mementos.Add(item, m);
			return m;
		}

		#endregion
	}
}
