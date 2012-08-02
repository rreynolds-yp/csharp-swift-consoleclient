using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using ATTi.Core;
using ATTi.TMail.Service.Implementation;
using ATTi.TMail.Service.Implementation.Agent;

namespace TMailAgent_debug_console
{
	class Program
	{
		static void Main(string[] args)
		{
			OnWriteLine(null, "Starting TMail Agent v", typeof(AgentProtocol).Assembly.GetName().Version.ToString());
			var done = false;

			var operations = "commands: start, stop, exit";
			var shortPrompt = ">";

			#region periodically write the prompt...
			ThreadPool.QueueUserWorkItem(new WaitCallback(unused =>
				{
					var count = 0;
					var prompt = 0;
					do
					{
						var n = Interlocked.CompareExchange(ref __writeCounter, count, count);
						if (n != count)
						{
							lock (Console.Out)
							{
								Console.Write(Environment.NewLine);
								if (n > prompt + 20)
								{
									Console.Write(operations);
									prompt = n;
								}
								Console.Write(shortPrompt);
								__prompt = true;
							}
							count = n;
						}
						Thread.Sleep(200);
					} while (!done);
				}));
			#endregion
						
			var timeout = DateTime.UtcNow;
			var input = "priv";
			while (!String.Equals("exit", input, StringComparison.CurrentCultureIgnoreCase))
			{
				switch (input)
				{
					case "start": PerformStart(); break;
					case "stop": PerformStop(); break;
				}
				input = Console.ReadLine();
			} while (!String.Equals(input, "exit", StringComparison.CurrentCultureIgnoreCase)) ;

			Util.Dispose(ref __agent);

			Thread.Sleep(1000);
		}

		private static void PerformStop()
		{
			Util.Dispose(ref __agent);
		}

		private static void PerformStart()
		{
			Thread.MemoryBarrier();
			var svc = __agent;
			Thread.MemoryBarrier();

			if (svc == null)
			{
				svc = new TMailService();
				if (Interlocked.CompareExchange(ref __agent, svc, null) == null)
				{
					svc.StartAgentLogic((s, t) =>
						{
							svc.OnWriteLine += OnWriteLine;
							OnWriteLine(s, "Agent service started");
						});
				}
			}
		}

		static int __writeCounter;
		static volatile bool __prompt;
		static TMailService __agent;

		private static void OnWriteLine(object sender, params object[] lineparts)
		{
			lock (Console.Out)
			{
				if (__prompt)
				{
					Console.Write(Environment.NewLine);
					Interlocked.Increment(ref __writeCounter);
					__prompt = false;
				}
				Console.WriteLine(String.Concat(lineparts));
				Interlocked.Increment(ref __writeCounter);
			}
		}
	}

	public class Arguments
	{
		StringDictionary _params;

		public Arguments(string[] args)
		{
			_params = new StringDictionary();
			Regex spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			string parm = null;
			string[] parts;

			foreach (var a in args)
			{
				parts = spliter.Split(a, 3);
				switch (parts.Length)
				{
					case 1:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
							{
								parts[0] = remover.Replace(parts[0], "$1");
								_params.Add(parm, parts[0]);
							}
							parm = null;
						}
						break;
					case 2:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
								_params.Add(parm, "true");
						}
						parm = parts[1];
						break;
					case 3:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
								_params.Add(parm, "true");
						}
						parm = parts[1];
						if (!_params.ContainsKey(parm))
						{
							parts[2] = remover.Replace(parts[2], "$1");
							_params.Add(parm, parts[2]);
						}

						parm = null;
						break;
				}
			}

			if (parm != null)
			{
				if (!_params.ContainsKey(parm))
					_params.Add(parm, "true");
			}
		}

		public string this[string Param]
		{
			get
			{
				return (_params[Param]);
			}
		}
	}

}
