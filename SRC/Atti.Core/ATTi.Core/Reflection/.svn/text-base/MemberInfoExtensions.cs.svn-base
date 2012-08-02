namespace ATTi.Core.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Contains extension methods for the MemberInfo class.
	/// </summary>
	public static class MemberInfoExtensions
	{
		

		public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member, bool inherit)
			where TAttribute : Attribute
		{
			TAttribute result = null;
			if (member.IsDefined(typeof(TAttribute), inherit))
			{
				object[] attribs = member.GetCustomAttributes(typeof(TAttribute), inherit);
				result = attribs[0] as TAttribute;
			}
			return result;
		}

		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member, bool inherit)
			where TAttribute : Attribute
		{
			if (member.IsDefined(typeof(TAttribute), inherit))
			{
				return (TAttribute[])member.GetCustomAttributes(typeof(TAttribute), inherit);
			}
			return new TAttribute[0];
		}

		
	}
}