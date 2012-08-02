using System;
using Newtonsoft.Json.Linq;
using ATTi.Core.Dto;
using ATTi.Core.Contracts;
using System.Collections.Generic;

namespace ATTi.Core.Dto.Json
{
	public static class JObjectExtensions
	{
		public static bool TryReadNamedValue(this JObject obj, string name, out bool value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Boolean
				|| typ == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (bool)p.Value;
				return true;
			}
			value = default(bool);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out bool[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<bool> items = new List<bool>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<bool>());
				}
				value = items.ToArray();
				return true;
			}
			
			value = default(bool[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out byte value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = Convert.ToByte((uint)p.Value);
				return true;
			}
			value = default(byte);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out byte[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<byte> items = new List<byte>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<byte>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(byte[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out char value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.String)
			{
				// Take the first character
				value = ((string)p.Value)[0];
				return true;
			}
			value = default(char);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out char[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<char> items = new List<char>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<char>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(char[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out DateTime value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Date)
			{
				value = (DateTime)p.Value;
				return true;
			}
			value = default(DateTime);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out DateTime[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<DateTime> items = new List<DateTime>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<DateTime>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(DateTime[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out decimal value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);
			
			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Float
				|| typ == JTokenType.String))
			{
				value = (decimal)p.Value;
				return true;
			}
			value = default(decimal);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out decimal[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<decimal> items = new List<decimal>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<decimal>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(decimal[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out double value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Float
				|| typ == JTokenType.String))
			{
				value = (double)p.Value;
				return true;
			}
			value = default(double);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out double[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<double> items = new List<double>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<double>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(double[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out short value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = Convert.ToInt16((int)p.Value);
				return true;
			}
			value = default(short);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out short[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<short> items = new List<short>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<short>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(short[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out int value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (int)p.Value;
				return true;
			}
			value = default(int);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out int[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<int> items = new List<int>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<int>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(int[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out long value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (long)p.Value;
				return true;
			}
			value = default(long);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out long[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<long>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<long>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(long[]);
			return false;
		}
		public static bool TryReadNamedValueAsArray<T>(this JObject obj, string name, out T[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				List<T> items = new List<T>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(DataTransfer.FromJson<T>(elm as JObject));
				}
				value = items.ToArray();
				return true;
			}
			value = default(T[]);
			return false;
		}
		public static bool TryReadNamedValue<T>(this JObject obj, string name, out T value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Object)
			{
				value =  DataTransfer.FromJson<T>(p.Value as JObject);
				return true;
			}
			value = default(T);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out sbyte value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (sbyte)p.Value;
				return true;
			}
			value = default(sbyte);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out sbyte[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<sbyte>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<sbyte>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(sbyte[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out float value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Float
				|| typ == JTokenType.String))
			{
				value = (float)p.Value;
				return true;
			}
			value = default(Single);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out float[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<float>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<float>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(float[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out string value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.String)
			{
				value = (string)p.Value;
				return true;
			}
			value = default(String);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out string[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<string>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<string>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(string[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out ushort value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = Convert.ToUInt16((uint)p.Value);
				return true;
			}
			value = default(ushort);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out ushort[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<ushort>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<ushort>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(ushort[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out uint value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (uint)p.Value;
				return true;
			}
			value = default(uint);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out uint[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<uint>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<uint>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(uint[]);
			return false;
		}
		public static bool TryReadNamedValue(this JObject obj, string name, out ulong value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			JTokenType typ;
			if (p != null
				&& ((typ = p.Value.Type) == JTokenType.Integer
				|| typ == JTokenType.String))
			{
				value = (ulong)p.Value;
				return true;
			}
			value = default(ulong);
			return false;
		}
		public static bool TryReadNamedValueAsArray(this JObject obj, string name, out ulong[] value)
		{
			Require.IsNotNull("obj", obj);
			Require.IsNotNull("name", name);

			JProperty p = obj.Property(name);
			if (p != null && p.Value.Type == JTokenType.Array)
			{
				var items = new List<ulong>();
				JArray arr = p.Value as JArray;
				foreach (var elm in arr)
				{
					items.Add(elm.Value<ulong>());
				}
				value = items.ToArray();
				return true;
			}

			value = default(ulong[]);
			return false;
		}
	}
}
