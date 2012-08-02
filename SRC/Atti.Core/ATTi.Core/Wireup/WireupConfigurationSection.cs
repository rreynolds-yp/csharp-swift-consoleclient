namespace ATTi.Core.Wireup
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.Reflection;

	using ATTi.Core.Configuration;
	using ATTi.Core.Trace;
	using ATTi.Core.Properties;

	public class WireupConfigurationElementCollection : AbstractConfigurationElementCollection<WireupElement, string>
	{		
		protected override string PerformGetElementKey(WireupElement element)
		{
			return element.AssemblyName;
		}		
	}

	public sealed class WireupConfigurationSection : ConfigurationSection
	{

		public const string PropertyName_assemblies = "assemblies";
		public const string SectionName = "wireup";
		
		[ConfigurationProperty(WireupConfigurationSection.PropertyName_assemblies, IsDefaultCollection = true)]
		public WireupConfigurationElementCollection Assemblies
		{
			get { return (WireupConfigurationElementCollection)this[PropertyName_assemblies]; }
		}		
	}

	public class WireupElement : ConfigurationElement, ITraceable
	{
		public const string PropertyName_assembly = "assembly";

		Assembly _asm;
		
		[ConfigurationProperty(WireupElement.PropertyName_assembly
			, IsKey = true
			, IsRequired = true)]
		public string AssemblyName
		{
			get { return (string)this[WireupElement.PropertyName_assembly]; }
			set { this[WireupElement.PropertyName_assembly] = value; }
		}

		internal Assembly ResolveAssembly
		{
			get
			{
				if (_asm == null)
				{
					try
					{
						if (!String.IsNullOrEmpty(this.AssemblyName))
						{
							_asm = Assembly.Load(this.AssemblyName);
						}
					}
					catch (Exception e)
					{
						this.TraceData(TraceEventType.Error
							, String.Format(Resources.Error_ErrorDuringAssemblyLoad, WireupElement.PropertyName_assembly
							, this.AssemblyName)
							, e);
						throw;
					}
				}
				return _asm;
			}
		}		

		internal void PerformWireup()
		{
			if (this.ResolveAssembly != null)
			{
				WireupCoordinator.CoordinateWireup(this.ResolveAssembly);
			}
		}		
	}
}