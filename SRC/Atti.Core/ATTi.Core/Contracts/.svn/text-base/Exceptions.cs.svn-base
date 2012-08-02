namespace ATTi.Core.Contracts
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Base contract exception; thrown when design contracts are violated.
	/// </summary>
	[Serializable]
	public class ContractException : ApplicationException
	{
		#region Constructors

		/// <summary>
		/// Default constructor; creates a new instance.
		/// </summary>
		public ContractException()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance using the error message given.
		/// </summary>
		/// <param name="errorMessage">An error message describing the contract violation.</param>
		public ContractException(string errorMessage)
			: base(errorMessage)
		{
		}

		public ContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		/// <summary>
		/// Used during serialization.
		/// </summary>
		/// <param name="si">SerializationInfo</param>
		/// <param name="sc">StreamingContext</param>
		protected ContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}

	[Serializable]
	public class IntermediateContractException : ContractException
	{
		#region Constructors

		/// <summary>
		/// Default constructor; creates a new instance.
		/// </summary>
		public IntermediateContractException()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance using the error message given.
		/// </summary>
		/// <param name="errorMessage">An error message describing the contract violation.</param>
		public IntermediateContractException(string errorMessage)
			: base(errorMessage)
		{
		}

		public IntermediateContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		/// <summary>
		/// Used during serialization.
		/// </summary>
		/// <param name="si">SerializationInfo</param>
		/// <param name="sc">StreamingContext</param>
		protected IntermediateContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}

	[Serializable]
	public class InvalidContractException : ContractException
	{
		#region Constructors

		public InvalidContractException()
			: base()
		{
		}

		public InvalidContractException(string errMsg)
			: base(errMsg)
		{
		}

		public InvalidContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		protected InvalidContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}

	[Serializable]
	public class InvariantContractException : ContractException
	{
		#region Constructors

		/// <summary>
		/// Default constructor; creates a new instance.
		/// </summary>
		public InvariantContractException()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance using the error message given.
		/// </summary>
		/// <param name="errorMessage">An error message describing the contract violation.</param>
		public InvariantContractException(string errorMessage)
			: base(errorMessage)
		{
		}

		public InvariantContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		/// <summary>
		/// Used during serialization.
		/// </summary>
		/// <param name="si">SerializationInfo</param>
		/// <param name="sc">StreamingContext</param>
		protected InvariantContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}

	[Serializable]
	public class PostconditionContractException : ContractException
	{
		#region Constructors

		/// <summary>
		/// Default constructor; creates a new instance.
		/// </summary>
		public PostconditionContractException()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance using the error message given.
		/// </summary>
		/// <param name="errorMessage">An error message describing the contract violation.</param>
		public PostconditionContractException(string errorMessage)
			: base(errorMessage)
		{
		}

		public PostconditionContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		/// <summary>
		/// Used during serialization.
		/// </summary>
		/// <param name="si">SerializationInfo</param>
		/// <param name="sc">StreamingContext</param>
		protected PostconditionContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}

	/// <summary>
	/// Thrown when Precondition design contracts are violated.
	/// </summary>
	[Serializable]
	public class PreconditionContractException : ContractException
	{
		#region Constructors

		/// <summary>
		/// Default constructor; creates a new instance.
		/// </summary>
		public PreconditionContractException()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance using the error message given.
		/// </summary>
		/// <param name="errorMessage">An error message describing the contract violation.</param>
		public PreconditionContractException(string errorMessage)
			: base(errorMessage)
		{
		}

		public PreconditionContractException(string errMsg, Exception ex)
			: base(errMsg, ex)
		{
		}

		/// <summary>
		/// Used during serialization.
		/// </summary>
		/// <param name="si">SerializationInfo</param>
		/// <param name="sc">StreamingContext</param>
		protected PreconditionContractException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		#endregion Constructors
	}
}