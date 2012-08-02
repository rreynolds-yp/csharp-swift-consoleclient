using System;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Interface for future values of type V.
	/// </summary>
	/// <typeparam name="V">value type V</typeparam>
	public interface IFuture<V>
	{
		/// <summary>
		/// Indicates whether the future contains a value.
		/// </summary>
		bool HasValue { get; }
		/// <summary>
		/// Gets the value of the future. If the value is not available
		/// this 
		/// </summary>
		V Value { get; }
		/// <summary>
		/// Tries to read the value. This call will not block the calling
		/// thread if the value is not present.
		/// </summary>
		/// <param name="value">A reference where the value will be written if 
		/// it is present.</param>
		/// <returns><em>true</em> if the value is present; otherwise <em>false</em>.</returns>
		bool TryReadValue(out V value);
		bool TryReadValue(int millisecondsTimeout, out V value);
		bool TryReadValue(TimeSpan timeout, out V value);

		/// <summary>
		/// Waits (blocks the current thread) until the value is present.
		/// </summary>
		/// <returns>The future's value.</returns>
		V WaitForValue();
		/// <summary>
		/// Waits (blocks the current thread) until the value is present or the timeout is exceeded.
		/// </summary>
		/// <param name="millisecondsTimeout">Timeout in milliseconds.</param>
		/// <returns>The future's value.</returns>
		/// <exception cref="ParallelTimeoutException">thrown if the timeout is exceeded before the value becomes available.</exception>
		V WaitForValue(int millisecondsTimeout);
		/// <summary>
		/// Waits (blocks the current thread) until the value is present or the timeout is exceeded.
		/// </summary>
		/// <param name="timeout">A timespan representing the timeout period.</param>
		/// <returns>The future's value.</returns>
		/// <exception cref="ParallelTimeoutException">thrown if the timeout is exceeded before the value becomes available.</exception>
		V WaitForValue(TimeSpan timeout);
	}		
}
