namespace ATTi.Core.Wireup
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;

	using ATTi.Core.Factory;
	using ATTi.Core.Reflection;
	using ATTi.Core.Wireup.Metadata;

	/// <summary>
	/// Utility class for coordinating wireup.
	/// </summary>
	public static class WireupCoordinator
	{
		static Object __lock = new Object();
		static Stack<Assembly> __assemblies = new Stack<Assembly>();
		static Stack<Type> __types = new Stack<Type>();
		static bool __isConfigured = false;
		static Dictionary<object, object> __asmTracking = new Dictionary<object, object>();
		
		/// <summary>
		/// Coordinates wireup for an assembly.
		/// </summary>
		/// <param name="asm">The assembly to wireup.</param>
		public static void CoordinateWireup(Assembly asm)
		{
			Contracts.Require.IsNotNull("asm", asm);

			lock (__lock)
			{
				if (!__isConfigured) SelfConfigure();
				if (!IsAssemblyWired(asm))
				{
					WireupDependencies(asm);
				}
			}
		}

		/// <summary>
		/// Causes the wireup coordinator to self-configure.
		/// </summary>
		public static void SelfConfigure()
		{
			Contracts.Invariant.AssertState(__isConfigured == false, "already configured");

			__isConfigured = true;
			WireupConfigurationSection config = ConfigurationManager.GetSection(WireupConfigurationSection.SectionName)
				as WireupConfigurationSection;
			if (config != null)
			{
				foreach (WireupElement e in config.Assemblies)
				{
					e.PerformWireup();
				}
			}
		}

		private static void InvokeWireupCommand(Type t)
		{
			if (!__types.Contains(t))
			{
				__types.Push(t);
				try
				{
					// Assemblies may have dependencies declared; wire them first.
					foreach (WireupDependencyAttribute d in t.GetCustomAttributes(typeof(WireupDependencyAttribute), false))
					{
						Type r = d.ReliantType;
						WireupDependencies(r.Assembly);
					}
					IWireupCommand cmd = Factory<IWireupCommand>.CreateImplInstance(t);
					cmd.Execute();
				}
				finally
				{
					__types.Pop();
				}
			}
		}

		private static bool IsAssemblyWired(Assembly asm)
		{
			return __asmTracking.ContainsKey(asm.GetSafeLock());
		}

		private static void WireupDependencies(Assembly asm)
		{
			// The stacks are used to avoid cycles among the dependency declarations.
			if (!IsAssemblyWired(asm) && !__assemblies.Contains(asm))
			{
				__assemblies.Push(asm);
				try
				{
					// Assemblies may have dependencies declared.
					foreach (WireupDependencyAttribute d in asm.GetCustomAttributes(typeof(WireupDependencyAttribute), false))
					{
						Type t = d.ReliantType;
						WireupDependencies(t.Assembly);
						if (typeof(IWireupCommand).IsAssignableFrom(t) && !__types.Contains(t))
							InvokeWireupCommand(t);
					}

					// Execute the wireup commands declared for the assembly...
					foreach (WireupAttribute w in asm.GetCustomAttributes(typeof(WireupAttribute), false))
					{
						Type t = w.CommandType;
						if (!__types.Contains(t))
							InvokeWireupCommand(t);
					}

					if (!IsAssemblyWired(asm))
						__asmTracking.Add(asm.GetSafeLock(), null);
				}
				finally
				{
					__assemblies.Pop();
				}
			}
		}

		
	}
}