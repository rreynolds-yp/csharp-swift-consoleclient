namespace ATTi.Core
{
	using System;
	using System.Text;
	using System.Threading;

	/// <summary>
	/// Contains utility methods.
	/// </summary>
	public static class Util
	{	

		static readonly char[] __nibbleHex = new char[] 
			{
			'0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
			};

		/// <summary>
		/// Converts the bytes to octets.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="len">The len.</param>
		/// <returns></returns>
		public static string ConvertBytesToOctets(byte[] buffer, int offset, int len)
		{
			StringBuilder builder = new StringBuilder(3 * len);
			int first = offset;
			for (int i = first; i < (offset + len); i++ )
			{
				byte b = buffer[i];
				if (i > first) builder.Append(' ');
				builder.Append(__nibbleHex[b >> 4]);
				builder.Append(__nibbleHex[b & 0x0F]);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Disposes of the referenced item and sets its value to null.
		/// </summary>
		/// <typeparam name="T">item type T; must implement IDisposable</typeparam>
		/// <param name="item">item to dispose</param>
		public static void Dispose<T>(ref T item)
			where T : class, IDisposable
		{
			var i = Interlocked.Exchange(ref item, null);
			if (i != null)
			{
				i.Dispose();
			}
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// Uses the default constroctur for variable type T.
		/// </summary>
		/// <typeparam name="T">variable type T; must be a class with default constructor</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		public static T LazyInitialize<T>(ref T variable)
			where T : class, new()
		{
			if (variable == null)
			{
				T ourNewInstance = new T();
				T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				// prefer the race winner's instance... the GC will collect ours.
				variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. Uses
		/// the <paramref name="factoryDelegate"/> to create the instance if necessary.
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="factoryDelegate">factory delegate</param>
		/// <returns>the value of the variable, after the lazy initailize</returns>
		public static T LazyInitialize<T>(ref T variable, Func<T> factoryDelegate)
			where T : class
		{
			if (factoryDelegate == null) throw new ArgumentNullException("factoryDelegate");

			if (variable == null)
			{
				T ourNewInstance = factoryDelegate();
				T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				// prefer the race winner's instance... the GC will collect ours.
				variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized.
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="lck">an object used as a lock if initialization is necessary</param>
		public static T LazyInitializeWithLock<T>(ref T variable, Object lck)
			where T : class, new()
		{
			if (lck == null) throw new ArgumentNullException("lck");

			if (variable == null)
			{
				lock (lck)
				{ // double-check the lock in case we're in a race...
					if (variable == null)
					{
						T ourNewInstance = new T();
						T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
						// prefer the race winner's instance... the GC will collect ours.
						variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
					}
				}
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. Uses
		/// the <paramref name="factoryDelegate"/> to create the instance if necessary.
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="lck">an object used as a lock if initialization is necessary</param>
		/// <param name="factoryDelegate">factory delegate</param>
		/// <returns>the value of the variable, after the lazy initailize</returns>
		public static T LazyInitializeWithLock<T>(ref T variable, Object lck, Func<T> factoryDelegate)
			where T : class
		{
			if (lck == null) throw new ArgumentNullException("lck");
			if (factoryDelegate == null) throw new ArgumentNullException("factoryDelegate");

			if (variable == null)
			{
				lock (lck)
				{ // double-check the lock in case we're in a race...
					if (variable == null)
					{
						T ourNewInstance = factoryDelegate();
						T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
						// prefer the race winner's instance... the GC will collect ours.
						variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
					}
				}
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// </summary>
		/// <typeparam name="T">variable type T; must be a class with default constructor</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		/// <remarks>
		/// This method is thread-safe and non-blocking, however, callers must be aware
		/// that a race condition may occur during object creation. It is possible for two 
		/// instances of type T to be created by two different threads calling this method
		/// at the same time; in such a case, the race loser will discard 
		/// its instance in favor of the race winner's instance. This is safe for most well
		/// defined types whose constructors are side-effect free.
		/// </remarks>
		public static T NonBlockingLazyInitializeVolatile<T>(ref object variable)
			where T : class, new()
		{
			T currentValue = (T)Thread.VolatileRead(ref variable);
			if (currentValue != null)
			{
				return currentValue;
			}

			T ourNewInstance = new T();
			T instanceCreatedByOtherThread = (T)Interlocked.CompareExchange(ref variable, ourNewInstance, null);
			// prefer the race winner's instance... the GC will collect ours.
			return (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="factoryDelegate">factory method that will be called upon to 
		/// create a new instance if necessary</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		/// <remarks>
		/// This method is thread-safe and non-blocking, however, callers must be aware
		/// that a race condition may occur during object creation. It is possible for two 
		/// instances of type T to be created by two different threads calling this method
		/// at the same time; in such a case, the race loser will discard 
		/// its instance in favor of the race winner's instance. This is safe for most well
		/// defined types whose constructors are side-effect free.
		/// </remarks>
		public static T NonBlockingLazyInitializeVolatile<T>(ref T variable, Func<T> factoryDelegate)
			where T : class
		{
			if (factoryDelegate == null) throw new ArgumentNullException("factoryDelegate");

			// treat the reference as volatile...
			Thread.MemoryBarrier();
			T currentValue = variable;
			Thread.MemoryBarrier();

			if (currentValue != null)
			{
				return currentValue;
			}

			T instanceCreatedByOtherThread = null;
			var isDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
			T ourNewInstance = factoryDelegate();
			try
			{
				instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				return (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			finally
			{
				if (instanceCreatedByOtherThread != null && isDisposable)
				{
					((IDisposable)ourNewInstance).Dispose();
				}
			}
		}

		public static T NonBlockingLazyInitializeVolatile<T>(ref T variable)
			where T : class, new()
		{			
			// treat the reference as volatile...
			Thread.MemoryBarrier();
			T currentValue = variable;
			Thread.MemoryBarrier();

			if (currentValue != null)
			{
				return currentValue;
			}

			T instanceCreatedByOtherThread = null;
			var isDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
			T ourNewInstance = new T();
			try
			{
				instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				return (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			finally
			{
				if (instanceCreatedByOtherThread != null && isDisposable)
				{
					((IDisposable)ourNewInstance).Dispose();
				}
			}
		}

	}
}