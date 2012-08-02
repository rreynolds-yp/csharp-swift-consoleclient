using System;
using System.Configuration;
using System.Data.Common;

namespace ATTi.Core.Data
{
	public static class DbProviderFactoryExtensions
	{
		public static DbConnection CreateConnection(this DbProviderFactory factory
			, string connectionStringName)
		{
			// Ensure we can identify the connection string...
			if (connectionStringName == null) throw new ArgumentNullException("connectionStringName");
			string cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			if (cs == null)
				throw new ArgumentException(String.Format("Connection string not defined: {0}", connectionStringName));

			DbConnection cn = factory.CreateConnection();
			cn.ConnectionString = cs;
			return cn;
		}
	}
}
