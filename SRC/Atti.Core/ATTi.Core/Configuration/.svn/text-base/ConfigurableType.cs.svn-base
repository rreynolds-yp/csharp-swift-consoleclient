namespace ATTi.Core.Configuration
{
	using System;
	using System.Configuration;
	using System.Reflection;

	using ATTi.Core.Configuration.Metadata;
	using ATTi.Core.Reflection;

	public static class ConfigurableType<T>
	{
		

		static Object __sync = new Object();
		static bool __isConfigured;

		

		

		public static void ConfigureType()
		{
			if (!__isConfigured)
			{
				Type t = typeof(T);
				ConfigurableTypeAttribute attr = t.DiscoverFirstCustomAttribute<ConfigurableTypeAttribute>(true);
				if (attr != null)
				{
					object config = null;
					ConfigurableTypeConfigurationSection section = ConfigurationManager.GetSection(ConfigurableTypeConfigurationSection.SectionName) as ConfigurableTypeConfigurationSection;
					if (section != null)
					{
						ConfigurableTypeConfigurationElement element = section.Types[t.AssemblyQualifiedName];
						if (element == null) element = section.Types[t.GetSimpleAssemblyQualifiedName()];
						if (element != null)
						{
							config = element.GetTypeConfig();
						}
					}

					MethodInfo cm = t.GetMethod("ConfigureType"
						, BindingFlags.Static | BindingFlags.Public
						, null
						, new Type[] { attr.ConfigElementType }
						, null
						);
					if (cm == null)
						throw new InvalidOperationException(
							String.Format(Properties.Resources.Error_ConfigureTypeMethodNotFoundOnType, t.FullName));

					cm.Invoke(null, new object[] { config });
				}
				__isConfigured = true;
			}
		}

		
	}
}