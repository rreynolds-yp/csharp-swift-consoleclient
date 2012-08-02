using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using ATTi.Core.Trace;
using ATTi.Core.Contracts;
using ATTi.Core.Parallel;

namespace ATTi.Core.Data
{
	public static class DbConnectionExtensions
	{
		/// <summary>
		/// Creates and initializes a DbCommand instance.
		/// </summary>
		/// <param name="cn">Connection used to create the command.</param>
		/// <param name="cmdText">The new command's CommandText.</param>
		/// <param name="cmdType">The new command's CommandType.</param>
		/// <param name="cmdTimeout">The new command's CommandTimeout.</param>
		/// <returns>Newly created DbCommand initialized with the values given.</returns>
		public static DbCommand CreateCommand(this DbConnection cn, string cmdText, CommandType cmdType, int cmdTimeout)
		{
			DbCommand cmd = cn.CreateCommand();
			cmd.CommandText = cmdText;
			cmd.CommandTimeout = cmdTimeout;
			cmd.CommandType = cmdType;
			return cmd;
		}
		/// <summary>
		/// Creates and initializes a DbCommand instance.
		/// </summary>
		/// <param name="cn">Connection used to create the command.</param>
		/// <param name="cmdText">The new command's CommandText.</param>
		/// <param name="cmdType">The new command's CommandType.</param>
		/// <returns>Newly created DbCommand initialized with the values given.</returns>
		public static DbCommand CreateCommand(this DbConnection cn, string cmdText, CommandType cmdType)
		{
			DbCommand cmd = cn.CreateCommand();
			cmd.CommandText = cmdText;
			cmd.CommandType = cmdType;
			return cmd;
		}

		/// <summary>
		/// Creates and initializes a DbCommand instance for the specified command text.
		/// </summary>
		/// <param name="cn">Connection used to create the command.</param>
		/// <param name="cmdText">The new command's CommandText.</param>
		/// <returns>A newly created DbCommand instance initialized with the specified command text.</returns>
		public static DbCommand CreateCommand(this DbConnection cn, string cmdText)
		{
			DbCommand cmd = cn.CreateCommand();
			cmd.CommandText = cmdText;
			return cmd;
		}

