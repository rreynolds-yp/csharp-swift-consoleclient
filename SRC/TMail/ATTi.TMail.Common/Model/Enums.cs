using System;

namespace ATTi.TMail.Model
{
	/// <summary>
	/// WARNING! The text values and numeric values of this enum correspond
	/// with the rows of the database table [tmail].[dbo].[MailTrackingStatus]
	/// 
	/// Any changes here require database script changes in order to keep
	/// the database in synch.
	/// </summary>
	public enum MailTrackingStatus
	{
		None = 0,
		/// <summary>
		/// Indicates the mailing has been accepted.
		/// </summary>
		Accepted = 1,
		/// <summary>
		/// Indicates the mailing has been submitted to StrongMail.
		/// </summary>
		Submitted = 2,
		/// <summary>
		/// Indicates the mailing has completed.
		/// </summary>
		Completed = 3,
		/// <summary>
		/// Indicates the mailing has been canceled.
		/// </summary>
		Canceled = 4,
        Paused = 5,

		Failed = 1001,
        Dropped = 1002
	}
}
