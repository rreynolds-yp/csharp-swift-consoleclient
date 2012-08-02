using System;
using System.Data.Common;
using System.Data.SqlClient;
using ATTi.Core.Contracts;
using ATTi.Core.Parallel;

namespace ATTi.Core.Data.SqlServer
{
	public class SqlDbProviderHelper : DbProviderHelper
	{
		public SqlDbProviderHelper()
		{
			base.Factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
		}

		public override bool SupportsAsync
		{
			get { return true; }
		}

		public override bool SupportsMultipleActiveResultSets(DbConnection cn)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			if (!typeof(SqlConnection).IsInstanceOfType(cn))
				throw new ArgumentException("Invalid DbConnection for DbProvider", "cn");

			SqlConnection scn = (SqlConnection)cn;
			SqlConnectionStringBuilder cnsb = new SqlConnectionStringBuilder(scn.ConnectionString);
			return cnsb.MultipleActiveResultSets;
		}

		public override string GetServerName(DbConnection cn)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			return cn.ImmediateExecuteSingle<string>("SELECT @@SERVERNAME", (DbDataReader r) => { return r.GetValueOrDefault(0, "Not Available"); });
		}

		protected override string FormatCreateSchemaCommandText(string catalogName, string schemaName)
		{
			// SQL Server doesn't allow you to specify the catalog, it assumes current catalog.
			return String.Format("CREATE SCHEMA [{0}]", schemaName);
		}

		public override string FormatParameterName(string parameterName)
		{
			if (parameterName == null) throw new ArgumentNullException("parameterName");
			if (parameterName.Length == 0) throw new ArgumentException("parameterName");

			return (parameterName[0] == '@') ? parameterName : String.Concat("@", parameterName);
		}

		public override void AsyncExecuteReader(DbCommand cmd, Action<DbDataReader> resultHandler)
		{
			Require.IsInstanceOfType<SqlCommand>("cmd", cmd);
			Require.IsNotNull("resultHandler", resultHandler);

			SqlCommand scmd = (SqlCommand)cmd;
			IAsyncResult ar = scmd.BeginExecuteReader();
			new ParallelActionWrapper<ThreadPoolExecutionStrategy>( () => { resultHandler(scmd.EndExecuteReader(ar)); }
				).ScheduledExecute(ar.AsyncWaitHandle, 0);
		}
	}

}
