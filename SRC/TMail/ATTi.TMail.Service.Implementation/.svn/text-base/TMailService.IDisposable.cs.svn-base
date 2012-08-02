using System;

namespace ATTi.TMail.Service.Implementation
{
	public partial class TMailService : IDisposable
	{
		~TMailService()
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
			var agent = Agent;
			if (agent != null)
			{
				agent.Dispose();
				Agent = null;
			}
		}
	}
}
