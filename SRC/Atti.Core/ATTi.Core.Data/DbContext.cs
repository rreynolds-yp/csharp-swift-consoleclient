using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using ATTi.Core.Trace;

namespace ATTi.Core.Data
{
	public enum DbContextOptions
	{
		/// <summary>
		/// Indicates the context should be readonly.
		/// </summary>
		ReadOnly = 0,
		/// <summary>
		/// Indicates the context must be readonly and if an attempt to upgrade to read-write is made
		/// an exception will be thrown.
		/// </summary>
		ReadOnlyEnforced = 1,
		/// <summary>
		/// Indicates the context should allow reads and writes.
		/// </summary>
		ReadAndWrite = 2
	}

	/// <summary>
	/// Database context.
	/// </summary>
	public interface IDbContext : IDisposable
	{
		bool IsReadOnly { get; }

		DbContextCache<TKey> ContextCache<TKey>();
		/// <summary>
		/// Gets a connection by connection name.
		/// </summary>
		/// <param name="cnName">The name of the connection. This name corresponds to the name in the ConnectionStrings configuration element.</param>
		/// <returns>A DbConnection.</returns>
		DbConnection CreateConnection(string cnName);
		/// <summary>
		/// Gets a shread connection by name and catalog name.
		/// </summary>
		/// <param name="cnName">The name of the connection. This name corresponds to the name in the ConnectionStrings configuration element.</param>
		/// <param name="catName">The catalog name. This parameter is used to ensure the connection is attached to the appropriate database.</param>
		/// <returns>An opened SharedConnection.</returns>
		SharedConnection OpenSharedConnection(string cnName, string catName);

		/// <summary>
		/// If the context includes an ambient transaction then the transaction
		/// is marked complete. Otherwise the call is benign.
		/// </summary>
		void Complete();
	}

	public static class DbContext
	{
		[ThreadStatic]
		private static IDbContext __current;

		public static IDbContext Current { get { return __current; } }

		/// <summary>
		/// Starts a readonly context. If you are not making database 
		/// modifications then use this context type. ReadOnly contexts 
		/// will use the ambient transaction and will not (on its own) 
		/// promote the transaction to a full distributed transaction.
		/// </summary>
		/// <returns></returns>
		public static IDbContext JoinOrStartReadonlyContext()
		{
			if (__current != null)
			{
				return new BenignSharedContext(true);
			}
			else
			{
				return new InnerContext(false, false);
			}
		}

		public static IDbContext JoinOrStartReadWriteContext()
		{
			if (__current != null)
			{
#if DEBUG
				if (__current.IsReadOnly) Traceable.TraceData(typeof(DbContext), TraceEventType.Error, "Outer DbContext is readonly");
#endif
				return new BenignSharedContext(false);
			}
			else
			{
				return new InnerContext(true, false);
			}
		}

		public static IDbContext JoinOrStartReadWriteContext(TimeSpan timeout)
		{
			if (__current != null)
			{
#if DEBUG
				if (__current.IsReadOnly) Traceable.TraceData(typeof(DbContext), TraceEventType.Error, "Outer DbContext is readonly");
#endif
				return new BenignSharedContext(false, timeout);
			}
			else
			{
				return new InnerContext(true, false, timeout);
			}
		}

		public static IDbContext JoinOrStartReadWriteContextWithTransaction()
		{
			if (__current != null)
			{
#if DEBUG
				if (__current.IsReadOnly) Traceable.TraceData(typeof(DbContext), TraceEventType.Error, "Outer DbContext is readonly");
#endif
				return new BenignSharedContext(false, true);
			}
			else
			{
				return new InnerContext(true, true);
			}
		}

		public static IDbContext JoinOrStartReadWriteContextWithTransaction(TimeSpan timeout)
		{
			if (__current != null)
			{
#if DEBUG
				if (__current.IsReadOnly) Traceable.TraceData(typeof(DbContext), TraceEventType.Error, "Outer DbContext is readonly");
#endif
				return new BenignSharedContext(false, true, timeout);
			}
			else
			{
				return new InnerContext(true, true, timeout);
			}
		}

		internal class InnerContext : IDbContext, ITraceable
		{
			private class TransactionSubcontext
			{
				Transaction _tx;
				Action<Transaction> _completionCallback;
				bool _txCompleted = false;
				Object _connectionsLock = new Object();
				Dictionary<DbConnection, ConnectionTracking> _connections;

				internal TransactionSubcontext(Transaction tx, Action<Transaction> completionCallback)
				{
					if (tx != null)
					{
						_tx = tx;
						_completionCallback = completionCallback;
						_tx.TransactionCompleted += new TransactionCompletedEventHandler(Transaction_TransactionCompleted);
					}
				}

