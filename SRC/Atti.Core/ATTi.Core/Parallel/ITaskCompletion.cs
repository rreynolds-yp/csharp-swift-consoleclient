using System;

namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Interface extending the IAsyncResult interface for use with the
	/// parallel task framework. (used for procedures and tasks that don't
	/// explicitly return results)
	/// </summary>
	public interface ITaskCompletion : IAsyncResult
	{
		/// <summary>
		/// Marks the task as completed.
		/// </summary>
		/// <param name="completedSyncronously">Indicates whether the task completed synchronously.</param>
		void MarkCompleted(bool completedSyncronously);
		/// <summary>
		/// Marks the task as completed with exception.
		/// </summary>
		/// <param name="ex">The exception.</param>
		/// <param name="completedSyncronously">Indicates whether the task completed synchronously.</param>
		void MarkException(Exception ex, bool completedSynchronously);
		/// <summary>
		/// Ends the invoke, blocking the calling thread until the task has completed.
		/// </summary>
		void EndInvoke();
		/// <summary>
		/// Ends the invoke, blocking the calling thread until the task completes
		/// or the timeout period is exceeded.
		/// </summary>
		/// <param name="timeout">The timeout period</param>
		/// <param name="exitContext"><em>true</em> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <em>false</em>.</param>
		/// <exception cref="ATTi.Core.Parallel.ParallelTimeoutException">thrown if the timeout is exceeded before the task completes.</exception>
		void EndInvoke(TimeSpan timeout, bool exitContext);
		/// <summary>
		/// Ends the invoke, blocking the calling thread until the task completes
		/// or the timeout period is exceeded.
		/// </summary>
		/// <param name="millisecondsTimeout">The timeout period expressed in milliseconds</param>
		/// <param name="exitContext"><em>true</em> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <em>false</em>.</param>
		/// <exception cref="ATTi.Core.Parallel.ParallelTimeoutException">thrown if the timeout is exceeded before the task completes.</exception>
		void EndInvoke(int millisecondsTimeout, bool exitContext);
	}

	/// <summary>
	/// Interface extending the IAsyncResult interface for use with the
	/// parallel task framework. (used for functions and tasks that 
	/// explicitly return results)
	/// </summary>
	/// <typeparam name="R">result type R</typeparam>
	public interface ITaskCompletion<R> : ITaskCompletion, IFuture<R>
	{
		/// <summary>
		/// Marks a task as completed and sets the task's result.
		/// </summary>
		/// <param name="result">The task's result.</param>
		/// <param name="completedSyncronously">Indicates whether the task completed synchronously.</param>
		void MarkCompleted(R result, bool completedSynchronously);
	}
}
