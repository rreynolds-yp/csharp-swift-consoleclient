using System.Data.Common;
using System.Collections.Generic;
using System;
using ATTi.Core.Contracts;

namespace ATTi.Core.Data
{
	public static class DbDataReaderExtensions
	{
		public static T GetValueOrDefault<T>(this DbDataReader reader, int ordinal)
		{
			return (T)((reader.IsDBNull(ordinal)) ? default(T) : reader.GetValue(ordinal));
		}
		public static T GetValueOrDefault<T>(this DbDataReader reader, int ordinal, T defa)
		{
			return (T)((reader.IsDBNull(ordinal)) ? defa : reader.GetValue(ordinal));
		}

		public static IEnumerable<T> EnumerateAndTransform<T>(this DbDataReader reader, Func<DbDataReader, T> transform)
		{
			Require.IsNotNull("reader", reader);
			Require.IsNotNull("transform", transform);

			List<T> result = new List<T>();
			while (reader.Read())
			{
				result.Add(transform(reader));
			}
			return result;
		}

		public static TCollection EnumerateAndTransform<T, TCollection>(this DbDataReader reader, Func<DbDataReader, T> transform)
			where TCollection : class, ICollection<T>, new()
		{
			Require.IsNotNull("reader", reader);
			Require.IsNotNull("transform", transform);

			TCollection result = new TCollection();
			while (reader.Read())
			{
				result.Add(transform(reader));
			}
			return result;
		}
	}
}
