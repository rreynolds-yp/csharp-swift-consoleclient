﻿using System;
using System.Configuration;
using System.Text;
using System.Threading;
using ATTi.Core;
using ATTi.Core.Factory;
using ATTi.Core.MessageBus.Configuration;
using ATTi.Core.Trace;
using ATTi.TMail.Common;
using ATTi.TMail.Service.Implementation.Agent.Messages;
using Newtonsoft.Json;

namespace ATTi.TMail.Service.Implementation.Agent
{
	//using ATTI.Core.Logging;
	
	public class AgentOutput
	{
		int _sequence = 0;
		Guid _id;
		AmqpConnectionConfigurationElement _config;

		public AgentOutput()
		{
			_id = Guid.NewGuid();

			_config = MessageBusConfigurationSection.Instance.Connections[AgentProtocol.CTMailConnectionName];
			if (_config == null)
				throw new ConfigurationErrorsException(String.Concat("Configuration for message bus connection missing: ",
					AgentProtocol.CTMailConnectionName));
		}

		public T CreateMessage<T>(Action<T> loader) where T : MessageBase
		{
			T m = Factory<T>.CreateInstance(Interlocked.Increment(ref _sequence));
			m.AgentID = _id;
			if (loader != null) loader(m);
			return m;
		}

		public void SendMessage<T>(string exchange, string replyTo, T message) where T : MessageBase
		{

			WriteDebugLine("TMAIL start publish message to MeesageQueue");
			using (var cn = _config.CreateConnection())
			{
				
				using (var model = cn.CreateModel())
				{
					model.TxSelect();
					var p = model.CreateBasicProperties();
					p.ContentEncoding = AgentProtocol.CContentEncoding;
					p.ContentType = AgentProtocol.CContentType;
					p.Type = Convert.ToString(message.Kind);
					if (!String.IsNullOrEmpty(replyTo))
					{
						p.ReplyTo = replyTo;
					}
					string json = JsonConvert.SerializeObject(message);

					//YPMon.Debug <AgentOutput>("TMAIL_SENDMSG", @"TMail Publish message to MessageQ");
					model.BasicPublish(exchange, String.Empty, p, UTF8Encoding.UTF8.GetBytes(json));
					model.TxCommit();
					WriteDebugLine("TMAIL published message to MeesageQueue");              					
				}
                if (cn != null)
                    cn.Close();
            }
		}

		public T CreateAndSendMessage<T>(string exchange, Action<T> loader) where T : MessageBase
		{
			return CreateAndSendMessage(exchange, null, loader);
		}
		public T CreateAndSendMessage<T>(string exchange, string replyTo, Action<T> loader) where T : MessageBase
		{
			T m = CreateMessage<T>(loader);
			SendMessage(exchange, replyTo, m);
			return m;
		}

		void WriteDebugLine(params object[] args)
		{
			YPMon.Debug<AgentOutput>("AgentProtocol.WriteLine", String.Concat(args));
		}
	}

	public class AgentProtocol : IDisposable, ITraceable, IMessageSender
	{
		public static readonly string CTMailConnectionName = "tmail-connection";
		public static readonly string CTMailExchange = "tmail-exchange";
		public static readonly string CTMailDelayed = "tmail-delayed";
		public static readonly string CTMailAccept = "tmail-accept";
		public static readonly string CTMailPaused = "tmail-paused";

		public static readonly string CContentEncoding = UTF8Encoding.UTF8.BodyName;
		public static readonly string CContentType = "application/json";
				
		AmqpConnectionConfigurationElement _config;
		QueueListener _accepter;
		QueueListener _delayed;
		QueueListener _paused;

		int _sequence = 0;
		Guid _id;
		ResponseTrackingLogic _responseTracking;
		
		public AgentProtocol()
		{
			_id = Guid.NewGuid();

			_config = MessageBusConfigurationSection.Instance.Connections[CTMailConnectionName];
			if (_config == null)
				throw new ConfigurationErrorsException(String.Concat("Configuration for message bus connection missing: ", CTMailConnectionName));

			_responseTracking = new ResponseTrackingLogic();
			_responseTracking.Start();
		}

