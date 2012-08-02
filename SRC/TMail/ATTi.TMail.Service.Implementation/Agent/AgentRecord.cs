using System;
using System.Threading;
using ATTi.Core;

namespace ATTi.TMail.Service.Implementation.Agent
{
	public class AgentRecord
	{
		Guid _id;
		DateTime _observed;
		string _topic;
		public AgentRecord(Guid id)
		{
			_id = id;
		}
		public Guid ID { get { return _id; } }
		public LeadershipRole Role { get; set; }
		public DateTime LastObserved { get { return _observed; } }
		public string Topic
		{
			get
			{
				return Util.LazyInitialize(ref _topic, () => { return String.Concat("tmail.agent.", _id.ToString("D")); });
			}
		}
		public void Mark()
		{
			Thread.MemoryBarrier();
			_observed = DateTime.Now;
			Thread.MemoryBarrier();
		}
		public bool IsInDoubt(TimeSpan timespan)
		{
			// in doubt if it hasn't been observed in the timespan
			return _observed < DateTime.Now.Subtract(timespan);
		}
	}
		
}
