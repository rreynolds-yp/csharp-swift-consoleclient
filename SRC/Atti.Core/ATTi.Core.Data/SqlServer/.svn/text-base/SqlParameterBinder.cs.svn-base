using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ATTi.Core.Data.SqlServer
{
	public class SqlParameterBinder : DbParameterBinder
	{
		private static DbTypeTranslation[] SqlDbTypeMap = new DbTypeTranslation[]
			{ new DbTypeTranslation(DbType.AnsiString, (int)SqlDbType.VarChar, typeof(string), "VARCHAR", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.Binary, (int)SqlDbType.Binary, typeof(byte[]), "BINARY", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.Byte, (int)SqlDbType.TinyInt, typeof(byte), "TINYINT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Boolean, (int)SqlDbType.Bit, typeof(bool), "BIT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Currency, (int)SqlDbType.Money, typeof(decimal), "MONEY", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.Date, (int)SqlDbType.Date, typeof(DateTime), "DATE", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.DateTime, (int)SqlDbType.DateTime, typeof(DateTime), "DATETIME", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Decimal, (int)SqlDbType.Decimal, typeof(decimal), "DECIMAL", DbTypeLengthSpecifiers.Precision | DbTypeLengthSpecifiers.Scale | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.Double, (int)SqlDbType.Float, typeof(double), "FLOAT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Guid, (int)SqlDbType.UniqueIdentifier, typeof(Guid), "UNIQUEIDENTIFIER", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Int16, (int)SqlDbType.SmallInt, typeof(short), "SMALLINT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Int32, (int)SqlDbType.Int, typeof(int), "INT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Int64, (int)SqlDbType.BigInt, typeof(long), "BIGINT", DbTypeLengthSpecifiers.None)
			, default(DbTypeTranslation) 
			, new DbTypeTranslation(DbType.SByte, (int)SqlDbType.TinyInt, typeof(byte), "TINYINT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.Single, (int)SqlDbType.Float, typeof(float), "FLOAT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.String, (int)SqlDbType.NVarChar, typeof(string), "NVARCHAR", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.Time, (int)SqlDbType.Time, typeof(DateTime), "TIME", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.UInt16, (int)SqlDbType.Int, typeof(ushort), "INT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.UInt32, (int)SqlDbType.BigInt, typeof(uint), "BIGINT", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.UInt64, (int)SqlDbType.BigInt, typeof(ulong), "BIGINT", DbTypeLengthSpecifiers.ApproximationMapping)
			, new DbTypeTranslation(DbType.VarNumeric, (int)SqlDbType.Decimal, typeof(string), "NUMERIC", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.AnsiStringFixedLength, (int)SqlDbType.Char, typeof(string), "CHAR", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.StringFixedLength, (int)SqlDbType.NChar, typeof(string), "NCHAR", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, default(DbTypeTranslation)			
			, new DbTypeTranslation(DbType.Xml, (int)SqlDbType.Xml, typeof(string), "XML", DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.IndicatedByParenthesis)
			, new DbTypeTranslation(DbType.DateTime2, (int)SqlDbType.DateTime2, typeof(DateTime), "DATETIME2", DbTypeLengthSpecifiers.None)
			, new DbTypeTranslation(DbType.DateTimeOffset, (int)SqlDbType.DateTimeOffset, typeof(DateTime), "DATETIMEOFFSET", DbTypeLengthSpecifiers.None)
			};

		static DbCommand EnsureDbCommandIsSqlCommand(DbCommand cmd)
		{
			if (!typeof(SqlCommand).IsInstanceOfType(cmd)) throw new ArgumentException("DbCommand type missmatch: must be a SqlCommand", "cmd");
			return cmd;
		}

		public SqlParameterBinder(DbCommand cmd)
			: base(EnsureDbCommandIsSqlCommand(cmd))
		{
		}

		public SqlCommand CommandAsSqlCommand { get { return (SqlCommand)base.DbCommand; } }

		public override DbParameter DefineParameter(string parameterName, DbType dbType, ParameterDirection direction, int length)
		{
			SqlCommand sqlCmd = (SqlCommand)base.DbCommand;
			SqlParameter sqlParm = sqlCmd.Parameters.Add(parameterName, (SqlDbType)SqlDbTypeMap[(int)dbType].ProviderDbType, length);
			sqlParm.Direction = direction;
			return sqlParm;
		}

		public override DbParameter DefineParameter(string parameterName, DbType dbType, ParameterDirection direction)
		{
			SqlCommand sqlCmd = (SqlCommand)base.DbCommand;
			SqlParameter sqlParm = sqlCmd.Parameters.Add(parameterName, (SqlDbType)SqlDbTypeMap[(int)dbType].ProviderDbType);
			sqlParm.Direction = direction;
			return sqlParm;
		}

		public override DbParameter GetDbParameterByName(string parameterName)
		{
			return CommandAsSqlCommand.Parameters[parameterName];
		}

		public override DbTypeTranslation TranslateDbType(DbType dbType)
		{
			return SqlDbTypeMap[(int)dbType];
		}
	}

}
