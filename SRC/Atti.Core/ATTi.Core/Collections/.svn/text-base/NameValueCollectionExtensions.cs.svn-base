using System;
using System.Collections.Specialized;
using System.Globalization;

namespace ATTi.Core.Collections
{
	public static class NameValueCollectionExtensions
	{
		public static bool TryGetBooleanValue(this NameValueCollection tthis, string name, out bool value)
		{
			string val = tthis.Get(name);
			if (val != null)
			{
				if (String.Equals("true", val, StringComparison.InvariantCultureIgnoreCase)
					|| String.Equals("yes", val, StringComparison.InvariantCultureIgnoreCase)
					|| String.Equals("0", val))
				{
					value = true;
					return true;
				}
				else if (String.Equals("false", val, StringComparison.InvariantCultureIgnoreCase)
					|| String.Equals("no", val, StringComparison.InvariantCultureIgnoreCase)
					|| String.Equals("1", val))
				{
					value = false;
					return true;
				}
			}
			value = default(bool);
			return false;
		}
		public static bool TryGetPositiveValue(this NameValueCollection tthis, string name, out int value)
		{
			string val = tthis.Get(name);
			if (val != null)
			{
				int num;
				try
				{
					num = Convert.ToInt32(val, CultureInfo.InvariantCulture);
					if (num >= 0)
					{
						value = num;
						return true;
					}
				}
				catch (FormatException)
				{					
				}
			}
			value = default(int);
			return false;
		}
		public static bool TryGetPositiveOrInfiniteValue(this NameValueCollection tthis, string name, out int value)
		{
			string val = tthis.Get(name);
			if (val != null)
			{
				int num;
				if (String.Equals("Infinite", val, StringComparison.InvariantCultureIgnoreCase))
				{
					value = 0x7FFFFFFF;
					return true;
				}
				try
				{
					num = Convert.ToInt32(val, CultureInfo.InvariantCulture);
					if (num >= 0)
					{
						value = num;
						return true;
					}
				}
				catch (Exception)
				{ // either FormatException or OverflowException... ignored to return false.
				}
			}
			value = default(int);
			return false;
		}

		public static bool TryGetAndRemoveBooleanValue(this NameValueCollection tthis, string name, out bool value)
		{			
			if (TryGetBooleanValue(tthis, name, out value))
			{
				tthis.Remove(name);
				return true;
			}
			return false;
		}



	}
}
