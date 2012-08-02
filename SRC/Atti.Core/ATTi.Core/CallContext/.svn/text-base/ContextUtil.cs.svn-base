using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using ATTi.Core.CallContext.Config;
using ATTi.Core.Trace;

namespace ATTi.Core.CallContext
{	
	internal static class ContextUtil
	{
		private static List<IClientSinkListener> _clientListeners;
		private static List<IServerSinkListener> _svrListeners;

		private struct ContextClientHandback
		{
			public IClientSinkListener Listener;
			public object Handback;
			public ContextClientHandback(IClientSinkListener l, object h)
			{
				Listener = l;
				Handback = h;
			}
		}

		private struct ContextServerHandback
		{
			public IServerSinkListener Listener;
			public object Handback;
			public ContextServerHandback(IServerSinkListener l, object h)
			{
				Listener = l;
				Handback = h;
			}
		}

		static ContextUtil()
		{
			ContextUtilConfigSection section = ConfigurationManager.GetSection(ContextUtilConfigSection.ConfigSectionName)
				as ContextUtilConfigSection;
			if (section.ClientListeners.Count > 0)
			{
				_clientListeners = new List<IClientSinkListener>();
				foreach (ClientListenerConfigElement ce in section.ClientListeners)
				{
					if (ce.ListenerTypeResolved == null)
						throw new ConfigurationErrorsException(String.Format("Unable to Resolve Type '{0}' from contextUtil Configuration > Client Listeners", ce.TypeName));
					IClientSinkListener item = Activator.CreateInstance(ce.ListenerTypeResolved) as IClientSinkListener;
					if (item == null)
						throw new ConfigurationErrorsException();
					_clientListeners.Add(item);
				}
			}
			if (section.ServerListeners.Count > 0)
			{
				_svrListeners = new List<IServerSinkListener>();
				foreach (ServerListenerConfigElement ce in section.ServerListeners)
				{
					if (ce.ListenerTypeResolved == null)
						throw new ConfigurationErrorsException(String.Format("Unable to Resolve Type '{0}' from contextUtil Configuration > Server Listeners", ce.TypeName));
					IServerSinkListener item = Activator.CreateInstance(ce.ListenerTypeResolved) as IServerSinkListener;
					if (item == null)
						throw new ConfigurationErrorsException();
					_svrListeners.Add(item);
				}
			}
		}

		internal static object NotifyClientMessage(System.Runtime.Remoting.Messaging.IMessage msg)
		{
			List<ContextClientHandback> result = new List<ContextClientHandback>();
			if (_clientListeners != null)
			{
				LogicalCallContext lcc = msg.Properties["__CallContext"] as LogicalCallContext;

				foreach (IClientSinkListener listener in _clientListeners)
				{
					try
					{
						object handback;
						if (listener.HandleClientMessage(msg, lcc, out handback))
						{
							result.Add(new ContextClientHandback(listener, handback));
						}
					}
					catch (Exception e)
					{
						Traceable.TraceData(typeof(ContextUtil), System.Diagnostics.TraceEventType.Error,
							String.Format(Properties.Resources.Error_ThrownFromClientListener,
							listener.GetType().FullName), e, e.Message, e.StackTrace);
					}
				}
			}
			return result;
		}

		internal static void NotifyClientMessageReturn(object handback, System.Runtime.Remoting.Messaging.IMessage msg)
		{
			List<ContextClientHandback> listeners = handback as List<ContextClientHandback>;

			if (listeners != null)
			{
				LogicalCallContext lcc = msg.Properties["__CallContext"] as LogicalCallContext;

				foreach (ContextClientHandback cuh in listeners)
				{
					try
					{
						cuh.Listener.HandleClientMessageReply(msg, lcc, cuh.Handback);
					}
					catch (Exception e)
					{
						Traceable.TraceData(typeof(ContextUtil), System.Diagnostics.TraceEventType.Error,
							String.Format(Properties.Resources.Error_ThrownFromClientListener,
							cuh.Listener.GetType().FullName), e, e.Message, e.StackTrace);
					}
				}
			}
		}

		internal static object NotifyServerMessage(IMessage msg, ITransportHeaders hdrs)
		{
			List<ContextServerHandback> result = new List<ContextServerHandback>();
			if (_svrListeners != null)
			{
				LogicalCallContext lcc = msg.Properties["__CallContext"] as LogicalCallContext;

				foreach (IServerSinkListener listener in _svrListeners)
				{
					try
					{
						object handback;
						if (listener.HandleServerMessage(msg, hdrs, lcc, out handback))
						{
							result.Add(new ContextServerHandback(listener, handback));
						}
					}
					catch (Exception e)
					{
						Traceable.TraceData(typeof(ContextUtil), System.Diagnostics.TraceEventType.Error,
							string.Format(Properties.Resources.Error_ThrownFromServerListener,
							listener.GetType().FullName), e, e.Message, e.StackTrace);
					}
				}
			}
			return result;
		}

		internal static void NotifyServerMessageResponse(IMessage msg, ITransportHeaders hdrs, object handback)
		{
			List<ContextServerHandback> listeners = handback as List<ContextServerHandback>;

			if (listeners != null)
			{
				LogicalCallContext lcc = msg.Properties["__CallContext"] as LogicalCallContext;

				foreach (ContextServerHandback cuh in listeners)
				{
					try
					{
						cuh.Listener.HandleServerMessageResponse(msg, hdrs, lcc, cuh.Handback);
					}
					catch (Exception e)
					{
						Traceable.TraceData(typeof(ContextUtil), System.Diagnostics.TraceEventType.Error,
							string.Format(Properties.Resources.Error_ThrownFromServerListener,
							cuh.Listener.GetType().FullName), e, e.Message, e.StackTrace);
					}
				}
			}
		}
	}
}
