using System;
using System.Configuration;
using System.Linq;
using ATTi.Core.Configuration;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace ATTi.Core.MessageBus.Configuration
{		
	public class AmqpConnectionConfigurationElement : ConfigurationElement
	{
		const string PropertyName_name = "name";
		const string PropertyName_endpoints = "endpoints";
		const string PropertyName_userName = "userName";
		const string PropertyName_password = "password";
		const string PropertyName_virtualHost = "virtualHost";

		const string PropertyName_allowEndpointRedirects = "allowEndpointRedirects";
		const string PropertyName_maxEndpointRedirects = "maxEndpointRedirects";
		
		/// <summary>
		/// Gets or sets the connection's name.
		/// </summary>
		/// <value>The connection's name.</value>
		[ConfigurationProperty(PropertyName_name
			, IsKey = true
			, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set { this[PropertyName_name] = value; }
		}

		[ConfigurationProperty(PropertyName_userName
			, IsRequired = false)]
		public string UserName
		{
			get { return (string)this[PropertyName_userName]; }
			set { this[PropertyName_userName] = value; }
		}

		[ConfigurationProperty(PropertyName_password
			, IsRequired = false)]
		public string Password
		{
			get { return (string)this[PropertyName_password]; }
			set { this[PropertyName_password] = value; }
		}

		[ConfigurationProperty(PropertyName_virtualHost
			, IsRequired = false)]
		public string VirtualHost
		{
			get { return (string)this[PropertyName_virtualHost]; }
			set { this[PropertyName_virtualHost] = value; }
		}

		[ConfigurationProperty(PropertyName_allowEndpointRedirects
			, IsRequired = false)]
		public bool AllowEndpointRedirects
		{
			get { return (bool)this[PropertyName_allowEndpointRedirects]; }
			set { this[PropertyName_allowEndpointRedirects] = value; }
		}
		
		[ConfigurationProperty(PropertyName_maxEndpointRedirects
			, IsRequired = false)]
		public int MaxEndpointRedirects
		{
			get { return (int)this[PropertyName_maxEndpointRedirects]; }
			set { this[PropertyName_maxEndpointRedirects] = value; }
		}

		[ConfigurationProperty(PropertyName_endpoints
			, IsRequired = true)]
		public AmqpEndpointConfigurationElementCollection Endpoints
		{
			get { return (AmqpEndpointConfigurationElementCollection)this[PropertyName_endpoints]; }
			set { this[PropertyName_endpoints] = value; }
		}
				
		public IConnection CreateConnection()
		{
			var factory = new ConnectionFactory();

			factory.Parameters.UserName = (String.IsNullOrEmpty(UserName)) ? ConnectionParameters.DefaultUser : UserName;
			factory.Parameters.Password = (String.IsNullOrEmpty(Password)) ? ConnectionParameters.DefaultPass : Password;
			factory.Parameters.VirtualHost = (String.IsNullOrEmpty(VirtualHost)) ? ConnectionParameters.DefaultVHost : VirtualHost;

			var endpoints = (from e in Endpoints.StrongTypeEnumerable
											 select e.EndpointInstance).ToArray<AmqpTcpEndpoint>();

			if (AllowEndpointRedirects)
			{
				return factory.CreateConnection(MaxEndpointRedirects, endpoints);				
			}
			else
			{
				return factory.CreateConnection(endpoints);
			}
		}
	}

	public class AmqpConnectionConfigurationElementCollection : AbstractConfigurationElementCollection<AmqpConnectionConfigurationElement, string>
	{
		protected override string PerformGetElementKey(AmqpConnectionConfigurationElement element)
		{
			return element.Name;
		}
	}

}
