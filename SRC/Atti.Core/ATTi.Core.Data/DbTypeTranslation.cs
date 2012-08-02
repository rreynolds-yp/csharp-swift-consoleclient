using System;
using System.Data;
using System.Text;

namespace ATTi.Core.Data
{
	[Flags]
	public enum DbTypeLengthSpecifiers
	{
		None = 0,
		Length = 1,
		Precision = 2,
		Scale = 4,
		IndicatedByBrackets = 8,
		IndicatedByParenthesis = 16,
		ApproximationMapping = 0x08000000
	}

	public struct DbTypeTranslation
	{
		private const DbTypeLengthSpecifiers DbTypeLengthSpecifiersMask = DbTypeLengthSpecifiers.Length | DbTypeLengthSpecifiers.Precision | DbTypeLengthSpecifiers.Scale;
		private DbType _dbType;
		private int _providerDbType;
		private Type _clrType;
		private string _dbTypeName;
		private DbTypeLengthSpecifiers _lengthSpec;		
	
		public DbTypeTranslation(DbType dbType, int providerDbType, Type clrType
			, string dbTypeName, DbTypeLengthSpecifiers lenthSpec)
		{
			_dbType = dbType;
			_providerDbType = providerDbType;
			_clrType = clrType;
			_dbTypeName = dbTypeName;
			_lengthSpec = lenthSpec;
		}

		public DbType DbType { get { return _dbType; } }
		public int ProviderDbType { get { return _providerDbType; } }
		public Type ClrType { get { return _clrType; } }
		public string ProviderTypeName { get { return _dbTypeName; } }
		public DbTypeLengthSpecifiers Requirements { get { return _lengthSpec; } }

		private Exception RequirementsNotMetException()
		{
			if ((_lengthSpec & DbTypeLengthSpecifiers.Length) == DbTypeLengthSpecifiers.Length)
				throw new InvalidOperationException(String.Format("A length must be specified when using this DbType: {0}", _dbTypeName));
			if ((_lengthSpec & DbTypeLengthSpecifiers.Precision) == DbTypeLengthSpecifiers.Precision
				&& (_lengthSpec & DbTypeLengthSpecifiers.Scale) == DbTypeLengthSpecifiers.Scale)
				throw new InvalidOperationException(String.Format("A length must be specified when using this DbType: {0}", _dbTypeName));
			else
				throw new InvalidOperationException(String.Format("Unrecognized requirements for this DbType: {0}", _dbTypeName));
		}

		public void AppendProviderDbTypeString(StringBuilder buffer)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if ((_lengthSpec & DbTypeLengthSpecifiersMask) != DbTypeLengthSpecifiers.None) throw RequirementsNotMetException();
			buffer.Append(_dbTypeName);
		}

		public void AppendProviderDbTypeString(StringBuilder buffer, int length)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if ((_lengthSpec & DbTypeLengthSpecifiers.Length) != DbTypeLengthSpecifiers.Length) throw RequirementsNotMetException();
			buffer.Append(_dbTypeName);
			// Use parenthesis as the default since they are specified in the SQL standard
			if ((_lengthSpec & DbTypeLengthSpecifiers.IndicatedByBrackets) == DbTypeLengthSpecifiers.IndicatedByBrackets)
			{
				buffer.Append('[').Append(length).Append(']');
			}
			else
			{
				buffer.Append('(').Append(length).Append(')');
			}
		}
		public void AppendProviderDbTypeString(StringBuilder buffer, int precision, int scale)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if ((_lengthSpec & DbTypeLengthSpecifiers.Precision) == DbTypeLengthSpecifiers.Precision
				&& (_lengthSpec & DbTypeLengthSpecifiers.Scale) == DbTypeLengthSpecifiers.Scale) throw RequirementsNotMetException();
			buffer.Append(_dbTypeName);
			// Use parenthesis as the default since they are specified in the SQL standard
			if ((_lengthSpec & DbTypeLengthSpecifiers.IndicatedByBrackets) == DbTypeLengthSpecifiers.IndicatedByBrackets)
			{
				buffer.Append('[').Append(precision).Append(',').Append(scale).Append(']');
			}
			else
			{
				buffer.Append('(').Append(precision).Append(',').Append(scale).Append(')');
			}
		}
	}
}