		/// <summary>
		/// Executes a non-query command on the connection using the specified command text, command type, and command timeout.
		/// </summary>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="cmdTimeout">A timeout for the command (in milliseconds).</param>
		/// <returns>The number of rows affected.</returns>
		public static int ImmediateExecuteNonQuery(this DbConnection cn, string cmdText, CommandType cmdType, int cmdTimeout)
		{
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType, cmdTimeout))
			{
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a non-query command on the connection using the specified command text and command type.
		/// </summary>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <returns>The number of rows affected.</returns>
		public static int ImmediateExecuteNonQuery(this DbConnection cn, string cmdText, CommandType cmdType)
		{
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a non-query command on the connection using the specified command text.
		/// </summary>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <returns>The number of rows affected.</returns>
		public static int ImmediateExecuteNonQuery(this DbConnection cn, string cmdText)
		{
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				return cmd.ExecuteNonQuery();
			}
		}

		public static int ImmediateExecuteNonQueryWithCommandParameters(this DbConnection cn, string cmdText
			, Action<DbParameterBinder> parameterBinder)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));
					//cmd.Prepare();
				}
				return cmd.ExecuteNonQuery();
			}
		}

		public static int ImmediateExecuteNonQueryWithCommandParametersAndOutputParams(this DbConnection cn, string cmdText
			, Action<DbParameterBinder> parameterBinder
			, Action<DbParameterBinder> parameterReader)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				DbParameterBinder binder = DbParameterBinders.GetBinderForDbCommand(cmd);
				if (parameterBinder != null)
				{
					parameterBinder(binder);
					//cmd.Prepare();
				}
				int result = cmd.ExecuteNonQuery();
				if (result != 0 && parameterReader != null) parameterReader(binder);
				return result;					
			}
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise an exception is thrown.</returns>
		public static T ImmediateExecuteSingle<T>(this DbConnection cn
			, string cmdText
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise an exception is thrown.</returns>
		public static T ImmediateExecuteSingle<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		public static T ImmediateExecuteSingleWithCommandParameters<T>(this DbConnection cn
			, string cmdText
			, Action<DbParameterBinder> parameterBinder
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));					
				}
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		public static T ImmediateExecuteSingleWithCommandParameters<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, Action<DbParameterBinder> parameterBinder
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));
				}
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		public static T ImmediateExecuteSingleWithCommandParametersForMultipleResults<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, Action<DbParameterBinder> parameterBinder
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));
				}
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		public static T ImmediateExecuteSingleOrDefaultWithCommandParametersForMultipleResults<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, Action<DbParameterBinder> parameterBinder
			, Func<DbDataReader, T> transform)
		{
			T result = default(T);
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));
				}
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						result = transform(reader);
						if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="cmdTimeout">A timeout for the command (in milliseconds).</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise an exception is thrown.</returns>
		public static T ImmediateExecuteSingle<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, int cmdTimeout
			, Func<DbDataReader, T> transform)
		{
			T result;
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType, cmdTimeout))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
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
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T. 
		/// If a result does not exist then default(T) is returned.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="cmdTimeout">A timeout for the command (in milliseconds).</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise default(T).</returns>
		public static T ImmediateExecuteSingleOrDefault<T>(this DbConnection cn
			, string cmdText
			, Func<DbDataReader, T> transform)
		{
			T result = default(T);
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						result = transform(reader);
						if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
					}
					reader.Close();
				}
			}
			return result;
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T. 
		/// If a result does not exist then default(T) is returned.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="cmdTimeout">A timeout for the command (in milliseconds).</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise default(T).</returns>
		public static T ImmediateExecuteSingleOrDefault<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, Func<DbDataReader, T> transform)
		{
			T result = default(T);
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						result = transform(reader);
						if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
					}
					reader.Close();
				}
			}
			return result;
		}

		/// <summary>
		/// Executes the specified command immediately and returns a result transformed to type T. 
		/// If a result does not exist then default(T) is returned.
		/// </summary>
		/// <typeparam name="T">Type of the transformed result.</typeparam>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The non-query command text.</param>
		/// <param name="cmdType">The type of command being executed.</param>
		/// <param name="cmdTimeout">A timeout for the command (in milliseconds).</param>
		/// <param name="transform">A delegate that will transform the result.</param>
		/// <returns>A single result of type T, otherwise default(T).</returns>
		public static T ImmediateExecuteSingleOrDefault<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, int cmdTimeout
			, Func<DbDataReader, T> transform)
		{
			T result = default(T);
			Traceable.TraceData(typeof(DbConnectionExtensions), TraceEventType.Verbose, cmdText);
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType, cmdTimeout))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						result = transform(reader);
						if (reader.Read()) throw new InvalidOperationException("The input sequence contains more than one element.");
					}
					reader.Close();
				}
			}
			return result;
		}

		/// <summary>
		/// Executes a command on the connection using the specified command text and enumerates the result.
		/// </summary>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The command text.</param>
		/// <returns>An enumeration over the rows (DbDataReader).</returns>
		public static IEnumerable<DbDataReader> ImmediateExecuteEnumerable(this DbConnection cn, string cmdText)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return reader;
					}
				}
			}
		}
		public static IEnumerable<DbDataReader> ImmediateExecuteEnumerable(this DbConnection cn, CleanupScope scope, string cmdText)
		{
			if (scope == null) throw new ArgumentNullException("scope");
			if (cmdText == null) throw new ArgumentNullException("cmdText");
			
			return scope.Add(cn.CreateCommand(cmdText)).ExecuteEnumerable(scope, CommandBehavior.Default);
		}

		public static IEnumerable<DbDataReader> ImmediateExecuteEnumerableWithCommandParameters(this DbConnection cn, string cmdText
			, Action<DbParameterBinder> parameterBinder)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				if (parameterBinder != null)
				{
					parameterBinder(DbParameterBinders.GetBinderForDbCommand(cmd));
					//cmd.Prepare();
				}
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return reader;
					}
				}
			}
		}

		/// <summary>
		/// Executes a command on the connection using the specified command text, transforms the results with the specified delegate, 
		/// and enumerates the results.
		/// </summary>
		/// <param name="cn">Connection used to execute the specified command.</param>
		/// <param name="cmdText">The command text.</param>
		/// <returns>An enumeration over the transformed results (Of T).</returns>
		public static IEnumerable<T> ImmediateExecuteAndTransform<T>(this DbConnection cn
			, string cmdText
			, CommandType cmdType
			, int cmdTimeout
			, Func<DbDataReader, T> transform)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText, cmdType, cmdTimeout))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return transform(reader);
					}
				}
			}
		}
		public static IEnumerable<T> ImmediateExecuteAndTransform<T>(this DbConnection cn
			, string cmdText
			, Func<DbDataReader, T> transform)
		{
			using (DbCommand cmd = cn.CreateCommand(cmdText))
			{
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return transform(reader);
					}
				}
			}
		}
		public static IEnumerable<T> ImmediateExecuteAndTransform<T>(this DbConnection cn
			, CleanupScope scope
			, string cmdText
			, CommandType cmdType
			, int cmdTimeout
			, Func<DbDataReader, T> transform)
		{
			if (scope == null) throw new ArgumentNullException("scope");

			DbCommand cmd = scope.Add(cn.CreateCommand(cmdText, cmdType, cmdTimeout));
			return cmd.ExecuteAndTransform(scope, CommandBehavior.Default, transform);
		}
		public static IEnumerable<T> ImmediateExecuteAndTransform<T>(this DbConnection cn
			, CleanupScope scope
			, string cmdText
			, Func<DbDataReader, T> transform)
		{
			if (scope == null) throw new ArgumentNullException("scope");
			if (cmdText == null) throw new ArgumentNullException("cmdText");
			if (transform == null) throw new ArgumentNullException("transform");

			DbCommand cmd = scope.Add(cn.CreateCommand(cmdText));

			DbDataReader reader = scope.AddWithNewDisposer(cmd.ExecuteReader(CommandBehavior.SequentialAccess));
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

		public static IFuture<IEnumerable<T>> AsyncExecuteAndTransform<T>(this DbConnection cn
			, CleanupScope scope
			, string cmdText
			, Func<DbDataReader, T> transform
			, Action<IEnumerable<T>> completionCallback)
		{
			Require.IsNotNull("scope", scope);
			Require.IsNotNull("cmdText", cmdText);
			Require.IsNotNull("transform", transform);

			ParallelFuncWrapper<ThreadPoolExecutionStrategy, IEnumerable<T>> result = new ParallelFuncWrapper<ThreadPoolExecutionStrategy, IEnumerable<T>>(() =>
				{
					List<T> items = new List<T>(scope.Add(cn.CreateCommand(cmdText)).ExecuteAndTransform<T>(CommandBehavior.Default, transform));
					if (completionCallback != null) completionCallback(items);
					return items;
				});

			result.AsyncExecute();
			return result.FutureResult;
		}
	}
}
