using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Collections;

namespace ATTi.Core.CallContext
{
	public class ContextServerSink : BaseChannelSinkWithProperties, IServerChannelSink
	{
		#region Declarations
		 IServerChannelSink _nextSink;
		#endregion

		#region Constructors
		public ContextServerSink(IServerChannelSink next)
		{
			_nextSink = next;
		}
		#endregion

		#region IServerChannelSink Members

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack,
			object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			ContextUtil.NotifyServerMessageResponse(msg, headers, state);
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack,
			object state, IMessage msg, ITransportHeaders headers)
		{
			return _nextSink.GetResponseStream(sinkStack, state, msg, headers);
		}

		public IServerChannelSink NextChannelSink
		{
			get { return _nextSink; }
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack,
			IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream,
			out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			object handback = ContextUtil.NotifyServerMessage(requestMsg, requestHeaders);
			sinkStack.Push(this, handback);

			ServerProcessing result = _nextSink.ProcessMessage(sinkStack,
				requestMsg, requestHeaders, requestStream,
				out responseMsg,
				out responseHeaders,
				out responseStream);

			if (result != ServerProcessing.Async)
			{
				ContextUtil.NotifyServerMessageResponse(responseMsg, responseHeaders, handback);
			}
			return result;
		}

		#endregion
	}

	public class ContextServerSinkProvider : IServerChannelSinkProvider
	{
		#region Declarations
		IServerChannelSinkProvider _next;
		#endregion

		#region Constructors
		public ContextServerSinkProvider(IDictionary properties,
			ICollection providerData)
		{
			/* not needed */
		}
		#endregion

		#region IServerChannelSinkProvider Members

		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			//
			// Put our sink on top of the chain...
			//
			IServerChannelSink next = _next.CreateSink(channel);
			return new ContextServerSink(next);
		}

		public IServerChannelSinkProvider Next
		{
			get { return _next; }
			set { _next = value; }
		}

		public void GetChannelData(IChannelDataStore channelData)
		{
			/* empty */
		}

		#endregion
	}
}
