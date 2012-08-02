using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace ATTi.Core.CallContext
{
	public class ContextClientSink : BaseChannelSinkWithProperties, IClientChannelSink,
		IMessageSink
	{
		#region Declarations
		IMessageSink _nextSink;
		#endregion

		#region Constructors
		[SecurityPermission(SecurityAction.LinkDemand)]
		public ContextClientSink(object next)
		{
			if (!typeof(IMessageSink).IsInstanceOfType(next))
				throw new ArgumentOutOfRangeException("next");

			_nextSink = (IMessageSink)next;
		}
		#endregion

		#region IClientChannelSink Members
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public System.IO.Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			throw new NotImplementedException();
		}

		public IClientChannelSink NextChannelSink
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
			get
			{
				throw new NotImplementedException();
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IMessageSink Members
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{

			throw new NotImplementedException();
		}

		public IMessageSink NextSink
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
			get { return _nextSink; }
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public IMessage SyncProcessMessage(IMessage msg)
		{
			object handback = ContextUtil.NotifyClientMessage(msg);
			IMessage result = _nextSink.SyncProcessMessage(msg);
			ContextUtil.NotifyClientMessageReturn(handback, result);
			return result;
		}

		#endregion
	}

	public class ContextClientSinkProvider : IClientChannelSinkProvider
	{
		// The next provider in the chain.
		IClientChannelSinkProvider _nextProvider;

		public IClientChannelSinkProvider Next
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
			get { return (_nextProvider); }
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
			set { _nextProvider = value; }
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public IClientChannelSink CreateSink(IChannelSender channel, String url, Object remoteChannelData)
		{
			// Create the next sink in the chain.
			IClientChannelSink nextSink = _nextProvider.CreateSink(channel, url, remoteChannelData);

			// Hook our sink up to it.
			return (new ContextClientSink(nextSink));
		}

		// This constructor is required in order to use the provider in file-based configuration.
		// It need not do anything unless you want to use the information in the parameters.
		public ContextClientSinkProvider(IDictionary properties, ICollection providerData) { }
	}
}
