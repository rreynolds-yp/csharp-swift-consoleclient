using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using ATTi.Core.Factory.Implementer;

namespace ATTi.Core.Factory
{	
	/// <summary>
	/// Static factory utilities.
	/// </summary>
	public static class Factory
	{
		public static IFactory GetFactory(this Type t)
		{
			// TODO: Investigate other ways to do this.
			// Although this is very "dynamic" it is relatively slow.			
			Type f = typeof(Factory<>).MakeGenericType(t);
			return (IFactory)f.InvokeMember("DefaultFactory", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);			
		}	

		private static readonly string FactoryResourceName = "Factory.registry.xml";
		private static WeakReference __config;
		private static Object __lock = new Object();
		private static Dictionary<string, WeakReference> __bindings
			= new Dictionary<string, WeakReference>(67);
		//private static AssemblyDistributor __distro = new AssemblyDistributor();
		
		[ThreadStatic]
		private static Queue<Type> __typeResolution;

		internal static void AddTypeToResolutionQueue(Type t)
		{
			if (__typeResolution == null) __typeResolution = new Queue<Type>();
			__typeResolution.Enqueue(t);
		}

		internal static void StartResolutionQueue(Type t)
		{
			__typeResolution = new Queue<Type>();
			__typeResolution.Enqueue(t);
		}

		static Factory()
		{
		}

		internal static FactoryConfigurationSection Config
		{
			get
			{
				if (__config == null || !__config.IsAlive)
				{
					lock (__lock)
					{
						__config = new WeakReference(ConfigurationManager.GetSection(
							FactoryConfigurationSection.SectionName
							), true);						
					}
				}
				return __config.Target as FactoryConfigurationSection;
			}
		}

		/// <summary>
		/// Retrieves a target type's factory.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <returns>A factory for the target type.</returns>
		public static IFactory AccessFactoryForType(Type targetType)
		{
			return GetFactory(targetType);
		}

		private static Stream FindResourceStream(Type type)
		{
			Stream result = null;
			// Find a resource streem for the type's assembly
			// It may be in a namespace above the current type's namespace.
			// WARNING: This scheme only works for assemblies wherein the 
			// classes are declared in namespaces at or below the default
			// namespace for the assembly. Unfortunately the default namespace
			// only exists as a notion in the VS environment. Uhg! This little
			// fact complicates discovering embedded resources by name when
			// you're outside of the assembly.
			Assembly asm = type.Assembly;
			string n = (from rn in asm.GetManifestResourceNames()
									where rn.EndsWith(Factory.FactoryResourceName)
									select rn).SingleOrDefault();
			if (n != null)
			{
				result = asm.GetManifestResourceStream(n);
			}			
			return result;
		}

		private static IFactory ReflectFactoryInstance(Type type)
		{
			// TODO: Investigate other ways to do this.
			// Although this is very "dynamic" it is relatively slow.			
			Type t = typeof(Factory<>).MakeGenericType(type);
			PropertyInfo p = t.GetProperty("DefaultFactory", BindingFlags.Static);			
			return (IFactory)p.GetGetMethod().Invoke(null, null);
		}	
	}

}
