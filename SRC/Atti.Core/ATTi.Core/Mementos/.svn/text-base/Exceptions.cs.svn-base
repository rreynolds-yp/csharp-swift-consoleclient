using System;
using System.Runtime.Serialization;

namespace ATTi.Core.Mementos
{
	/// <summary>
	/// Exception indicating Memento failure.
	/// </summary>
	[Serializable]
	public class MementoException : ApplicationException
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public MementoException() : base() { }
		/// <summary>
		/// Creates a new instance with the message given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		public MementoException(string errMsg) : base(errMsg) { }
		/// <summary>
		/// Creates a new instance with the message and cause given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		/// <param name="cause">An Exception class that is the cause of this
		/// exception.</param>
		public MementoException(string errMsg, Exception cause) : base(errMsg, cause) { }

		protected MementoException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

	[Serializable]
	public class MementoNotSupportedException : ApplicationException
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public MementoNotSupportedException() : base() { }
		/// <summary>
		/// Creates a new instance with the message given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		public MementoNotSupportedException(string errMsg) : base(errMsg) { }
		/// <summary>
		/// Creates a new instance with the message and cause given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		/// <param name="cause">An Exception class that is the cause of this
		/// exception.</param>
		public MementoNotSupportedException(string errMsg, Exception cause) : base(errMsg, cause) { }

		protected MementoNotSupportedException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}
}
