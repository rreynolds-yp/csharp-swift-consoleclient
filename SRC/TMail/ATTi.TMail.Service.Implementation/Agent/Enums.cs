
using System;
namespace ATTi.TMail.Service.Implementation.Agent
{
	public enum AgentMessageKind
	{
		None = 0,
		Mailing = 1,
		MailingDelayed = 2,
		MailingPaused =3,
		
		Ping = 1000,
		Pong = 1001,
		Join = 3000,
		Leave = 3001,
		LeadershipAnnounce = 5000,
	}

	[Flags]
	public enum PartialCreateStatus
	{
        Dropped =0,
		None = 1,
		/// <summary>
		/// Indicates the mailing has been saved.
		/// </summary>
		Saved = 2,
		/// <summary>
		/// Indicates the mailing has been validated.
		/// </summary>
		Validated = 3,
		/// <summary>
		/// Indicates the email addresses have been recorded.
		/// </summary>
		EmailsRecorded = 4,
		/// <summary>
		/// Indicates the mailing has been delayed; it is not active or strongmail is down.
		/// </summary>
		Paused = 5,
		/// <summary>
		/// Indicates the mailing has been submitted to StrongMail.
		/// </summary>
		Delayed = 6,
		/// <summary>
		/// Indicates the mailing has been submitted to StrongMail.
		/// </summary>
		Submitted = 7,
		/// <summary>
		/// Indicates the mailing serial number issued by StrongMail has been recorded.
		/// </summary>
		Recorded = 8,
		/// <summary>
		/// Indicates the mailing has failed.
		/// </summary>
		Failed = 9
	}

	public enum LeadershipRole
	{
		Observer = 0,
		Follower,
		Leader
	}

	public enum MonitorState
	{
		None = 0,
		Starting = 1,
		Started = 2,
		Waiting = 3,
		Monitoring = 4,
		Stopping = 6,
		Stopped = 7,
		Disposed = 8,
	}
}