		public Guid ID { get { return _id; } }
		
		public LineWriterDelegate OnWriteLine { get; set; }		
		public InboundMessageDelegate<MailingMessage> OnMailing { get; set; }
		public InboundMessageDelegate<DelayedMailingMessage> OnDelayedMailing { get; set; }
		public InboundMessageDelegate<PausedMailingMessage> OnPausedMailing { get; set; }

		public bool StartAgentLogic(Action<object, string> onStarted)
		{
			Thread.MemoryBarrier();
			var accepter = _accepter;
			Thread.MemoryBarrier();
			if (accepter == null)
			{
                WriteDebugLine("Start Agent Logic");
				accepter = new QueueListener(_config, CTMailAccept, CTMailAccept, null);
				if (Interlocked.CompareExchange(ref _accepter, accepter, null) == null)
				{
					accepter.OnListeningStarted += new ListenerEventDelegate((s, t) =>
						{
							StartDelayedLogic(onStarted);
						});
					accepter.OnWriteLine += OnWriteLine;
					accepter.OnMailing += PerformOnMailing;

					accepter.Start();

					WriteDebugLine("Started Agent Logic: Listening new incoming email from queue");
					return true;
				}
				
			}
			//this.LogInfo("Error listening to MQ");
            WriteDebugLine("Error listening to MQ");
			return false;			
		}

		private void StartDelayedLogic(Action<object, string> onStarted)
		{
			Thread.MemoryBarrier();
			var delayed = _delayed;
			Thread.MemoryBarrier();
			if (delayed == null)
			{
				WriteDebugLine("Starting Delayed Logic");
				delayed = new QueueListener(_config, CTMailDelayed, CTMailDelayed, null);
				if (Interlocked.CompareExchange(ref _delayed, delayed, null) == null)
				{
					if (onStarted != null)
					{						
						delayed.OnListeningStarted += new ListenerEventDelegate(onStarted);
					}
					delayed.OnWriteLine += OnWriteLine;
					delayed.OnDelayedMailing += PerformOnDelayedMailing;

					delayed.Start();
					WriteDebugLine("Started Delayed Logic: Listening for old mail from queue");
				}
			}
		}

		private void StartPausedLogic(Action<object, string> onStarted)
		{
			Thread.MemoryBarrier();
			var paused = _paused;
			Thread.MemoryBarrier();
			if (paused == null)
			{
				WriteDebugLine("Starting Paused Logic");
				paused = new QueueListener(_config, CTMailPaused, CTMailPaused, null);
				if (Interlocked.CompareExchange(ref _paused, paused, null) == null)
				{
					if (onStarted != null)
					{
						paused.OnListeningStarted += new ListenerEventDelegate(onStarted);
					}
					paused.OnWriteLine += OnWriteLine;
					paused.OnPausedMailing += PerformOnPausedMailing;
					paused.Start();
					WriteDebugLine("Started Paused Logic: Listening for paused mail from queue");
				}
			}
		}


		public void StopAgentLogic()
		{
			Util.Dispose(ref _accepter);
			Util.Dispose(ref _delayed);
			Util.Dispose(ref _paused);
		}
				
		void PerformOnMailing(object sender, InboundMessageArgs<MailingMessage> args)
		{
			var m = args.Message;
			var t = _responseTracking.Get(m.AgentID, m.Sequence);
			if (t != null) t.Ack(m);
			var dlg = OnMailing;
			if (dlg != null)
			{				
				dlg(this, args);
			}
		}				
		void PerformOnDelayedMailing(object sender, InboundMessageArgs<DelayedMailingMessage> args)
		{
			var msg = args.Message;
			var t = _responseTracking.Get(msg.AgentID, msg.Sequence);
			if (t != null) t.Ack(msg);
			var dlg = OnDelayedMailing;
			if (dlg != null)
			{
				dlg(this, args);
			}
		}
		void PerformOnPausedMailing(object sender, InboundMessageArgs<PausedMailingMessage> args)
		{
			var msg = args.Message;
			var t = _responseTracking.Get(msg.AgentID, msg.Sequence);
			if (t != null) t.Ack(msg);
			var dlg = OnPausedMailing;
			if (dlg != null)
			{
				dlg(this, args);
			}
		}
		void PerformOnReturned(object sender, ReturnedMessageArgs args)
		{
			var p = args.Message;
			var t = _responseTracking.Get(p.AgentID, p.Sequence);
			if (t != null) t.Returned();
		}
		