				void Transaction_TransactionCompleted(object sender, TransactionEventArgs e)
				{
					if (e.Transaction == _tx)
					{
						lock (_connectionsLock)
						{
							if (_connections != null)
								_connections.Clear();
							_txCompleted = true;
							if (_completionCallback != null) _completionCallback(_tx);
						}
					}
				}

				internal DbConnection CreateAndTrackConnection(string cnName)
				{
					if (_txCompleted == true) throw new InvalidOperationException("Cannot create connections in the context of a transaction that has completed");

					ConnectionTracking tr = new ConnectionTracking();
					tr.ConnectionName = cnName;
					tr.RawDbConnection = DbExtensions.CreateConnection(cnName);
					BeginTrackingConnection(tr);
					return tr.RawDbConnection;
				}
				private void BeginTrackingConnection(ConnectionTracking tr)
				{
					lock (_connectionsLock)
					{
						if (_connections == null) _connections = new Dictionary<DbConnection, ConnectionTracking>();
						tr.RawDbConnection.StateChange += new StateChangeEventHandler(RawDbConnection_StateChange);
						_connections.Add(tr.RawDbConnection, tr);
					}
				}
				void RawDbConnection_StateChange(object sender, StateChangeEventArgs e)
				{
					DbConnection cn = sender as DbConnection;
					if (cn != null
						&& (e.CurrentState == ConnectionState.Closed
						|| e.CurrentState == ConnectionState.Broken))
					{
						// Remove the connection so it doesn't get handed back out (shared) by the context.
						lock (_connectionsLock)
						{
							_connections.Remove(cn);
						}
					}
				}


				internal SharedConnection OpenAndTrackSharedConnection(string cnName, string catName)
				{
					if (_txCompleted == true) throw new InvalidOperationException("Cannot create connections in the context of a transaction that has completed");

					lock (_connectionsLock)
					{
						ConnectionTracking tr;
						if (_connections == null) _connections = new Dictionary<DbConnection, ConnectionTracking>();

						// connections can be shared if:
						// 1) they are connected to the same resource
						// 2) they support sharing, which in MS SQL Server case means MARS support
						if (_connections != null)
						{
							tr = (from t in _connections.Values
										where t.ConnectionName == cnName
										 && t.CatalogName == catName
										 && t.SupportsSharing
										select t).FirstOrDefault();
							if (tr != null) return tr.SharedConnection.Clone();
						}

						tr = new ConnectionTracking();
						tr.ConnectionName = cnName;
						tr.CatalogName = catName;
						tr.RawDbConnection = CreateAndOpenConnection(cnName, catName);
						tr.SupportsSharing = DbProviderHelpers.GetDbProviderHelperForDbConnection(tr.RawDbConnection)
							.SupportsMultipleActiveResultSets(tr.RawDbConnection);
						tr.SharedConnection = new SharedConnection(tr.RawDbConnection);
						tr.RawDbConnection.StateChange += new StateChangeEventHandler(RawDbConnection_StateChange);
						_connections.Add(tr.RawDbConnection, tr);
						return tr.SharedConnection;
					}
				}

				private DbConnection CreateAndOpenConnection(string cnName, string catName)
				{
					DbConnection result = DbExtensions.CreateConnection(cnName);
					result.Open();
					if (!String.Equals(catName, result.Database, StringComparison.InvariantCultureIgnoreCase))
					{
						result.ChangeDatabase(catName);
					}
					return result;
				}

			}

			private class ConnectionTracking
			{
				public string ConnectionName;
				public String CatalogName;
				public DbConnection RawDbConnection;
				public bool SupportsSharing;
				public SharedConnection SharedConnection;
			}

			List<ConnectionTracking> _trackingFew = new List<ConnectionTracking>();
			TransactionSubcontext _nullSubcontext = new TransactionSubcontext(null, null);
			Dictionary<Transaction, TransactionSubcontext> _subcontexts;
			Dictionary<Type, object> _caches;
			bool _isReadOnly;
			TransactionScope _tx;

			internal InnerContext(bool allowWritesInThisContext, bool joinOrCreateTransaction)
			{
				_isReadOnly = (!allowWritesInThisContext);
				if (joinOrCreateTransaction)
				{
					_tx = TransactionScopeHelper.CreateScope_ShareCurrentOrCreate();
				}

				__current = this;
			}

			internal InnerContext(bool allowWritesInThisContext, bool joinOrCreateTransaction, TimeSpan timeout)
			{
				_isReadOnly = (!allowWritesInThisContext);
				if (joinOrCreateTransaction)
				{
					_tx = TransactionScopeHelper.CreateScope_ShareCurrentOrCreate(timeout);
				}

				__current = this;
			}

			#region IDbContext Members

			public bool IsReadOnly { get { return _isReadOnly; } }

