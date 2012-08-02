using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ATTi.Core;
using ATTi.Core.Trace;
using ATTi.Core.Data;
using ATTi.Core.Factory;
using ATTi.TMail.Common;
using ATTi.TMail.Model;
using System.Web;

namespace ATTi.TMail.Service.Implementation
{
	partial class TMailService
	{
		static readonly string CConnectionStringName = "tmail-data";
		static readonly TimeSpan CApplicationCacheTimeout = TimeSpan.FromMinutes(5);

		readonly bool _traceExecutionSecurityContext;

		delegate T DbAction<T>(DbConnection cn, T instance, List<DbAction<T>> actions);

		MailTracking PerformMailTrackingInsert(DbConnection cn, MailTracking t, string statusNote)
		{
            YPMon.Info("PERFORM_MAIL_TRACKING_INSERT", string.Format("START: DB Connection: {0}, MailTracking: {1}, StatusNote: {2} RunAs {3}", cn.ConnectionString, t.ToString(), statusNote, Thread.CurrentPrincipal.Identity.Name));
			// Insert the mail tracking instance...
			TraceExecutionContext(_traceExecutionSecurityContext);
			var inserted = cn.ImmediateExecuteSingleWithCommandParameters<MailTracking>(@"[dbo].[MailTracking_Create] @ID, @StatusID, @Application, @Environment, @EmailTemplate, @Note",
					(binder) =>
					{
						binder.DefineAndBindParameter("@ID", DbType.Guid, ParameterDirection.Input, t.ID);
						binder.DefineAndBindParameter("@StatusID", DbType.Int32, ParameterDirection.Input, (int)t.Status);
						binder.DefineAndBindParameter("@Application", DbType.String, ParameterDirection.Input, t.Application);
						binder.DefineAndBindParameter("@Environment", DbType.String, ParameterDirection.Input, t.Environment);
						binder.DefineAndBindParameter("@EmailTemplate", DbType.String, ParameterDirection.Input, t.EmailTemplate);
						binder.DefineAndBindParameter("@Note", DbType.String, ParameterDirection.Input, statusNote);
					}, LoadMailTrackingFromDbDataReader
				);
			return inserted;
		}

		MailTracking PerformMailTrackingUpdate(DbConnection cn, MailTracking current, string statusNote)
		{
			// Get the current instance from the DB...
			YPMon.Info("PERFORM_MAIL_TRACKING", string.Format("START: DB Connection: {0}, MailTracking: {1}, StatusNote: {2}", cn.ConnectionString, current.ToString(), statusNote));
			TraceExecutionContext(_traceExecutionSecurityContext);
			var updated = cn.ImmediateExecuteSingleWithCommandParameters<MailTracking>("[dbo].[MailTracking_Update] @ID, @DateUpdated, @StatusID, @Note",
						(binder) =>
						{
							binder.DefineAndBindParameter("@ID", DbType.Guid, ParameterDirection.Input, current.ID);
							binder.DefineAndBindParameter("@DateUpdated", DbType.DateTime2, ParameterDirection.Input, current.DateUpdated);
							binder.DefineAndBindParameter("@StatusID", DbType.Int32, ParameterDirection.Input, (int)current.Status);
							binder.DefineAndBindParameter("@Note", DbType.String, ParameterDirection.Input, statusNote);
						}, LoadMailTrackingFromDbDataReader
						);
			YPMon.Info("PERFORM_MAIL_TRACKING", string.Format("DONE: DB Connection: {0}, MailTracking: {1}, StatusNote: {2}", cn.ConnectionString, current.ToString(), statusNote));

			return updated;
		}

		MailTracking PerformMailTrackingSelectByID(DbConnection cn, Guid mailingID)
		{
			YPMon.Info("PERFORMMAILINGTRACKING_SELECT_BY_ID",string.Format("DBConn: {0}, MailingId {1}",cn.ConnectionString,mailingID.ToString()));
			TraceExecutionContext(_traceExecutionSecurityContext);
			return cn.ImmediateExecuteSingleOrDefaultWithCommandParametersForMultipleResults<MailTracking>(@"[dbo].[MailTracking_SelectByID] @ID",
							CommandType.Text,
							(binder) =>
							{
								binder.DefineAndBindParameter("@ID", DbType.Guid, ParameterDirection.Input, mailingID);
							},
							LoadMailTrackingFromDbDataReader);
		}

		MailTracking LoadMailTrackingFromDbDataReader(DbDataReader reader)
		{
			// Load the Application's fields...
			var t = Factory<MailTracking>.CreateInstance();
			t.ID = reader.GetValueOrDefault<Guid>(reader.GetOrdinal("ID"));
			t.DateCreated = reader.GetValueOrDefault<DateTime>(reader.GetOrdinal("DateCreated"));
			t.DateUpdated = reader.GetValueOrDefault<DateTime>(reader.GetOrdinal("DateUpdated"));
			t.Status = (MailTrackingStatus)reader.GetValueOrDefault<int>(reader.GetOrdinal("StatusID"));
			t.Application = reader.GetValueOrDefault<string>(reader.GetOrdinal("Application"));
			t.Environment = reader.GetValueOrDefault<string>(reader.GetOrdinal("Environment"));
			t.EmailTemplate = reader.GetValueOrDefault<string>(reader.GetOrdinal("EmailTemplate"));
			// Load the Notes...
			if (reader.NextResult())
			{
				t.Notes = reader.EnumerateAndTransform<TrackingNote, List<TrackingNote>>(
					LoadTrackingNoteFromDbDataReader);
				// Fixup the note's reference to the tracking instance...
				foreach (var e in t.Notes)
				{
					e.Tracking = t;
				}
			}
			return t;
		}

