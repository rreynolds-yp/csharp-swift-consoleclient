using System;
using System.Configuration;
using ATTi.TMail.StrongMail.TransactionalApi;
using ATTi.Core;
using ATTi.TMail.StrongMail;
using Microsoft.Web.Services2.Security.Tokens;
using Microsoft.Web.Services2;
using ATTi.TMail.Service.Implementation.StrongMail;
using System.Net;

namespace ATTi.TMail.Service.Implementation.Configuration
{
	public class StrongMailConfigurationSection : ConfigurationSection
	{
		const string CSectionName = "strongMail";
		const string PropertyName_userName = "userName";
		const string PropertyName_password = "password";
		const string PropertyName_messageStudioUrl = "messageStudioApiUrl";
		const string PropertyName_easBatchApiUrl = "easBatchApiUrl";
		const string PropertyName_easTransactionalApiUrl = "easTransactionalApiUrl";

		static WeakReference __instance;

		[ConfigurationProperty(PropertyName_userName, IsRequired = true)]
		public string UserName
		{
			get { return (string)this[PropertyName_userName]; }
			set { this[PropertyName_userName] = value; }
		}

		[ConfigurationProperty(PropertyName_password, IsRequired = true)]
		public string Password
		{
			get { return (string)this[PropertyName_password]; }
			set { this[PropertyName_password] = value; }
		}

		[ConfigurationProperty(PropertyName_messageStudioUrl, IsRequired = false)]
		public string MessageStudioApiUrl
		{
			get { return (string)this[PropertyName_messageStudioUrl]; }
			set { this[PropertyName_messageStudioUrl] = value; }
		}

		[ConfigurationProperty(PropertyName_easTransactionalApiUrl, IsRequired = false)]
		public string EasTransactionalApiUrl
		{
			get { return (string)this[PropertyName_easTransactionalApiUrl]; }
			set { this[PropertyName_easTransactionalApiUrl] = value; }
		}

		[ConfigurationProperty(PropertyName_easBatchApiUrl, IsRequired = false)]
		public string EasBatchApiUrl
		{
			get { return (string)this[PropertyName_easBatchApiUrl]; }
			set { this[PropertyName_easBatchApiUrl] = value; }
		}

		public static StrongMailConfigurationSection Instance
		{
			get
			{
				if (__instance == null || __instance.IsAlive == false)
				{
					var config = ConfigurationManager.GetSection(CSectionName) as StrongMailConfigurationSection;
					if (config == null)
						throw new ConfigurationErrorsException("StrongMail configuration section not found");
					__instance = new WeakReference(config);
				}
				return (StrongMailConfigurationSection)__instance.Target;
			}
		}

		AuthDetails _authDetails;
		public AuthDetails EasApiCredentials
		{
			get { return Util.LazyInitialize(ref _authDetails,
				() => {
					var credentials = new AuthDetails();
					credentials.UserName = this.UserName;
					credentials.Password = this.Password;
					return credentials;
				}); 
			}			
		}

		public MailingService CreateMailingService()
		{
			if (String.IsNullOrEmpty(this.MessageStudioApiUrl))
				throw new ConfigurationErrorsException("The MessageStudio API URL is not present in the configuration file");
						
			ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((o, cert, chain, sslerr) =>
				{
					return true;
				});

			var svc = new MailingService(this.MessageStudioApiUrl);
			UsernameToken userToken = new UsernameToken(this.UserName, this.Password, PasswordOption.SendPlainText);
			SoapContext ctx = svc.RequestSoapContext;
			ctx.Security.Tokens.Add(userToken);
						
			svc.Pipeline.OutputFilters.Insert(0, new InjectOrganizationInWsseSecurity("ATTI"));

			svc.RequestEncoding = System.Text.Encoding.UTF8;
			return svc;
		}

		public EasBatchApi CreateBatchApiWebService()
		{
			if (String.IsNullOrEmpty(this.EasBatchApiUrl))
				throw new ConfigurationErrorsException("The EAS Batch API URL is not present in the configuration file");
			var batchApi = new EasBatchApi(this.EasBatchApiUrl);
			batchApi.RequestEncoding = System.Text.Encoding.UTF8;
			return batchApi;
		}

		public EasTransactionalApi CreateTransactionalApiWebService()
		{
			if (String.IsNullOrEmpty(this.EasTransactionalApiUrl))
				throw new ConfigurationErrorsException("The EAS Transactional API URL is not present in the configuration file");
			var easApi = new EasTransactionalApi(this.EasTransactionalApiUrl);
			easApi.RequestEncoding = System.Text.Encoding.UTF8;
			return easApi;
		}
	}
}
