using System;
using System.Threading;
using ATTi.Core.Properties;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Placeholder for a future variable. 
	/// </summary>
	/// <typeparam name="T">Type of the future variable.</typeparam>
	/// <remarks>
	/// Future variables are use for communicating values among threads when
	/// the different threads need to coordinate calculations or operations
	/// based on variables produced by the other threads. Each thread can
	/// safely perform part of an operation and set a <b>future</b> to 
	/// communicate values or state to the other threads.
	/// 
	/// Futures use a set-once semantic. It is an error to write a value to
	/// a future once its value has been set.
	/// </remarks>
	public class Future<T> : IFuture<T>
	{
		#region Declarations
		private const int ValuePending = 0;
		private const int ValueIsValid = 1;

		private Object _lock;
		private int _validity = ValuePending;
		private T _value;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new instance with no value.
		/// </summary>
		public Future() { }
		/// <summary>
		/// Constructs a new instance and initializes it with the value given.
		/// </summary>
		/// <param name="value">A value for the future variable.</param>
		public Future(T value)
		{
			_value = value;
			_validity = ValueIsValid;
		}
		#endregion
						
		/// <summary>
		/// Sets the future variable's value.
		/// </summary>
		/// <param name="value">A value for the future variable.</param>
		/// <exception cref="InvalidOpeationException">thrown if the future
		/// variable has already been set. Future variables use a set-once semantic.
		/// </exception>
		public void SetValue(T value)
		{
			// Ensure the value only gets set once...
			int previousValidity = Interlocked.Exchange(ref _validity, ValueIsValid);
			if (previousValidity != ValuePending)
				throw new InvalidOperationException(Resources.Error_FutureValueAlreadyHasAValue);

			_value = value;

			if (_lock != null)
			{
				lock (_lock)
				{
					Monitor.PulseAll(_lock);
				}
			}
		}

		private Object SyncRoot
		{
			get
			{
				if (_lock == null)
				{
					Boolean done = this.HasValue;
					Object syncRoot = new Object();
					if (Interlocked.CompareExchange(ref _lock, syncRoot, null) == null)
					{
						if (!done && this.HasValue)
						{
							lock (_lock)
							{
								// The value was set while creating sync root, signal it.
								Monitor.PulseAll(_lock);
							}
						}
					}
				}
				return _lock;
			}
		}

		#region IFuture<T> Members

		/// <summary>
		/// Indicates whether the future variable has been set.
		/// </summary>
		public bool HasValue { get { return _validity == ValueIsValid; } }

		/// <summary>
		/// Gets the future variable's value. Warning! Reading this property
		/// will block your thread indefinitely or until the future variable
		/// has been set; whichever comes sooner.
		/// </summary>
		public T Value { get { return WaitForValue(); } }
		
		public bool TryReadValue(out T value)
		{
			if (this.HasValue)
			{
				value = _value;
				return true;
			}
			
			value = default(T);
			return false;
		}

		public bool TryReadValue(int millisecondsTimeout, out T value)
		{
			if (millisecondsTimeout == 0) return TryReadValue(out value);
			else return TryReadValue(TimeSpan.FromMilliseconds(millisecondsTimeout), out value);
		}

		public bool TryReadValue(TimeSpan timeout, out T value)
		{
			long ticks = DateTime.Now.Ticks;
			long timeoutExpiration = ticks + timeout.Ticks;
			long remainingTicks;

			if (this.HasValue)
			{
				value = _value;
				return true;
			}
			else
			{
				lock (this.SyncRoot)
				{
					remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
					do
					{
						if (remainingTicks > 0
							&& Monitor.Wait(_lock, new TimeSpan(remainingTicks))
							&& this.HasValue)
						{
							value = _value;
							return true;
						}
						remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
					} while (remainingTicks > 0);
				}
			}

			value = default(T);
			return false;
		}

		public T WaitForValue()
		{
			if (!this.HasValue)
			{
				lock (SyncRoot)
				{
					while (!this.HasValue)
						Monitor.Wait(_lock);
				}
			}
			return _value;
		}

		public T WaitForValue(int millisecondsTimeout)
		{
			if (millisecondsTimeout == 0) return WaitForValue();
			else return WaitForValue(TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		public T WaitForValue(TimeSpan timeout)
		{
			long ticks = DateTime.Now.Ticks;
			long timeoutExpiration = ticks + timeout.Ticks;
			long remainingTicks;

			if (this.HasValue)
			{
				return _value;
			}
			else
			{
				lock (this.SyncRoot)
				{
					remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
					do
					{
						if (remainingTicks > 0
							&& Monitor.Wait(_lock, new TimeSpan(remainingTicks))
							&& this.HasValue)
						{
							return _value;
						}
						remainingTicks = timeoutExpiration - DateTime.Now.Ticks;
					} while (remainingTicks > 0);
				}
			}

			throw new ParallelTimeoutException();
		}

		#endregion
	}
}
