using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ATTi.Core.Data
{
	public static class DbProviderExtensions
	{

	}

	public interface IDbProviderExtensions
	{
		DbTypeTranslation GetDbTypeMapping(DbType dbType);
	}
}
