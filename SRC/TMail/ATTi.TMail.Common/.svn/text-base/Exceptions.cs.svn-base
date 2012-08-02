using System;
using System.Runtime.Serialization;

namespace ATTi.TMail.Common
{
	/// <summary>
	/// Base service exception.
	/// </summary>
	[Serializable]
	public class ServiceException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceException"/> class.
		/// </summary>
		public ServiceException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ServiceException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public ServiceException(string errorMessage, Exception cause) : base(errorMessage, cause) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected ServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

	/// <summary>
	/// Exception indicating that a resource already exists.
	/// </summary>
	[Serializable]
	public class ResourceAlreadyExistsException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
		/// </summary>
		public ResourceAlreadyExistsException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ResourceAlreadyExistsException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public ResourceAlreadyExistsException(string errorMessage, Exception cause) : base(errorMessage, cause) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected ResourceAlreadyExistsException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

	/// <summary>
	/// Exception indicating that a resource is invalid.
	/// </summary>
	[Serializable]
	public class InvalidResourceException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResourceException"/> class.
		/// </summary>
		public InvalidResourceException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResourceException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public InvalidResourceException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResourceException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public InvalidResourceException(string errorMessage, Exception cause) : base(errorMessage, cause) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResourceException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected InvalidResourceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

	/// <summary>
	/// Indicates the request resource was not found.
	/// </summary>
	[Serializable]
	public class ResourceNotFoundException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		public ResourceNotFoundException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ResourceNotFoundException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public ResourceNotFoundException(string errorMessage, Exception cause) : base(errorMessage, cause) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected ResourceNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

	/// <summary>
	/// Indicates the resource is not allowed.
	/// </summary>
	[Serializable]
	public class ResourceNotAllowedException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		public ResourceNotAllowedException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ResourceNotAllowedException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public ResourceNotAllowedException(string errorMessage, Exception cause) : base(errorMessage, cause) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected ResourceNotAllowedException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

	/// <summary>
	/// Indicates that an updated version of the resource is available and must be retrived in
	/// order to complete the operation.
	/// </summary>
	[Serializable]
	public class ResourceDataInconsistentException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceDataInconsistentException"/> class.
		/// </summary>
		public ResourceDataInconsistentException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceDataInconsistentException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ResourceDataInconsistentException(string errorMessage) : base(errorMessage) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceDataInconsistentException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="cause">The cause.</param>
		public ResourceDataInconsistentException(string errorMessage, Exception cause) : base(errorMessage, cause) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceDataInconsistentException"/> class.
		/// </summary>
		/// <param name="serializationInfo">The serialization info.</param>
		/// <param name="streamingContext">The streaming context.</param>
		protected ResourceDataInconsistentException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
	}

}