		TrackingNote LoadTrackingNoteFromDbDataReader(DbDataReader reader)
		{
			var n = new TrackingNote();
			n.ID = reader.GetValueOrDefault<long>(reader.GetOrdinal("ID"));
			n.DateCreated = reader.GetValueOrDefault<DateTime>(reader.GetOrdinal("DateCreated"));
			n.Status = (MailTrackingStatus)reader.GetValueOrDefault<int>(reader.GetOrdinal("StatusID"));
			n.StatusNote = reader.GetValueOrDefault<String>(reader.GetOrdinal("StatusNote"));
			return n;
		}

		DbConnection CreateAndOpenConnection(string cnName)
		{
			YPMon.Info("CREATEANDOPENCONNECTION", String.Concat("Connection: ", cnName));
			TraceExecutionContext(_traceExecutionSecurityContext);
			DbConnection result = null; //= DbExtensions.CreateConnection(cnName);
			try
			{
				result = DbExtensions.CreateConnection(cnName);
				result.Open();
			}
			catch (Exception ex)
			{
				YPMon.Info("CREATEANDOPENCONNECTION", String.Concat("Connection: ",cnName," ConnectionString: ",result.ConnectionString, " Exception: ",ex.Message));
				if(result!=null)
				 result.Dispose();
				throw;
			}
			YPMon.Info("CREATEANDOPENCONNECTION", String.Concat("Provider Returned: ", result.ConnectionString));
			return result;
		}

		T ExecuteDbActionsWithinTransaction<T>(T instance, params DbAction<T>[] actions)
		{
			using (var scope = new CleanupScope())
			{
				IEnumerable<DbAction<T>> currentActions = actions;

				var cn = scope.Add(CreateAndOpenConnection(CConnectionStringName));
				var tx = scope.Add(TransactionScopeHelper.CreateScope_ShareCurrentOrCreate());
				try
				{
					T result = instance;

					while (currentActions.Count() > 0)
					{
						List<DbAction<T>> futureActions = new List<DbAction<T>>();
						foreach (var action in actions)
						{
							result = action(cn, result, futureActions);
						}
						currentActions = futureActions;
					}
					tx.Complete();
					return result;
				}
				catch (DbException e)
				{
					YPMon.Critical("ExecuteDbActionsWithinTransaction", "ERROR with exception", e.Message);
					throw TranslateDbException(e);
				}
			}
		}

		Exception TranslateDbException(DbException e)
		{
			if (e is SqlException)
			{
				var sqlEx = (SqlException)e;
				// Filter the generic error right here:				
				if (sqlEx.Number == (int)ObjectDbErrorCodes.Generic)
				{
					var s = sqlEx.Message.Split('|');
					int errorCode;
					if (s.Length > 2 && Int32.TryParse(s[1], out errorCode))
					{
						return DoTranslateFromSqlServerErrorCode(sqlEx, errorCode);
					}
				}
				else return DoTranslateFromSqlServerErrorCode(sqlEx, sqlEx.Number);
			}

			return e;
		}

		Exception DoTranslateFromSqlServerErrorCode(SqlException sqlEx, int errorCode)
		{
			switch ((ObjectDbErrorCodes)errorCode)
			{
				case ObjectDbErrorCodes.ObjectAlreadyExists:
					return new ResourceAlreadyExistsException(sqlEx.Message, sqlEx);
				case ObjectDbErrorCodes.ObjectNotFound:
					return new ResourceNotFoundException(sqlEx.Message, sqlEx);
				case ObjectDbErrorCodes.ObjectDataInconsistent:
					return new ResourceDataInconsistentException(sqlEx.Message, sqlEx);
				case ObjectDbErrorCodes.ObjectNotAllowed:
					return new ResourceNotAllowedException(sqlEx.Message, sqlEx);
				default:
					return sqlEx;
			}
		}

		//ToDO: this is such a secuity hazard for debugging
		void TraceExecutionContext(bool debugExecutionContext) 
		{
			if (debugExecutionContext)
			{
				string executionIdentity = string.Empty;
				executionIdentity = string.Format("Identity Name: {0} IsAuthenticated: {1} AuthenticationType: {2}",
						Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.IsAuthenticated,
						Thread.CurrentPrincipal.Identity.AuthenticationType);
				Traceable.TraceData(typeof(TMailService), TraceEventType.Verbose, executionIdentity);
			}

		}

	}
}
