using System;
using System.Runtime.Serialization;

namespace ATTi.Core.Parallel
{
	[Serializable]
	public class ParallelException : ApplicationException
	{
		public ParallelException() : base() { }
		public ParallelException(string errMsg) : base(errMsg) { }
		public ParallelException(string errMsg, Exception ex) : base(errMsg, ex) { }

		protected ParallelException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

	[Serializable]
	public class TaskStateInvalidException : ParallelException
	{
		public TaskStateInvalidException() : base() { }
		public TaskStateInvalidException(TaskStates current, params TaskStates[] expected)
			: base(TaskStateInvalidException.FormatMessage(current, expected)) { }
		public TaskStateInvalidException(string errMsg) : base(errMsg) { }
		public TaskStateInvalidException(string errMsg, Exception ex) : base(errMsg, ex) { }

		protected TaskStateInvalidException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }

		private static String FormatMessage(TaskStates current, params TaskStates[] expected)
		{
			Contracts.Require.AtLeastOneParam<TaskStates>("expected", expected);
			String sexp = expected[0].ToString();
			for (int i = 0; i < expected.Length; ++i)
			{
				if (i == expected.Length - 1)
					sexp += "or " + expected[i].ToString();
				else
					sexp += ", " + expected[i].ToString();
			}
			return String.Format(Properties.Resources.Error_TaskStateInvalidExpect, sexp, current);
		}
	}

	[Serializable]
	public class TaskCanceledException : ParallelException
	{
		public TaskCanceledException() : base() { }
		public TaskCanceledException(string errMsg) : base(errMsg) { }
		public TaskCanceledException(string errMsg, Exception ex) : base(errMsg, ex) { }

		protected TaskCanceledException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

	[Serializable]
	public class TaskAbortedException : ParallelException
	{
		public TaskAbortedException() : base() { }
		public TaskAbortedException(string errMsg) : base(errMsg) { }
		public TaskAbortedException(string errMsg, Exception ex) : base(errMsg, ex) { }

		protected TaskAbortedException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

	[Serializable]
	public class ParallelTimeoutException : ParallelException
	{
		public ParallelTimeoutException() : base() { }
		public ParallelTimeoutException(string errMsg) : base(errMsg) { }
		public ParallelTimeoutException(string errMsg, Exception ex) : base(errMsg, ex) { }

		protected ParallelTimeoutException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

}
