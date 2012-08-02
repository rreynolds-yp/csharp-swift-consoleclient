using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ATTi.Core.Data.SqlServer;
using ATTi.Core.Contracts;

namespace ATTi.Core.Data
{
	public static class DbParameterBinders
	{
		public delegate DbParameterBinder BinderFactoryMethod(DbCommand cmd);

		private static Object __binderFactoriesLock = new Object();
		private static Dictionary<Type, BinderFactoryMethod> __binderFactories = new Dictionary<Type, BinderFactoryMethod>();
		private static BinderWireup __wireup = new BinderWireup();

		static DbParameterBinders()
		{
			__binderFactories.Add(typeof(SqlCommand), (DbCommand cmd) => { return new SqlParameterBinder(cmd); });
		}

		public class BinderWireup
		{
			public void RegisterBinderFactoryMethod<TDbCommand>(BinderFactoryMethod factory)
				where TDbCommand : DbCommand
			{
				__binderFactories.Add(typeof(TDbCommand), factory);
			}
		}

		public static BinderWireup Wireup
		{
			get
			{
				return __wireup;
			}
		}

		public static DbParameterBinder GetBinderForDbCommand(DbCommand cmd)
		{
			BinderFactoryMethod factory;
			bool hasFactory = false;
			lock (__binderFactoriesLock)
			{
				hasFactory = __binderFactories.TryGetValue(cmd.GetType(), out factory);
			}
			return (hasFactory) ? factory(cmd) : null;
		}
	}

	public abstract class DbParameterBinder
	{
		private readonly DbCommand _cmd;

		protected DbCommand DbCommand { get { return _cmd; } }

		protected DbParameterBinder(DbCommand cmd)
		{
			Require.IsNotNull("cmd", cmd);
			_cmd = cmd;
		}

		public abstract DbTypeTranslation TranslateDbType(DbType dbType);

		public abstract DbParameter DefineParameter(string parameterName, DbType dbType, ParameterDirection direction, int length);
		public abstract DbParameter DefineParameter(string parameterName, DbType dbType, ParameterDirection direction);
		public virtual DbParameter GetDbParameterByName(string parameterName)
		{
			Require.IsNotNull("parameterName", parameterName);

			return _cmd.Parameters[parameterName];
		}

		public void DefineAndBindParameter<T>(string parameterName, DbType dbType, ParameterDirection direction, T value)
		{
			Require.IsNotNull("parameterName", parameterName);

			DbParameter p = DefineParameter(parameterName, dbType, direction);
			p.Value = value;
		}

		public void DefineOutParameter(string parameterName, DbType dbType, ParameterDirection direction)
		{
			Require.IsNotNull("parameterName", parameterName);
			Require.AssertState((direction & ParameterDirection.Output) == ParameterDirection.Output,
				"direction must include ParameterDirection.Output");

			DefineParameter(parameterName, dbType, direction);
		}

		public void DefineAndBindParameter(string parameterName, DbType dbType, ParameterDirection direction, int length, string value)
		{
			Require.IsNotNull("parameterName", parameterName);

			DbParameter p = DefineParameter(parameterName, dbType, direction, length);
			p.Value = value;
		}

		public virtual void GetOutputParam<T>(string parameterName, out T result)
		{
			object v = GetDbParameterByName(parameterName).Value;

			result = (T)v;
		}
	}

}
