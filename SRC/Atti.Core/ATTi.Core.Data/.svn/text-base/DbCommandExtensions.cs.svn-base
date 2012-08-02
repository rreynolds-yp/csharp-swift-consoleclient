using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using ATTi.Core.Contracts;
using ATTi.Core.Trace;

namespace ATTi.Core.Data
{
	public static class DbCommandExtensions
	{
		/// <summary>
		/// Executes a DbCommand and returns an iterator over the results.
		/// </summary>
		/// <param name="cmd">The command being executed.</param>
		/// <param name="behavior">Behavior to be used when executing the command.</param>
		/// <returns>DbDataReader for each row in the results.</returns>
		public static IEnumerable<DbDataReader> ExecuteEnumerable(this DbCommand cmd, CommandBehavior behavior)
		{
			using (DbDataReader reader = cmd.ExecuteReader(behavior))
			{
				while (reader.Read())
				{
					yield return reader;
				}
				reader.Close();
			}
		}

		public static IEnumerable<DbDataReader> ExecuteEnumerable(this DbCommand cmd, CleanupScope scope, CommandBehavior behavior)
		{
			if (scope == null) throw new ArgumentNullException("scope");

			DbDataReader reader = scope.AddWithNewDisposer(cmd.ExecuteReader(behavior));
			while (reader.Read())
			{
				yield return reader;
			}
			reader.Close();
			scope.Dispose();
		}

		/// <summary>
		/// Executes a DbCommand and returns an iterator over transformed results.
		/// </summary>
		/// <typeparam name="T">Target type for the transformation.</typeparam>
		/// <param name="cmd">The command being executed.</param>
		/// <param name="behavior">Behavior to be used when executing the command.</param>
		/// <param name="transform">Transform delegate taking the DbDataReader for each row and returning an instance of type T.</param>
		/// <returns>A type T for each row in the results.</returns>
		public static IEnumerable<T> ExecuteAndTransform<T>(this DbCommand cmd, CommandBehavior behavior, Func<DbDataReader, T> transform)
		{
			using (DbDataReader reader = cmd.ExecuteReader(behavior))
			{
				while (reader.Read())
				{
					T item = transform(reader);
					yield return item;
				}
				reader.Close();
			}
		}
		public static IEnumerable<T> ExecuteAndTransform<T>(this DbCommand cmd, CleanupScope scope, CommandBehavior behavior, Func<DbDataReader, T> transform)
		{
			if (scope == null) throw new ArgumentNullException("scope");
			if (transform == null) throw new ArgumentNullException("transform");

			DbDataReader reader = scope.AddWithNewDisposer(cmd.ExecuteReader(behavior));
			try
			{
				while (reader.Read())
				{
					T item = transform(reader);
					yield return item;
				}
				reader.Close();
			}
			finally
			{
				scope.Dispose();
			}
		}

		public static T ExecuteSingle<T>(this DbCommand cmd
			, CommandBehavior behavior
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbCommandExtensions), TraceEventType.Verbose, cmd);
			using (DbDataReader reader = cmd.ExecuteReader(behavior))
			{
				if (reader.Read())
				{
					result = transform(reader);
					if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
					return result;
				}
				else throw new InvalidOperationException("The input sequence is empty.");
			}
		}

		public static T ExecuteSingleOrDefault<T>(this DbCommand cmd
			, CommandBehavior behavior
			, Func<DbDataReader, T> transform)
		{
			T result = default(T);
			Traceable.TraceData(typeof(DbCommandExtensions), TraceEventType.Verbose, cmd);
			using (DbDataReader reader = cmd.ExecuteReader(behavior))
			{
				if (reader.Read())
				{
					result = transform(reader);
					if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
				}
				reader.Close();
			}
			return result;
		}
	}
	
}
