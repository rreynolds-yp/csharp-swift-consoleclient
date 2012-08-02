namespace ATTi.Core
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Various extension methods.
	/// </summary>
	public static class ExtensionMethods
	{
		
		/// <summary>
		/// Gets the fully qualified, human readable name for a delegate.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static string GetFullName(this Delegate d)
		{
			return String.Concat(d.Target.GetType().FullName, ".", d.Method.Name, "()");
		}

		/// <summary>
		/// Removes a string from the end of another string if present.
		/// </summary>
		/// <param name="target">The target string.</param>
		/// <param name="value">The value to remove.</param>
		/// <returns>the target string with the value removed</returns>
		public static string RemoveTrailing(this string target, string value)
		{
			if (!String.IsNullOrEmpty(target) && !String.IsNullOrEmpty(value)
				&& target.EndsWith(value))
			{
				return target.Substring(0, target.Length - value.Length);
			}
			return target;
		}

		/// <summary>
		/// Interns a string if it is not already interned.
		/// </summary>
		/// <param name="target">the target string</param>
		/// <returns>the target string interned</returns>
		public static string InternIt(this string target)
		{
			if (String.IsInterned(target) == null)
			{
				return String.Intern(target);
			}
			return target;
		}

		/// <summary>
		/// Determines if the arrays are equal or if the items in two different arrays
		/// are equal.
		/// </summary>
		/// <typeparam name="T">Item type T</typeparam>
		/// <param name="lhs">Left-hand comparand</param>
		/// <param name="rhs">Right-hand comparand</param>
		/// <returns><b>true</b> if the arrays are equal or if the items in the arrays are equal.</returns>
		public static bool EqualsOrItemsEqual<T>(this T[] lhs, T[] rhs)
		{
			bool result = Object.Equals(lhs, rhs);
			if (result == false && lhs != null && rhs != null
				&& lhs.LongLength == rhs.LongLength)
			{
				IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
				#region Unroll the loop (within reason)
				switch (lhs.LongLength)
				{
					case 0: return true;
					case 1: return comparer.Equals(lhs[0], rhs[0]);
					case 2: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1]);
					case 3: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2]);
					case 4: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3]);
					case 5: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4]);
					case 6: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4])
						&& comparer.Equals(lhs[5], rhs[5]);
					case 7: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4])
						&& comparer.Equals(lhs[5], rhs[5])
						&& comparer.Equals(lhs[6], rhs[6]);
					case 8: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4])
						&& comparer.Equals(lhs[5], rhs[5])
						&& comparer.Equals(lhs[6], rhs[6])
						&& comparer.Equals(lhs[7], rhs[7]);
					case 9: return comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4])
						&& comparer.Equals(lhs[5], rhs[5])
						&& comparer.Equals(lhs[6], rhs[6])
						&& comparer.Equals(lhs[7], rhs[7])
						&& comparer.Equals(lhs[8], rhs[8]);
					default:
						result = comparer.Equals(lhs[0], rhs[0])
						&& comparer.Equals(lhs[1], rhs[1])
						&& comparer.Equals(lhs[2], rhs[2])
						&& comparer.Equals(lhs[3], rhs[3])
						&& comparer.Equals(lhs[4], rhs[4])
						&& comparer.Equals(lhs[5], rhs[5])
						&& comparer.Equals(lhs[6], rhs[6])
						&& comparer.Equals(lhs[7], rhs[7])
						&& comparer.Equals(lhs[8], rhs[8])
						&& comparer.Equals(lhs[9], rhs[9]);
						if (result && lhs.LongLength > 10)
						{
							for (int i = 10; i < lhs.LongLength; i++)
							{
								result = comparer.Equals(lhs[i], rhs[i]);
								if (!result) break;
							}
						}
						break;
				}
				#endregion
			}
			return result;
		}

		/// <summary>
		/// Formats the exception for logging.
		/// </summary>
		/// <param name="ex">The ex.</param>
		/// <returns></returns>
		public static string FormatForLogging(this Exception ex)
		{
			return String.Concat(ex.GetType().FullName, ": ", ex.Message,
				Environment.NewLine, "\t", ex.StackTrace, 
				(ex.InnerException != null) 
				? String.Concat(Environment.NewLine, "\t>> InnerException: ", ex.InnerException.FormatForLogging())
				: String.Empty
				);
		}
	}
}