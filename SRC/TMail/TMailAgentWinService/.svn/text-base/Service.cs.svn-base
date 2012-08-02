namespace ATTi.TMailAgent.WindowsService
{
	using System;
	using System.Diagnostics;
	using System.ServiceProcess;
	using System.Threading;
	using ATTi.Core;
	using ATTi.Core.Trace;
    using ATTi.TMail.Common;
	using ATTi.TMail.Service.Implementation;
	using ATTi.TMail.Service.Implementation.Agent;

	public partial class TMailAgentService : ServiceBase, ITraceable
	{
		

		public TMailAgentService()
		{
            WriteDebugLine("Initializing");
			InitializeComponent();
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            WriteDebugLine("Initialized");
            //YPMon.Info(this.ServiceName, "TMailAgentService Initialized");
            //Traceable.TraceData(typeof(TMailAgentService), TraceEventType.Information, "TMailAgentService Initialed");
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				//this.LogInfo("Unhandled Exception in TmailAgentService: {0}",ex.Message);
				this.TraceData(TraceEventType.Error, "Unhandled error in the AppDomain:", ex.FormatForLogging());
			}
		}

		public void Run(string[] args)
		{
            //this.TraceData(TraceEventType.Information, "TMailAgent Starting ..");
            WriteDebugLine("Starting to Run .....");
			//this.LogInfo("TMailAgent Starting ..");
			OnStart(args);
            WriteDebugLine("is Running...");
            //this.TraceData(TraceEventType.Information, "TMailAgent Started ..");
			//this.LogInfo("TMailAgent Started .. ");
        }

        internal void RunInDebug(string[] args)
        {
            OnStart(args);

            Console.WriteLine("Running in debug mode.");
            Console.WriteLine("Press ENTER to terminate...");
            Console.ReadLine();

            OnStop();
        }

        const bool _asAgent = true;
		protected override void OnStart(string[] args)
		{
			Thread.MemoryBarrier();
			var agent = _agent;
			Thread.MemoryBarrier();

			if (agent == null)
			{
                //this.TraceData(TraceEventType.Information, "TMailAgent OnStarting ..");
                WriteDebugLine("OnStarting ..");
				agent = new TMailService();
				agent.OnWriteLine += new LineWriterDelegate(agent_OnWriteLine);
				if (Interlocked.CompareExchange(ref _agent, agent, null) == null)
				{
                    //WriteLine("OnStart Begin");
                    //this.TraceData(TraceEventType.Information, "TMailAgent OnStart Begin");
                    WriteDebugLine("PerformStart Begin");
					PerfromStart(agent);
                    WriteDebugLine("PerformStart Done");
					//this.TraceData(TraceEventType.Information, "TMailAgentService started");
				}
                //this.TraceData(TraceEventType.Information, "TMailAgent OnStart Completed ..");
			}
		}

		
		void agent_OnWriteLine(object sender, params object[] args)
		{
            this.TraceData(TraceEventType.Information,String.Concat(args));
			//this.TraceData(TraceEventType.Information, String.Concat(args));
		}

		protected override void OnStop()
		{
			Thread.MemoryBarrier();
			var agent = _agent;
			Thread.MemoryBarrier();

			if (agent != null)
			{
				
                WriteDebugLine("OnStop Begin");
				agent.StopAgentLogic();
				// The agent is still listening on it's private topic
                WriteDebugLine("OnStop Done");
			}
		}

		protected override void OnShutdown()
		{            
            WriteDebugLine("Shutdown Begin");			
			Util.Dispose(ref _agent);
			WriteDebugLine("Shutdown Done");
        }

        private void WriteDebugLine(params object[] args)
        {
			YPMon.Debug<TMailAgentService>("TMailAgentService ", String.Concat(args));
        }	

		private void PerfromStart(TMailService agent)
		{
			try
			{
                //this.TraceData(TraceEventType.Information, "TMailAgent PerformStarting ..");
                WriteDebugLine("PerformStart Begin..");
				//this.LogInfo("TMailAgent PerformStarting ..");
				agent.StartAgentLogic(null);
                 WriteDebugLine("PerformStart Done..");
                //this.TraceData(TraceEventType.Information, "TMailAgent PerformStarted ..");
				//this.LogInfo("TMailAgent PerformStarted ..");			
				            
			}
			catch (Exception ex)
			{
                //this.TraceData(TraceEventType.Information, "TMailAgent failed to PerformStart: {0}", ex.Message);
				//this.LogInfo("TMailAgent failed to PerformStart: {0}",ex.Message);
				//this.TraceData(TraceEventType.Error, "Unexpected Error In PerformStart", ex.FormatForLogging());
                YPMon.Critical("TMailAgentService", "Unexpected Error In PerformStart", ex.FormatForLogging());
				throw;
			}
		}

		private void PerformStop(AgentProtocol agent)
		{
			try
			{
				agent.StopAgentLogic();
			}
			catch (Exception ex)
			{

				//this.LogInfo("TMailAgent Failed to Stop : {0}",ex.Message);
				//this.TraceData(TraceEventType.Error, "Unexpected Error In PerformStart", ex.FormatForLogging());
                YPMon.Critical("TMailAgentService", "Unexpected Error In PerformStop", ex.FormatForLogging());
				throw;
			}
		}
	}
}