			public DbContextCache<TKey> ContextCache<TKey>()
			{
				if (_caches == null) _caches = new Dictionary<Type, object>();
				object cache;
				if (!_caches.TryGetValue(typeof(TKey), out cache))
				{
					cache = new DefaultContextCache<TKey>();
					_caches.Add(typeof(TKey), cache);
				}
				return (DbContextCache<TKey>)cache;
			}

			public DbConnection CreateConnection(string cnName)
			{
				Transaction tx = Transaction.Current;
				TransactionSubcontext sub;

				if (TransactionSubcontextExists(tx))
				{
					sub = GetTransactionSubcontext(tx);
				}
				else
				{
					sub = BeginTransactionSubcontext(tx);
				}

				return sub.CreateAndTrackConnection(cnName);
			}

			private TransactionSubcontext BeginTransactionSubcontext(Transaction tx)
			{
				if (_subcontexts == null) _subcontexts = new Dictionary<Transaction, TransactionSubcontext>();
				TransactionSubcontext tracking = new TransactionSubcontext(tx, (Transaction t) =>
				{
					_subcontexts.Remove(t);
				});
				_subcontexts.Add(tx, tracking);
				return tracking;
			}

			private TransactionSubcontext GetTransactionSubcontext(Transaction tx)
			{
				return (tx == null) ? _nullSubcontext : (_subcontexts != null) ? _subcontexts[tx] : null;
			}

			private bool TransactionSubcontextExists(Transaction tx)
			{
				return (tx == null || (_subcontexts != null && _subcontexts.ContainsKey(tx)));
			}

			public SharedConnection OpenSharedConnection(string cnName, string catName)
			{
				Transaction tx = Transaction.Current;
				TransactionSubcontext sub;

				if (TransactionSubcontextExists(tx))
				{
					sub = GetTransactionSubcontext(tx);
				}
				else
				{
					sub = BeginTransactionSubcontext(tx);
				}

				return sub.OpenAndTrackSharedConnection(cnName, catName);
			}

			public void Complete()
			{
				if (_tx != null) _tx.Complete();
			}

			#endregion

			#region IDisposable Members

			~InnerContext()
			{
				try
				{
					this.TraceData(TraceEventType.Error, "DbContext finalize reached; DbConnections are living longer than necessary and may degrade performance");
				}
				catch { /* eat any errors thrown by the tracing */ }
				Dispose(false);
			}

			private void Dispose(bool disposing)
			{
				try
				{
					foreach (ConnectionTracking tr in _trackingFew)
					{
						DbConnection cn = tr.RawDbConnection;
						if ((cn.State & ConnectionState.Closed) != ConnectionState.Closed)
						{
							cn.Close();
						}
						cn.Dispose();
					}
					if (_tx != null) _tx.Dispose();
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, "Error while cleaning up shared DbConnections in DbContext", e);
				}
				finally
				{
					if (__current != this)
						this.TraceData(TraceEventType.Error, "Error while cleaning up shared DbConnections in DbContext... __current is not this?");
					__current = null;
				}
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			#endregion

		}
		internal class BenignSharedContext : IDbContext
		{
			TransactionScope _tx;
			public BenignSharedContext(bool isReadOnly)
				: this(isReadOnly, false)
			{
			}

			public BenignSharedContext(bool isReadOnly, TimeSpan timeout)
				: this(isReadOnly, false, timeout)
			{
			}

			public BenignSharedContext(bool isReadOnly, bool joinOrStartTransaction)
			{
				IsReadOnly = isReadOnly;
				ID = Guid.NewGuid();
				if (joinOrStartTransaction)
				{
					_tx = TransactionScopeHelper.CreateScope_ShareCurrentOrCreate();
				}
			}

			public BenignSharedContext(bool isReadOnly, bool joinOrStartTransaction, TimeSpan timeout)
			{
				IsReadOnly = isReadOnly;
				ID = Guid.NewGuid();
				if (joinOrStartTransaction)
				{
					_tx = TransactionScopeHelper.CreateScope_ShareCurrentOrCreate(timeout);
				}
			}

			public Guid ID { get; private set; }

			public bool IsReadOnly { get; private set; }

			public DbContextCache<TKey> ContextCache<TKey>()
			{
				return __current.ContextCache<TKey>();
			}
			public DbConnection CreateConnection(string cnName)
			{
				return __current.CreateConnection(cnName);
			}
			public SharedConnection OpenSharedConnection(string cnName, string catName)
			{
				return __current.OpenSharedConnection(cnName, catName);
			}
			public void Dispose()
			{
				if (_tx != null) _tx.Dispose();
			}
			public void Complete()
			{
				if (_tx != null) _tx.Complete();
			}
		}
	}
}
