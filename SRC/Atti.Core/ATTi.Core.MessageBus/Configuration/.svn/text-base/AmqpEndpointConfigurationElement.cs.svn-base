using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATTi.Core.Configuration;
using RabbitMQ.Client;
using System.Configuration;

namespace ATTi.Core.MessageBus.Configuration
{
	/// <summary>
	/// AMQP Endoint configuration element.
	/// </summary>
	public class AmqpEndpointConfigurationElement : ConfigurationElement
	{
		const string PropertyName_uri = "uri";
		const string PropertyName_protocolResolution = "protocolResolution";
		const string PropertyName_protocol = "protocol";

		IProtocol _protocol;
		AmqpTcpEndpoint _endpoint;

		/// <summary>
		/// Gets or sets the endpoint name.
		/// </summary>
		/// <value>The endpoint name.</value>
		[ConfigurationProperty(PropertyName_uri
			, IsKey = true
			, IsRequired = true)]
		public string Uri
		{
			get { return (string)this[PropertyName_uri]; }
			set { this[PropertyName_uri] = value; }
		}

		/// <summary>
		/// Gets or sets the strategy used to resolve the protocol name. { ByName | FromConfiguration | FromEnvironment }
		/// </summary>
		/// <value>The protocol resolution.</value>
		[ConfigurationProperty(PropertyName_protocolResolution
			, IsRequired = false
			, DefaultValue = ProtocolResolutionKind.ByName)]
		public ProtocolResolutionKind ProtocolResolution
		{
			get { return (ProtocolResolutionKind)this[PropertyName_protocolResolution]; }
			set { this[PropertyName_protocolResolution] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the protocol.
		/// </summary>
		/// <value>The name of the protocol.</value>
		[ConfigurationProperty(PropertyName_protocol
			, IsRequired = false)]
		public string ProtocolName
		{
			get { return (string)this[PropertyName_protocol]; }
			set { this[PropertyName_protocol] = value; }
		}

		internal IProtocol ProtocolInstance
		{
			get
			{
				if (_protocol == null && !String.IsNullOrEmpty(this.ProtocolName))
				{
					_protocol = Protocols.DefaultProtocol;
				}
				else
				{
					switch (ProtocolResolution)
					{
						case ProtocolResolutionKind.ByName:
							_protocol = Protocols.SafeLookup(ProtocolName);
							break;
						case ProtocolResolutionKind.FromConfiguration:
							_protocol = Protocols.FromConfiguration(ProtocolName);
							break;
						case ProtocolResolutionKind.FromEnvironment:
							_protocol = Protocols.FromEnvironment(ProtocolName);
							break;
					}
				}
				return _protocol;
			}
		}

		internal AmqpTcpEndpoint EndpointInstance
		{
			get
			{
				if (_endpoint == null)
				{
					_endpoint = new AmqpTcpEndpoint(ProtocolInstance, new Uri(this.Uri));
				}
				return _endpoint;
			}
		}
	}

	/// <summary>
	/// Collection of MementoConfigurationElements.
	/// </summary>
	public class AmqpEndpointConfigurationElementCollection : AbstractConfigurationElementCollection<AmqpEndpointConfigurationElement, string>
	{
		protected override string PerformGetElementKey(AmqpEndpointConfigurationElement element)
		{
			return element.Uri;
		}
	}

}
