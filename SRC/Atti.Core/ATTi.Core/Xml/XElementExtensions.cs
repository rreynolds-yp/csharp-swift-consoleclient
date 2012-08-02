namespace ATTi.Core
{
	using System;
	using System.Xml.Linq;

	using ATTi.Core.Dto;
	using ATTi.Core.Contracts;

	public static class XElementExtensions
	{
		public static bool TryReadNamedValue(this XElement elm, string name, out bool value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (bool)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (bool)elm.Element(name);
				return true;
			}
			value = default(bool);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out bool value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out bool value, bool defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out bool value, Func<bool> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out byte value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = Convert.ToByte((int)elm.Attribute(name));
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = Convert.ToByte((int)elm.Element(name));
				return true;
			}
			value = default(byte);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out byte value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out byte value, byte defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out byte value, Func<byte> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out char value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = Convert.ToChar((int)elm.Attribute(name));
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = Convert.ToChar((int)elm.Element(name));
				return true;
			}
			value = default(char);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out char value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out char value, char defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out char value, Func<char> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out DateTime value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (DateTime)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (DateTime)elm.Element(name);
				return true;
			}
			value = default(DateTime);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out DateTime value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out DateTime value, DateTime defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out DateTime value, Func<DateTime> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out decimal value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (decimal)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (decimal)elm.Element(name);
				return true;
			}
			value = default(decimal);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out decimal value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out decimal value, decimal defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out decimal value, Func<decimal> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out double value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (double)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (double)elm.Element(name);
				return true;
			}
			value = default(double);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out double value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out double value, double defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out double value, Func<double> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out short value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (short)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (short)elm.Element(name);
				return true;
			}
			value = default(short);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out short value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out short value, short defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out short value, Func<short> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out int value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (int)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (int)elm.Element(name);
				return true;
			}
			value = default(int);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out int value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out int value, int defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out int value, Func<int> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out long value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (long)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (long)elm.Element(name);
				return true;
			}
			value = default(long);
			return false;
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out long value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out long value, long defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault(this XElement elm, string name, out long value, Func<long> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue<T>(this XElement elm, string name, out T value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = DataTransfer.FromXml<T>(elm.Element(name));
				return true;
			}
			value = default(T);
			return false;
		}

		public static void ReadNamedValueOrDefault<T>(this XElement elm, string name, out T value)
		{
			TryReadNamedValue(elm, name, out value);
		}

		public static void ReadNamedValueOrDefault<T>(this XElement elm, string name, out T value, T defa)
		{
			if (!TryReadNamedValue<T>(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefault<T>(this XElement elm, string name, out T value, Func<T> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		#region Enum

		public static bool TryReadNamedValueAsEnum<T>(this XElement elm, string name, out T value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);
			Require.IsAssignableFrom<Enum>("value", typeof(T));

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				string v = (string)elm.Attribute(name);
				if (Enum.IsDefined(typeof(T), v))
				{
					value = (T)Enum.Parse(typeof(T), v);
					return true;
				}
			}
			else if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				string v = (string)elm.Element(name);
				if (Enum.IsDefined(typeof(T), v))
				{
					value = (T)Enum.Parse(typeof(T), v);
					return true;
				}
			}
			value = default(T);
			return false;
		}

		public static void ReadNamedValueOrDefaultAsEnum<T>(this XElement elm, string name, out T value)
		{
			TryReadNamedValueAsEnum(elm, name, out value);
		}

		public static void ReadNamedValueOrDefaultAsEnum<T>(this XElement elm, string name, out T value, T defa)
		{
			if (!TryReadNamedValueAsEnum<T>(elm, name, out value))
			{
				value = defa;
			}
		}

		public static void ReadNamedValueOrDefaultAsEnum<T>(this XElement elm, string name, out T value, Func<T> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValueAsEnum(elm, name, out value))
			{
				value = defa();
			}
		}

		#endregion

		public static bool TryReadNamedValue(this XElement elm, string name, out sbyte value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (sbyte)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (sbyte)elm.Element(name);
				return true;
			}
			value = default(sbyte);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out sbyte value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out sbyte value, sbyte defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out sbyte value, Func<sbyte> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out float value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (Single)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (Single)elm.Element(name);
				return true;
			}
			value = default(Single);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out float value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out float value, float defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out float value, Func<float> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out string value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = elm.Attribute(name).Value;
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = elm.Element(name).Value;
				return true;
			}
			value = default(String);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out string value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out string value, string defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out string value, Func<string> defa)
		{
			Require.IsNotNull("defa", defa);
			
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}
		public static bool TryReadNamedValue(this XElement elm, string name, out ushort value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = Convert.ToUInt16((uint)elm.Attribute(name));
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = Convert.ToUInt16((uint)elm.Element(name));
				return true;
			}
			value = default(ushort);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ushort value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ushort value, ushort defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ushort value, Func<ushort> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out uint value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (uint)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (uint)elm.Element(name);
				return true;
			}
			value = default(uint);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out uint value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out uint value, uint defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out uint value, Func<uint> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		public static bool TryReadNamedValue(this XElement elm, string name, out ulong value)
		{
			Require.IsNotNull("elm", elm);
			Require.IsNotNull("name", name);

			if (elm.Attribute(name) != null && elm.Attribute(name).Value.Length > 0)
			{
				value = (ulong)elm.Attribute(name);
				return true;
			}
			if (elm.Element(name) != null && elm.Element(name).Value.Length > 0)
			{
				value = (ulong)elm.Element(name);
				return true;
			}
			value = default(ulong);
			return false;
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ulong value)
		{
			TryReadNamedValue(elm, name, out value);
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ulong value, ulong defa)
		{
			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa;
			}
		}
		public static void ReadNamedValueOrDefault(this XElement elm, string name, out ulong value, Func<ulong> defa)
		{
			Require.IsNotNull("defa", defa);

			if (!TryReadNamedValue(elm, name, out value))
			{
				value = defa();
			}
		}

		
	}
}