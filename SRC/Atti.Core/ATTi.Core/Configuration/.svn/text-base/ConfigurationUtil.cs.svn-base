using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ATTi.Core.Properties;

namespace ATTi.Core.Configuration
{
	public static class ConfigurationUtil
	{
		public static readonly string AppSettingsKey_OverrideConfigurationSectionNesting = "configurationSectionNesting";
		public static readonly string ConfigurationSectionNesting = typeof(ConfigurationUtil).Namespace;

		static ConfigurationUtil()
		{
			string overrideNesting = ConfigurationManager.AppSettings[AppSettingsKey_OverrideConfigurationSectionNesting];
			if (!string.IsNullOrEmpty(overrideNesting))
			{
				ConfigurationUtil.ConfigurationSectionNesting = overrideNesting;
			}
		}

		public static T AssertSection<T>(string sectionName)
			where T : ConfigurationSection
		{
			T result = EnsureSectionOrDefault<T>(sectionName);
			if (result == default(T))
				throw new ConfigurationErrorsException(String.Format(
					Resources.Error_MissingConfigurationSection, sectionName, typeof(T).FullName));
			return result;
		}

		public static T EnsureSectionOrDefault<T>(string sectionName)
			where T : ConfigurationSection
		{
			T result = ConfigurationManager.GetSection(String.Concat(ConfigurationUtil.ConfigurationSectionNesting, '/', sectionName)) as T;
			if (result == null)
			{ // Fall back to the root level if the section isn't located at the nested location.
				result = ConfigurationManager.GetSection(sectionName) as T;
			}
			return result;
		}
	}

}
