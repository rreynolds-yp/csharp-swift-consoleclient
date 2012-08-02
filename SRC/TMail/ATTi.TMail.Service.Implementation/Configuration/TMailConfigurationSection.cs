using System.Configuration;
using System;
using System.Linq;
using ATTi.Core;

namespace ATTi.TMail.Service.Implementation.Configuration
{
	public class TMailConfigurationSection : ConfigurationSection
	{
		const string CSectionName = "tmail";
		const string PropertyName_submissionMode = "submissionMode";
		const string PropertyName_allowedEnvironments = "allowedEnvironments";
		const string PropertyName_numberOfAmqpRetriesBeforeHardFail = "numberOfAmqpRetriesBeforeHardFail";
        const string PropertyName_ServerMode = "isAgentMode";
        const string PropertyName_TraceExecutionContext = "traceExecutionContext";
        bool isServerMode;
        bool shouldTraceExecutionContext;

		static WeakReference __instance;

		[ConfigurationProperty(PropertyName_numberOfAmqpRetriesBeforeHardFail, IsRequired = false, DefaultValue = 3)]
		public int NumberOfAmqpRetriesBeforeHardFail
		{
			get { return (int)this[PropertyName_numberOfAmqpRetriesBeforeHardFail]; }
			set { this[PropertyName_numberOfAmqpRetriesBeforeHardFail] = value; }
		}

        [ConfigurationProperty(PropertyName_submissionMode, IsRequired = false, DefaultValue = "QueuedForAgent")]
        public string SubmissionMode
        {
            get { return (string)this[PropertyName_submissionMode]; }
            set { this[PropertyName_submissionMode] = value; }
        }

		[ConfigurationProperty(PropertyName_allowedEnvironments, IsRequired = false, DefaultValue = "dev")]
		public string AllowedEnvironments
		{
			get { return (string)this[PropertyName_allowedEnvironments]; }
			set
			{
				this[PropertyName_allowedEnvironments] = value;
				_environments = null;
			}
		}
        
        //TODO do a enum method instead if stay at freidly string
        [ConfigurationProperty(PropertyName_ServerMode, IsRequired = true, DefaultValue = "true")]
        public string ServerOperationMode
        {
            get { return (string)this[PropertyName_ServerMode]; }
            set
            {
                this[PropertyName_ServerMode] = value;           
            }
        }

		public  bool IsServerOperationMode 
		{ 
			get
			{
                bool.TryParse(PropertyName_ServerMode, out isServerMode);
                return isServerMode;
			}
		}

        [ConfigurationProperty(PropertyName_TraceExecutionContext, IsRequired = false, DefaultValue = "false")]
        public string TraceExecutionContext
        {
            get { return (string)this[PropertyName_TraceExecutionContext]; }
            set
            {
                this[PropertyName_TraceExecutionContext] = value;
            }
        }

        public bool IsTraceExecutionContext
        {
            get
            {
                bool.TryParse(PropertyName_TraceExecutionContext, out shouldTraceExecutionContext);
                return shouldTraceExecutionContext;
            }
        }
        
		string[] _environments;
		public bool IsEnvironmentAllowed(string env)
		{
			var allowed = Util.LazyInitialize(ref _environments, () =>
				{
					return (from e in AllowedEnvironments.Split(',')
									select e.Trim()).ToArray();
				});
			return allowed.Contains(env);
		}

		public static TMailConfigurationSection Instance
		{
			get
			{
				if (__instance == null || __instance.IsAlive == false)
				{
					var config = ConfigurationManager.GetSection(CSectionName) as TMailConfigurationSection;
					if (config == null)
						throw new ConfigurationErrorsException("TMail configuration section not found");
					__instance = new WeakReference(config);
				}
				return (TMailConfigurationSection)__instance.Target;
			}
		}
	}
}
