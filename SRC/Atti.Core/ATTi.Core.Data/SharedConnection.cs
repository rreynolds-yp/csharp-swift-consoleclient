using System;
using System.Data.Common;
using System.Threading;

namespace ATTi.Core.Data
{
	public class SharedConnection : IDisposable, ICloneable
	{
		#region Declarations
		private readonly SharedConnection _originator;
		private DbConnection _cn;
		private int _count = 0;
		#endregion

		internal SharedConnection(DbConnection cn)
		{
			if (cn == null) throw new ArgumentNullException("cn");
			_originator = this;
			_cn = cn;
			_count = 1;
		}

		public string CatalogName
		{
			get { return _cn.Database; }
		}

		public DbConnection Connection
		{
			get { return _cn; }
		}

		#region IDisposable Members
		~SharedConnection()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (Interlocked.Decrement(ref _originator._count) == 0)
			{
				if (_cn.State != System.Data.ConnectionState.Closed)
					_cn.Close();
				_cn.Dispose();
				_cn = null;
			}
		}
		#endregion

		#region ICloneable Members
		public SharedConnection Clone()
		{
			SharedConnection clone = (SharedConnection)base.MemberwiseClone();
			Interlocked.Increment(ref _originator._count);
			return clone;
		}
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		#endregion
	}	
}
