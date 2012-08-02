using System;
using System.Runtime.Serialization;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Exception indicating TypeFactory failure.
	/// </summary>
	[Serializable]
	public class FactoryException : ApplicationException
	{
		/// <summary>
		/// Constructs a new FactoryException
		/// </summary>
		public FactoryException() : base() { }
		/// <summary>
		/// Constructs a new FactoryException initialized with the error message
		/// given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		public FactoryException(string errMsg) : base(errMsg) { }
		/// <summary>
		/// Constructs a new FactoryException initialized with the error message
		/// and cause given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		/// <param name="cause">An Exception class that is the cause of this
		/// exception.</param>
		public FactoryException(string errMsg, Exception cause) : base(errMsg, cause) { }

		protected FactoryException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}

	[Serializable]
	public class CircularCreationException : FactoryException
	{
		/// <summary>
		/// Constructs a new FactoryException
		/// </summary>
		public CircularCreationException() : base() { }
		/// <summary>
		/// Constructs a new FactoryException initialized with the error message
		/// given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		public CircularCreationException(string errMsg) : base(errMsg) { }
		/// <summary>
		/// Constructs a new FactoryException initialized with the error message
		/// and cause given.
		/// </summary>
		/// <param name="errMsg">Error message describing the exception 
		/// condition.</param>
		/// <param name="cause">An Exception class that is the cause of this
		/// exception.</param>
		public CircularCreationException(string errMsg, Exception cause) : base(errMsg, cause) { }

		protected CircularCreationException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
	}
}
