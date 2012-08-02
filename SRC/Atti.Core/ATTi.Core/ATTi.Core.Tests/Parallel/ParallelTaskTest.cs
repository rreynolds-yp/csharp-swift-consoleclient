using System;
using System.Threading;
using ATTi.Core.Parallel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATTi.Core.Tests.Parallel
{
	[TestClass]
	public class ParallelTaskTest
	{
		private int _counter = 0;

		private void WaitForTwoSeconds()
		{
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Interlocked.Increment(ref _counter);
		}

		public ParallelTaskTest() { }

		/// <summar>y
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }
			

		[TestMethod]
		public void IndependentParallelTask()
		{
			// Reset the counter, the tasks will update this.
			Thread.VolatileWrite(ref _counter, 0);

			// Create and execute one task that waits two seconds before it completes.
			ParallelTask task = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
			Assert.IsTrue(task.IsIdle);
			task.AsyncExecute();
			Assert.IsTrue(task.IsPending | task.IsExecuting, "task state should be Pending or Executing");

			// We should get here in milliseconds, so the counter should not
			// be incremented yet. If it is then one of the above calls was
			// a blocking call, which is an error.
			Assert.AreEqual(0, Thread.VolatileRead(ref _counter), "Counter should not have incremented already");

			// Wait for the task to complete.
			task.WaitForCompletion();
			Assert.IsTrue(task.IsCompletedSuccess);

			// And now the counter should have been incremented by the task.
			Assert.AreEqual(1, Thread.VolatileRead(ref _counter));
		}

		/// <summary>
		/// Tests a simple dependent ParallelTask
		/// </summary>
		[TestMethod]
		public void DependentParallelTask()
		{
			// Reset the counter, the tasks will update this.
			Thread.VolatileWrite(ref _counter, 0);

			// Create an boundary that will ensure all tasks complete before exiting.
			using (TaskBoundary boundary = new TaskBoundary())
			{
				// Create two tasks...
				ParallelTask firstTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
				ParallelTask secondTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
				Assert.IsTrue(firstTask.IsIdle);
				Assert.IsTrue(secondTask.IsIdle);

				// Start the first task, it will take 2 seconds to complete.
				firstTask.AsyncExecute();
				Assert.IsTrue(firstTask.IsPending | firstTask.IsExecuting, "task state should be Pending or Executing");
				Assert.IsTrue(secondTask.IsIdle, "task state should be Idle");

				// Schedule the second task to start when the first task completes.
				//   The timeout is set to 10 seconds just in case the first task
				//   fails to complete. You can also use Timeout.Infinite.
				secondTask.ScheduledExecute(firstTask, TimeSpan.FromSeconds(10));
				Assert.IsTrue(secondTask.IsPending | secondTask.IsExecuting, "task state should be Pending or Executing");
				Assert.IsTrue(secondTask.IsScheduled | secondTask.IsExecuting, "task state should be Scheduled or Executing");

				// We should get here in milliseconds, so the counter should not
				// be incremented yet. If it is then one of the above calls was
				// a blocking call, which is an error.
				Assert.AreEqual(0, Thread.VolatileRead(ref _counter), "Counter should not have incremented already");

				// Wait for the second task to complete. This will take slightly
				// longer than 4 seconds.
				secondTask.WaitForCompletion();
				Assert.IsTrue(firstTask.IsCompletedSuccess);
				Assert.IsTrue(secondTask.IsCompletedSuccess);

				boundary.MarkCompleted();
			}
			// And now the counter should have been incremented by both tasks.
			Assert.AreEqual(2, Thread.VolatileRead(ref _counter));
		}

		/// <summary>
		/// Tests a simple dependent ParallelTask where the dependent task is scheduled 
		/// before the first task starts
		/// </summary>
		[TestMethod]
		public void DependentParallelTask_DependentTaskFirst()
		{
			// Reset the counter, the tasks will update this.
			Thread.VolatileWrite(ref _counter, 0);

			// Create an boundary that will ensure all tasks complete before exiting.
			using (TaskBoundary boundary = new TaskBoundary())
			{
				// Create two tasks...
				ParallelTask firstTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
				ParallelTask secondTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
				Assert.IsTrue(firstTask.IsIdle);
				Assert.IsTrue(secondTask.IsIdle);

				// Schedule the second task to start when the first task completes.
				//   The timeout is set to 10 seconds just in case the first task
				//   fails to complete. You can also use Timeout.Infinite.
				secondTask.ScheduledExecute(firstTask, TimeSpan.FromSeconds(10));
				Assert.IsTrue(secondTask.IsPending, "task state should be Pending");
				Assert.IsTrue(secondTask.IsScheduled, "task state should be Scheduled");
				Assert.IsTrue(firstTask.IsIdle);

				// Start the first task, it will take 2 seconds to complete.
				firstTask.AsyncExecute();
				Assert.IsTrue(firstTask.IsPending | firstTask.IsExecuting, "task state should be Pending or Executing");

				// We should get here in milliseconds, so the counter should not
				// be incremented yet. If it is then one of the above calls was
				// a blocking call, which is an error.
				Assert.AreEqual(0, Thread.VolatileRead(ref _counter), "Counter should not have incremented already");

				// Wait for the second task to complete. This will take slightly
				// longer than 4 seconds.
				secondTask.WaitForCompletion();
				Assert.IsTrue(firstTask.IsCompleted);
				Assert.IsTrue(secondTask.IsCompleted);

				boundary.MarkCompleted();
			}
			// And now the counter should have been incremented by both tasks.
			Assert.AreEqual(2, Thread.VolatileRead(ref _counter));
		}

		/// <summary>
		/// Tests a simple dependent ParallelTask where the dependent task times out 
		/// before the first task completes
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ParallelTimeoutException))]
		public void DependentParallelTask_SecondTaskTimeoutBeforeFirstTaskCompletes()
		{
			// Reset the counter, the tasks will update this.
			Thread.VolatileWrite(ref _counter, 0);

			try
			{
				// Create an boundary that will ensure all tasks complete before exiting.
				using (TaskBoundary boundary = new TaskBoundary())
				{
					// Create two tasks...
					ParallelTask firstTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
					ParallelTask secondTask = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
					Assert.IsTrue(firstTask.IsIdle);
					Assert.IsTrue(secondTask.IsIdle);

					// Schedule the second task to start when the first task completes.
					//   The timeout is set to 1 second so that it times out before
					//   the first task completes.
					secondTask.ScheduledExecute(firstTask, TimeSpan.FromSeconds(1));
					Assert.IsTrue(secondTask.IsPending, "task state should be Pending");
					Assert.IsTrue(secondTask.IsScheduled, "task state should be Scheduled");
					Assert.IsTrue(firstTask.IsIdle);

					// Start the first task, it will take 2 seconds to complete.
					firstTask.AsyncExecute();
					Assert.IsTrue(firstTask.IsPending | firstTask.IsExecuting, "task state should be Pending or Executing");

					// We should get here in milliseconds, so the counter should not
					// be incremented yet. If it is then one of the above calls was
					// a blocking call, which is an error.
					Assert.AreEqual(0, Thread.VolatileRead(ref _counter), "Counter should not have incremented already");

					try
					{
						// Wait for the second task to complete. This will take slightly
						// longer than 1 second and a TimeoutException is thrown.
						secondTask.WaitForCompletion();
						Assert.Fail("The dependent task should have thrown an ParallelTimeoutException");
					}
					finally
					{
						// We expected the error and want the first task to complete.
						boundary.MarkCompleted();
					}
				}
			}
			catch (ParallelTimeoutException)
			{
				// And now the counter should have been incremented by the first task before 
				// exiting the boundary.
				Assert.AreEqual(1, Thread.VolatileRead(ref _counter));
				throw;
			}
		}

		private void FireAndForget()
		{
			ParallelTask task = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds);
			task.AsyncExecute();
			new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.WaitForTwoSeconds).ScheduledExecute(task, Timeout.Infinite);
		}

		/// <summary>
		/// Tests an independent ParallelTask that fires two additional tasks and 
		/// forgets them.
		/// </summary>
		[TestMethod]
		public void IndependentParallelTask_FireAndForget()
		{
			// Reset the counter, the tasks will update this.
			Thread.VolatileWrite(ref _counter, 0);

			using (TaskBoundary boundary = new TaskBoundary())
			{
				// Create and execute one task that waits two seconds before it completes.
				ParallelTask task = new ParallelActionWrapper<ThreadPoolExecutionStrategy>(this.FireAndForget);
				Assert.IsTrue(task.IsIdle);
				task.AsyncExecute();
				Assert.IsTrue(task.IsPending | task.IsExecuting, "task state should be Pending or Executing");

				// The counter will increment after the fire and forget tasks execute.
				Assert.AreEqual(0, Thread.VolatileRead(ref _counter), "Counter should not have incremented already");

				// Wait for the task to complete.
				task.WaitForCompletion();
				Assert.IsTrue(task.IsCompletedSuccess);

				boundary.MarkCompleted();
			}
			// And now the counter should have been incremented by the two fire and forget tasks.
			Assert.AreEqual(2, Thread.VolatileRead(ref _counter));
		}

	}

}
