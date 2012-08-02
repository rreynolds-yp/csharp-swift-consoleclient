
namespace ATTi.Core.Parallel
{
	/// <summary>
	/// Enumeration of the states of a parallel task.
	/// </summary>
	public enum TaskStates
	{
		/// <summary>
		/// Indicates the task is idle.
		/// </summary>
		Idle = 0x0000,
		/// <summary>
		/// Indicates the task is pending execution.
		/// </summary>
		Pending = 0x0001 << 0,
		/// <summary>
		/// Indicates the task has been scheduled for execution (it is waiting on another task or condition).
		/// </summary>
		PendingScheduled = 0x0001 << 1 | Pending,
		/// <summary>
		/// Indicates the task has been marked for cancelation before it began execution.
		/// </summary>
		PendingCancel = 0x0001 << 2 | Pending,
		/// <summary>
		/// Indicates the task has begun execution.
		/// </summary>
		Executing = 0x0001 << 3,
		/// <summary>
		/// Indicates the task is yielding to another task or condition.
		/// </summary>
		Yielding = 0x0001 << 4 | Executing,
		/// <summary>
		/// Indicates the task has completed.
		/// </summary>
		Completed = 0x0001 << 5,
		/// <summary>
		/// Indicates the task completed with success.
		/// </summary>
		CompletedSuccess = 0x0001 << 6 | Completed,
		/// <summary>
		/// Indicates the task completed with error.
		/// </summary>
		CompletedWithError = 0x0001 << 7 | Completed,
		/// <summary>
		/// Indicates the task was canceled.
		/// </summary>
		CompletedCanceled = 0x0001 << 8 | Completed
	}
}
