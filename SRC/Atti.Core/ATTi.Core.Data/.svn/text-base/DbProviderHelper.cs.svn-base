using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using ATTi.Core.Data.SqlServer;
using ATTi.Core.Contracts;
using System.Threading;

namespace ATTi.Core.Data
{
	public static class DbProviderHelpers
	{
		public delegate DbProviderHelper HelperFactoryMethod(DbConnection cn);

		private static Object __helperFactoriesLock = new Object();
		private static Dictionary<Type, HelperFactoryMethod> __helperFactories = new Dictionary<Type, HelperFactoryMethod>();

		static DbProviderHelpers()
		{
			__helperFactories.Add(typeof(SqlConnection), (DbConnection cn) => { return new SqlDbProviderHelper(); });
		}

		public static DbProviderHelper GetDbProviderHelperForDbConnection(DbConnection cn)
		{
			if (cn == null) throw new ArgumentNullException("cn");

			HelperFactoryMethod factory;
			bool hasFactory = false;
			lock (__helperFactoriesLock)
			{
				hasFactory = __helperFactories.TryGetValue(cn.GetType(), out factory);
			}
			return (hasFactory) ? factory(cn) : null;
		}
	}

	public abstract class DbProviderHelper
	{
		public DbProviderFactory Factory { get; protected set; }

		public virtual bool SupportsMultipleActiveResultSets(DbConnection cn)
		{
			return false;
		}

		public abstract bool SupportsAsync { get; }

		public bool SchemaExists(DbConnection cn, string catalogName, string schemaName)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");

			return (cn.ImmediateExecuteSingleOrDefault<string>(
				FormatSchemaExistsQuery(catalogName, schemaName)
				, (DbDataReader reader) => { return reader.GetValueOrDefault(0, (string)null); }) != null);
		}

		protected virtual string FormatSchemaExistsQuery(string catalogName, string schemaName)
		{
			return String.Format(@"SELECT SCHEMA_NAME
FROM INFORMATION_SCHEMA.SCHEMATA
WHERE CATALOG_NAME = '{0}'
  AND SCHEMA_NAME = '{1}'", catalogName, schemaName);
		}

		public bool TableExists(DbConnection cn, string catalogName, string schemaName, string tableName)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");
			if (tableName == null) throw new ArgumentNullException("tableName");
			if (tableName.Length == 0) throw new ArgumentException("tableName");

			return (cn.ImmediateExecuteSingleOrDefault<string>(
				FormatTableExistsQuery(catalogName, schemaName, tableName)
				, (DbDataReader reader) => { return reader.GetValueOrDefault<string>(0); }) != null);
		}

		protected virtual string FormatTableExistsQuery(string catalogName, string schemaName, string tableName)
		{
			return String.Format(@"SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_CATALOG = '{0}'
  AND TABLE_SCHEMA = '{1}'
  AND TABLE_NAME = '{2}'
  AND TABLE_TYPE = 'BASE TABLE'", catalogName, schemaName, tableName);
		}

		public bool ViewExists(DbConnection cn, string catalogName, string schemaName, string viewName)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");
			if (viewName == null) throw new ArgumentNullException("viewName");
			if (viewName.Length == 0) throw new ArgumentException("viewName");

			return (cn.ImmediateExecuteSingleOrDefault<string>(
				FormatViewExistsQuery(catalogName, schemaName, viewName)
				, (DbDataReader reader) => { return reader.GetValueOrDefault<string>(0); }) != null);
		}

		protected virtual string FormatViewExistsQuery(string catalogName, string schemaName, string viewName)
		{
			return String.Format(@"SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.VIEWS
WHERE TABLE_CATALOG = '{0}'
  AND TABLE_SCHEMA = '{1}'
  AND TABLE_NAME = '{2}'", catalogName, schemaName, viewName);
		}

		public bool StoredProcedureExists(DbConnection cn, string catalogName, string schemaName, string storedProcedureName)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");
			if (storedProcedureName == null) throw new ArgumentNullException("storedProcedureName");
			if (storedProcedureName.Length == 0) throw new ArgumentException("storedProcedureName");

			return (cn.ImmediateExecuteSingleOrDefault<string>(
				FormatProcedureExistsQuery(catalogName, schemaName, storedProcedureName)
				, (DbDataReader reader) => { return reader.GetValueOrDefault<string>(0); }) != null);
		}

		protected virtual string FormatProcedureExistsQuery(string catalogName, string schemaName, string storedProcedureName)
		{
			return String.Format(@"SELECT ROUTINE_NAME
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_CATALOG = '{0}'
  AND ROUTINE_SCHEMA = '{1}'
  AND ROUTINE_NAME = '{2}'
  AND ROUTINE_TYPE = 'PROCEDURE'", catalogName, schemaName, storedProcedureName);
		}

		public bool FunctionExists(DbConnection cn, string catalogName, string schemaName, string functionName)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");
			if (functionName == null) throw new ArgumentNullException("functionName");
			if (functionName.Length == 0) throw new ArgumentException("functionName");

			return (cn.ImmediateExecuteSingleOrDefault<string>(
				FormatFunctionExistsQuery(catalogName, schemaName, functionName)
				, (DbDataReader reader) => { return reader.GetValueOrDefault<string>(0); }) != null);
		}

		protected virtual string FormatFunctionExistsQuery(string catalogName, string schemaName, string functionName)
		{
			return String.Format(@"SELECT ROUTINE_NAME
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_CATALOG = '{0}'
  AND ROUTINE_SCHEMA = '{1}'
  AND ROUTINE_NAME = '{2}'
  AND ROUTINE_TYPE = 'FUNCTION'", catalogName, schemaName, functionName);
		}

		public virtual string GetServerName(DbConnection cn)
		{
			if (cn == null) throw new ArgumentNullException("cn");

			return cn.DataSource;
		}

		public void CreateSchema(DbConnection cn, string catalogName, string schemaName)
		{
			if (cn == null) throw new ArgumentNullException("cn");

			if (catalogName == null) throw new ArgumentNullException("catalogName");
			if (catalogName.Length == 0) throw new ArgumentException("catalogName");
			if (schemaName == null) throw new ArgumentNullException("schemaName");
			if (schemaName.Length == 0) throw new ArgumentException("schemaName");
			cn.ImmediateExecuteNonQuery(FormatCreateSchemaCommandText(catalogName, schemaName));
		}

		protected abstract string FormatCreateSchemaCommandText(string catalogName, string schemaName);

		public abstract string FormatParameterName(string rawParameterName);

		public virtual void AsyncExecuteReader(DbCommand cmd, Action<DbDataReader> readerHandler)
		{
			Require.IsNotNull("cmd", cmd);
			Require.IsNotNull("resultHandler", readerHandler);

			ThreadPool.QueueUserWorkItem(new WaitCallback((unused) =>
			{
				readerHandler(cmd.ExecuteReader());
			})); 
		}				
	}
}
