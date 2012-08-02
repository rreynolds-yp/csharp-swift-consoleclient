using System.Diagnostics;
using System.ServiceProcess;

namespace ATTi.TMailAgent.WindowsService
{

    static class DebugConsole
	{
		/// <summary>
		/// The main entry point for the application. What the What ?
		/// </summary>
        static void Main(string[] args)
        {
            bool debug = args.Length != 0 && args[0] == "-d";

            TMailAgentService service = new TMailAgentService();
            if (debug)
            {
                service.RunInDebug(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
            { 
                service
            };
                ServiceBase.Run(ServicesToRun);
            }
            //DebuggWinService();
		}

		[Conditional("DEBUG")]
		static void DebuggWinService()
		{
			TMailAgentService agentService = new TMailAgentService();
			agentService.Run(null);
		}
	}
}
