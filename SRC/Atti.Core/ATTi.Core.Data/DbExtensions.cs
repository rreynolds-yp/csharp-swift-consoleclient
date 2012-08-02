using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using ATTi.Core.Contracts;

namespace ATTi.Core.Data
{
	public static class DbExtensions
	{
		struct ProviderRecord
		{
			public string ConnectionName;
			public string ConnectionString;
			public DbProviderFactory Provider;			
		}

		static Object __providerLock = new Object();
		static Dictionary<string, ProviderRecord> __providers = new Dictionary<string, ProviderRecord>();

		/// <summary>
		/// Clears cached providers.
		/// </summary>
		public static void ResetProviders()
		{
			lock (__providerLock)
			{
				__providers = new Dictionary<string, ProviderRecord>();
			}
		}

		/// <summary>
		/// Gets a DbProviderFactory identified by the specified connection name. The connection name 
		/// must be present in the &lt;connectionStrings&gt; section of the application's config file
		/// </summary>
		/// <param name="cnName">A connection string name declared in the application's config file.</param>
		/// <exception cref="System.ArgumentNullException">thrown if the connection name is not specified.</exception>
		/// <exception cref="System.ArgumentException">thrown if the connection name is not found in the application's config file.</exception>
		/// <returns>The DbProviderFactory associated with the specified connection string.</returns>
		public static DbProviderFactory GetProviderByConnectionName(string cnName)
		{
			Require.IsNotNull("cnName", cnName);

			return AccessProvider(cnName).Provider;
		}

		private static ProviderRecord AccessProvider(string cnName)
		{
			Require.IsNotNull("cnName", cnName);

			ProviderRecord r;
			lock (__providerLock)
			{
				if (!__providers.TryGetValue(cnName, out r))
				{
					ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[cnName];
					if (css == null) throw new ArgumentException("cnName", String.Format("ConnectionString not configured: {0}", cnName));
					r.ConnectionName = cnName;
					r.ConnectionString = css.ConnectionString;
					r.Provider = DbProviderFactories.GetFactory(css.ProviderName);					
				}
			}
			return r;
		}

		/// <summary>
		/// Creates a new DbConnection initialized with the specified connection name. The connection name 
		/// must be present in the &lt;connectionStrings&gt; section of the application's config file
		/// </summary>
		/// <param name="cnName">A connection string name declared in the application's config file.</param>
		/// <exception cref="System.ArgumentNullException">thrown if the connection name is not specified.</exception>
		/// <exception cref="System.ArgumentException">thrown if the connection name is not found in the application's config file.</exception>
		/// <returns>The DbConnection initialized with the specified connection string.</returns>
		public static DbConnection CreateConnection(string cnName)
		{
			Require.IsNotNull("cnName", cnName);

			ProviderRecord r = AccessProvider(cnName);
			DbConnection cn = r.Provider.CreateConnection();
			cn.ConnectionString = r.ConnectionString;
			return cn;
		}
	}
}
