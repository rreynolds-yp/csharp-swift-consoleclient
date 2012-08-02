namespace ATTi.Core.Reflection
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Utility class containing extension methods for the Assembly class.
	/// </summary>
	public static class AssemblyExtensions
	{
		

		/// <summary>
		/// Gets a surrogate lock object for an assembly.
		/// </summary>
		/// <param name="asm">The assembly for which we need a surrogate lock.</param>
		/// <returns>An object for operations that need to be synchronized on the assembly.</returns>
		public static Object GetSafeLock(this Assembly asm)
		{
			string fn = asm.FullName;
			if (String.IsInterned(fn) == null)
			{
				return String.Intern(fn);
			}
			return fn;
		}		
	}
}