
namespace ATTi.Core.Trace
{
	/// <summary>
	/// Enables tracing extensions for a class.
	/// </summary>
	/// <remarks>
	/// By adding this marker interface to a class the class will be able to
	/// trace via the trace extensions. To control which trace messages are
	/// emitted to the underlying framework use the .NET framework's 
	/// </remarks>
	public interface ITraceable
	{
	}
}