		void WriteDebugLine(params object[] args)
		{
			var dlg = OnWriteLine;
			YPMon.Debug<AgentProtocol>("AgentProtocol.WriteLine", String.Concat(args));
			if (dlg != null)
				dlg(this, args);
		}		
				
		#region IMessageSender Members
		
		public T CreateMessage<T>(Action<T> loader) where T : MessageBase
		{
			T m = Factory<T>.CreateInstance(Interlocked.Increment(ref _sequence));
			m.AgentID = _id;
			if (loader != null) loader(m);
			return m;
		}
		public void SendMessage<T>(string exchange, T message) where T : MessageBase
		{
			SendMessage(exchange, message);
		}		
		public void SendMessage<T>(string exchange, string replyTo, T message) where T : MessageBase
		{
			
            WriteDebugLine("AgentProtocol About to send message to MQ");
			using (var cn = _config.CreateConnection())
			{
				
                WriteDebugLine("AgentProtocol Send Messaing to MQ");
				using (var model = cn.CreateModel())
				{
					model.TxSelect();
					var p = model.CreateBasicProperties();
					p.ContentEncoding = AgentProtocol.CContentEncoding;
					p.ContentType = AgentProtocol.CContentType;
					p.Type = Convert.ToString(message.Kind);
					if (!String.IsNullOrEmpty(replyTo))
					{
						p.ReplyTo = replyTo;
					}
					string json = JsonConvert.SerializeObject(message);
					//model.BasicPublish(CTMailExchange, topic, p, UTF8Encoding.UTF8.GetBytes(json));
			
                    WriteDebugLine("AgentProtocol publish to MQ");
					model.BasicPublish(exchange, String.Empty, p, UTF8Encoding.UTF8.GetBytes(json));
					model.TxCommit();
					//this.LogInfo("AgentProtocol publish to MQ -Done!");
                    WriteDebugLine("AgentProtocol publish to MQ -Done!");
				}
			}
		}

		public T CreateAndSendMessage<T>(string exchange, Action<T> loader) where T : MessageBase
		{
			T m = CreateMessage<T>(loader);
			SendMessage(exchange, null, m);
			return m;
		}

		public T CreateAndSendMessage<T>(string exchange, string replyTo, Action<T> loader) where T : MessageBase
		{
			T m = CreateMessage<T>(loader);
			SendMessage(exchange, replyTo, m);
			return m;
		}

		public T CreateAndSendMessage<T>(string exchange, Action<T> loader, TimeSpan timeout,
			 Action<ResponseTracking, bool, bool> completion) where T : MessageBase
		{
			T m = CreateMessage<T>(loader);
			_responseTracking.Add(new ResponseTracking(m, timeout), completion);
			SendMessage(exchange, m);
			return m;
		}

		public T CreateAndSendMessage<T>(string exchange, string replyTo, Action<T> loader, TimeSpan timeout,
			 Action<ResponseTracking, bool, bool> completion) where T : MessageBase
		{
			T m = CreateMessage<T>(loader);
			_responseTracking.Add(new ResponseTracking(m, timeout), completion);
			SendMessage(exchange, replyTo, m);
			return m;
		}

		#endregion

		#region IDisposable Members
		~AgentProtocol()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool dispose)
		{
			Util.Dispose(ref _accepter);
			Util.Dispose(ref _delayed);
			Util.Dispose(ref _paused);
		}
		#endregion				
	}

}
