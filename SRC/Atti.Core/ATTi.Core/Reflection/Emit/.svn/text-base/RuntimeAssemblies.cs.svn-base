using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ATTi.Core.Trace;

namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Utility class for emitting assemblies and tracking those assemblies so
	/// that type resolution works for the emitted types.
	/// </summary>
	public static class RuntimeAssemblies
	{
		static Dictionary<string, Assembly> __asm = new Dictionary<string, Assembly>();

		static RuntimeAssemblies()
		{
			AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(CurrentDomain_TypeResolve);
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_TypeResolve);
		}

		static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
		{
			Traceable.TraceData(typeof(RuntimeAssemblies), TraceEventType.Error, String.Concat(
							"CLR could not locate assembly or type: ", args.Name));
			return null;
		}

		public static AssemblyName MakeEmittedAssemblyNameFromAssembly(string nameFormat, Assembly target)
		{
			Contracts.Require.IsNotEmpty("nameFormat", nameFormat);
			Contracts.Require.IsNotNull("target", target);

			AssemblyName tasmName = target.GetName();
			AssemblyName asmName = new AssemblyName(String.Format(nameFormat
					, tasmName.Name
					, tasmName.Version
					, Assembly.GetCallingAssembly().GetName().Version).Replace('.', '_')
					);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}

		public static Assembly GetEmittedAssembly(string name)
		{
			Contracts.Require.IsNotNull("name", name);

			Assembly asm;
			lock (__asm)
			{
				__asm.TryGetValue(name, out asm);
			}
			return asm;
		}
		public static Assembly GetEmittedAssemblyWithEmitWhenNotFound(AssemblyName name, Action<EmittedAssembly> emitter)
		{
			Contracts.Require.IsNotNull("name", name);
			Contracts.Require.IsNotNull("emitter", emitter);

			Assembly asm;
			lock (__asm)
			{
				if (!__asm.TryGetValue(name.FullName, out asm))
				{
					EmittedAssembly emittedAsm;
					try
					{
						emittedAsm = new EmittedAssembly(name, String.Empty);
						emitter(emittedAsm);
						asm = emittedAsm.Compile();
					}
					catch (Exception e)
					{
						Traceable.TraceData(typeof(RuntimeAssemblies), TraceEventType.Error, String.Concat(
							"Unable to generate and compile assembly: ", name.FullName), e.FormatForLogging());
						throw;
					}
					
#if DEBUG
					try
					{
						emittedAsm.Save();
					}
					catch (IOException e)
					{
						Traceable.TraceData(typeof(RuntimeAssemblies), TraceEventType.Verbose, e.FormatForLogging());
					}
#endif
					__asm.Add(name.FullName, asm);
				}
			}
			return asm;
		}		
	}
	/*
ATTi.Core.Wireup.IWireupCommandv0.1.6950.25351__ImplFactory_0.1.6950.25351_for_ATTi.TMail.Service.Implementation.LocalWireupCommand
	 */
}
