using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ATTi.Core;
using ATTi.TMail.Common;

namespace ATTi.TMail.Service.Implementation.Agent
{
	internal struct ResponseKey
	{
		public Guid ID;
		public int Sequence;

		internal ResponseKey(Guid id, int sequence)
		{
			ID = id;
			Sequence = sequence;
		}
		public bool Equals(ResponseKey other)
		{
			return ID == other.ID
				&& Sequence == other.Sequence;
		}
		public override bool Equals(object obj)
		{
			return typeof(ResponseKey).IsInstanceOfType(obj)
				&& Equals((ResponseKey)obj);
		}
		public override int GetHashCode()
		{
			int prime = 777888047;
			int hash = ID.GetHashCode() ^ prime;
			return hash * Sequence ^ prime;
		}
		public override string ToString()
		{
			return String.Concat(ID, ":", Sequence);
		}
		public static bool operator ==(ResponseKey lhs, ResponseKey rhs)
		{
			return lhs.ID == rhs.ID
				&& lhs.Sequence == rhs.Sequence;		
		}
		public static bool operator !=(ResponseKey lhs, ResponseKey rhs)
		{
			return lhs.ID != rhs.ID
				|| lhs.Sequence != rhs.Sequence;
		}

	}
	public class ResponseTracking
	{
		internal static readonly TimeSpan CExcessiveWaitTimeout = TimeSpan.FromSeconds(30);

		int _ack = 0;
		DateTime _timeout;
		MessageBase _message;
		List<MessageBase> _responses;
		Func<ResponseTracking, bool> _ready;
		Action<ResponseTracking, bool, bool> _completion;
		int _completed = 0;

		internal ResponseTracking(MessageBase message)
			: this(message, CExcessiveWaitTimeout)
		{
		}		
		internal ResponseTracking(MessageBase message, TimeSpan timeout)
		{
			_timeout = DateTime.Now.Add(timeout);
			_message = message;
			this.Key = new ResponseKey(message.AgentID, message.Sequence);
		}
		internal ResponseKey Key { get; private set; }
		internal int AcknowledgementCount { get { return Thread.VolatileRead(ref _ack); } }
		internal bool Completed { get { return Thread.VolatileRead(ref _completed) != 0; } }

		internal IEnumerable<MessageBase> Responses
		{
			get
			{
				Thread.MemoryBarrier();
				var responses = _responses;

				if (responses == null) return new MessageBase[0];
				else
				{
					lock (responses)
					{
						return responses.ToArray();
					}
				}
			}
		}
		internal int Ack(MessageBase response)
		{
			var ack = Interlocked.Increment(ref _ack);
			var responses = Util.NonBlockingLazyInitializeVolatile(ref _responses);
			lock (responses)
			{
				responses.Add(response);
			}
			RunCompletionLogic();
			return ack;
		}

		internal void SetCompletionLogic(Func<ResponseTracking, bool> ready,
			Action<ResponseTracking, bool, bool> completion)
		{
			_ready = ready;
			_completion = completion;
		}
		internal void RunCompletionLogic()
		{
			if (!Completed)
			{
				var returned = Thread.VolatileRead(ref _completed) < 0;
				var timedOut = _timeout < DateTime.Now;
				if (returned || (_ready != null && _ready(this)) || timedOut)
				{
					_completion(this, timedOut, returned);
					Thread.VolatileWrite(ref _completed, 1);
				}				
			}
		}
		internal void Returned()
		{
			Thread.VolatileWrite(ref _completed, -1);
			RunCompletionLogic();
		}
	}

	internal class ResponseTrackingLogic : IDisposable
	{
		internal static readonly TimeSpan CResponseTimeout = TimeSpan.FromMilliseconds(500);

		Status<MonitorState> _state;
		Thread _th;
		ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		Dictionary<ResponseKey, ResponseTracking> _items = new Dictionary<ResponseKey, ResponseTracking>();

		internal ResponseTrackingLogic()
		{
		}

		internal LineWriterDelegate OnWriteLine { get; set; }
		
		internal void Start()
		{
			if (_state.TryTransition(MonitorState.Starting, MonitorState.None))
			{
				_th = new Thread(RunResponseTrackingLogic);
				_th.IsBackground = true;
				_th.Start();
			}
		}

		internal void Add(ResponseTracking t,
			Func<ResponseTracking, bool> ready,
			Action<ResponseTracking, bool, bool> completion)
		{
			t.SetCompletionLogic(ready, completion);
			_lock.EnterWriteLock();
			try
			{
				_items.Add(t.Key, t);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		internal void Add(ResponseTracking t,	Action<ResponseTracking, bool, bool> completion)
		{
			t.SetCompletionLogic(null, completion);
			_lock.EnterWriteLock();
			try
			{
				_items.Add(t.Key, t);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		internal ResponseTracking Get(Guid id, int sequence)
		{
			ResponseKey k = new ResponseKey(id, sequence);
			ResponseTracking t;
			_lock.EnterWriteLock();
			try
			{
				_items.TryGetValue(k, out t);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
			return t;
		}
		
		bool Wait()
		{
			if (_state.TryTransition(MonitorState.Waiting, MonitorState.Started))
			{
				Thread.Sleep(CResponseTimeout);
				return _state.TryTransition(MonitorState.Started, MonitorState.Waiting);
			}
			return false;
		}

		void RunResponseTrackingLogic()
		{
			try
			{
				if (_state.TryTransition(MonitorState.Started, MonitorState.Starting))
				{
					WriteLine("Response tracking logic started.");
					while (Wait())
					{
						IEnumerable<ResponseTracking> items;
						_lock.EnterReadLock();
						try
						{
							items = _items.Values.ToArray();
						}
						finally
						{
							_lock.ExitReadLock();
						}
						if (items.Count() > 0)
						{
							var removals = new List<ResponseKey>();
							foreach(var i in items)
							{
								i.RunCompletionLogic();
								if (i.Completed)
									removals.Add(i.Key);
							}
							if (removals.Count > 0)
							{
								_lock.EnterWriteLock();
								try
								{
									foreach (var i in removals)
									{
										_items.Remove(i);
									}
								}
								finally
								{
									_lock.ExitWriteLock();
								}
							}
						}
					}
				}
			}
			finally
			{
				_state.SetState(MonitorState.Stopped);
				WriteLine("Response tracking logic stopped.");
			}
		}

		void WriteLine(params object[] args)
		{
			var dlg = OnWriteLine;
			YPMon.Debug<ResponseTrackingLogic>("ResponseTracking.WriteLine", String.Concat(args));
			if (dlg != null)
				dlg(this, args);
		}

		#region IDisposable Members
		~ResponseTrackingLogic()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing)
		{
			Thread.MemoryBarrier();
			var th = _th;
			Thread.MemoryBarrier();
			if (th != null && Interlocked.CompareExchange(ref _th, null, th) == th)
			{
				if (_state.SetStateIfLessThan(MonitorState.Stopping, MonitorState.Stopped))
				{
					th.Join(CResponseTimeout);
				}
				_state.SetState(MonitorState.Disposed);
			}
		}
		#endregion
	}
}
