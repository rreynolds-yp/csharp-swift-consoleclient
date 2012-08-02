using System;
using System.Text;
using System.Threading;
using ATTi.Core;
using ATTi.Core.Contracts;
using ATTi.Core.MessageBus.Configuration;
using ATTi.TMail.Common;
using ATTi.TMail.Service.Implementation.Agent.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Org.Lwes;
using ATTi.Core.Trace;
using System.Diagnostics;

namespace ATTi.TMail.Service.Implementation.Agent
{
	public enum ListenerState
	{
		Idle = 0,
		Starting,
		BrokerUnreachable,
		ConnectionInterrupted,
		Suspending,
		Suspended,
		Resetting,
		Started,
		Stopping,
		Stopped,
		Disposed,
	}
	internal class QueueListener : ITraceable, IDisposable
	{
		const int CDefaultBackoffMilliseconds = 1200;
		const int CMaxBackoffMilliseconds = 120000;
		const int CBackoffMillisecondsRangeMultiplier = 5;

		AmqpConnectionConfigurationElement _config;
		ATTi.Core.Status<ListenerState> _state;
		IConnection _cn;
		Thread _th;
		
		internal QueueListener(AmqpConnectionConfigurationElement config, 
			string exchange,
			string queue,
			string topic)
		{
			Require.IsNotNull("config", config);
			Require.IsNotNull("exchange", exchange);
			Require.IsNotNull("queue", queue);
			_config = config;
			this.Exchange = exchange;
			this.Queue = queue;
			this.Topic = topic ?? String.Empty;
		}

		public string Exchange { get; private set; }
		public string Queue { get; private set; }
		public string Topic { get; private set; }
		public event LineWriterDelegate OnWriteLine;
		public event ListenerEventDelegate OnBrokerUnreachable;
		public event ListenerEventDelegate OnConnectionInterrupted;
		public event ListenerEventDelegate OnListeningStarted;
		public event ListenerEventDelegate OnListeningStopped;
		public event InboundMessageDelegate<MailingMessage> OnMailing;
		public event InboundMessageDelegate<DelayedMailingMessage> OnDelayedMailing;
		public event InboundMessageDelegate<PausedMailingMessage> OnPausedMailing;
		public event ReturnedMessageDelegate OnReturned;

		public void Start()
		{
			if (_state.TryTransition(ListenerState.Idle, ListenerState.Idle, ListenerState.Stopped))
			{
				_th = new Thread(RunQueueLogicByConsume);
				_th.IsBackground = true;
				_th.Start();
			}
		}

		public bool Stop()
		{
			Thread.MemoryBarrier();
			var th = _th;
			Thread.MemoryBarrier();
			if (th != null && Interlocked.CompareExchange(ref _th, null, th) == th)
			{
				var cn = Interlocked.Exchange(ref _cn, null);
				if (cn != null) 
				{
					var timeout = DateTime.Now.Add(TimeSpan.FromMilliseconds(500));
					_state.TryTransition(ListenerState.Stopping, ListenerState.Started);
					_state.TrySpinWaitForState(ListenerState.Stopped, s =>
					{
						return timeout < DateTime.Now;
					});
					cn.Dispose();					
					th.Join(TimeSpan.FromSeconds(2));
				}
			}
			return _state.CurrentState == ListenerState.Stopped;
		}

		public bool Suspend()
		{
			return _state.TryTransition(ListenerState.Suspending, ListenerState.Started);			
		}

		public bool Resume()
		{
			return _state.TryTransition(ListenerState.Starting, ListenerState.Suspended);
		}

		public ListenerState Status
		{
			get { return _state.CurrentState; }
		}

		public bool SpinWaitForState(ListenerState desired, Func<ListenerState, bool> action)
		{
			return _state.TrySpinWaitForState(desired, action);
		}
		void WriteLine(params object[] args)
		{
			WriteLine(true, args);
		}		
		void WriteLine(bool writeToTraceOutput, params object[] args)
		{
			var dlg = OnWriteLine;
			YPMon.Debug <QueueListener>("QueueListner ", String.Concat(args));
			if (dlg != null)
				dlg(this, args);
		}
		void BrokerUnreachable()
		{
			var dlg = OnBrokerUnreachable;
			if (dlg != null)
				dlg(this, Topic);
		}
		void ConnectionInterrupted()
		{
			var dlg = OnConnectionInterrupted;
			if (dlg != null)
				dlg(this, Topic);
		}
		void ListeningStarted()
		{
			var dlg = OnListeningStarted;
			if (dlg != null)
				dlg(this, Topic);
		}
		void ListeningStopped()
		{
			var dlg = OnListeningStopped;
			if (dlg != null)
				dlg(this, Topic);
		}
		void Mailing(MailingMessage m)
		{
			var dlg = OnMailing;
			if (dlg != null)
			{
				var args = new InboundMessageArgs<MailingMessage> { Message = m, Topic = this.Topic };
				dlg(this, args);
			}
		}
		void DelayedMailing(DelayedMailingMessage m)
		{
			var dlg = OnDelayedMailing;
			if (dlg != null)
			{
				var args = new InboundMessageArgs<DelayedMailingMessage> { Message = m, Topic = this.Topic };
				dlg(this, args);
			}
		}

		void PausedMailing(PausedMailingMessage m)
		{
			var dlg = OnPausedMailing;
			if (dlg != null)
			{
				var args = new InboundMessageArgs<PausedMailingMessage> { Message = m, Topic = this.Topic };
				dlg(this, args);
			}
		}

		void Returned(IBasicProperties p, MessageBase m)
		{
			var dlg = OnReturned;
			if (dlg != null)
			{
				var args = new ReturnedMessageArgs
				{
					Queue = this.Queue,
					ReplyTo = p.ReplyTo,
					Message = m,
					Topic = this.Topic
				};
				dlg(this, args);
			}
		}		
				
		void RunQueueLogicByConsume(object o)
		{
			var exchangeType = "direct";
			var topicName = String.Concat(exchangeType, ":", Exchange, (String.IsNullOrEmpty(Topic)) ? "" : "/" + Topic);
			if (_state.TryTransition(ListenerState.Starting, ListenerState.Idle, ListenerState.Stopped))
			{
				try
				{
					while (_state.IsLessThan(ListenerState.Started))
					{
						var backoff_milliseconds_seed = CDefaultBackoffMilliseconds;
						var backoff_completely = false;
						var rand = new Random(Environment.TickCount);
						try
						{
							while (_state.IsLessThan(ListenerState.Stopping) && !backoff_completely)
							{
								Thread.MemoryBarrier();
								var cn = _cn = _config.CreateConnection();
								Thread.MemoryBarrier();
								try
								{
									using (var model = cn.CreateModel())
									{
										model.ExchangeDeclare(Exchange, exchangeType, false, true, false, false, false, null);
										var tag = model.QueueDeclare(Exchange, false, true, false, false, false, null);
										model.QueueBind(Exchange, Queue, Topic, false, null);
										backoff_milliseconds_seed = CDefaultBackoffMilliseconds;
										if (_state.TryTransition(ListenerState.Started, ListenerState.Starting))
										{
											WriteLine("Listening for messages on ", topicName);
											ThreadPool.QueueUserWorkItem(new WaitCallback(unused => { ListeningStarted(); }));
											while (_state.IsLessThan(ListenerState.Stopping))
											{
												var s = _state.CurrentState;
												if (s == ListenerState.Suspending && _state.TryTransition(ListenerState.Suspended, s))
												{
													// Stay suspended until siganled to start again (ListenerState.Starting)
													if (!_state.TrySpinWaitForState(ListenerState.Starting, c =>
													{

														Thread.Sleep(500);
														return c < ListenerState.Stopping;
													}))
													{
														// Signaled to stop (ListenerState.Stopping), break out of the read loop.
														break;
													}
												}
												var r = model.BasicGet(Queue, true);
												if (r != null)
												{
													var m = CreateMessageFromEvent(r.BasicProperties, r.Body);
													DispatchMessage(r.BasicProperties, m);
													model.BasicAck(r.DeliveryTag, r.Redelivered);
												}
											}
										}
									}
								}
								catch (OperationInterruptedException)
								{
									_state.SetState(ListenerState.ConnectionInterrupted);
									var m = String.Concat("Connection interrupted on ", topicName);
									this.TraceData(TraceEventType.Error, m);
									YPMon.Warn("ConnectionInterrupted", m);
									WriteLine(false, m);
									ConnectionInterrupted();
								}
								catch (BrokerUnreachableException)
								{
									_state.SetState(ListenerState.BrokerUnreachable);
									var m = String.Concat("Broker unreachable on ", topicName);
									this.TraceData(TraceEventType.Error, m);
									YPMon.Warn("BrokerUnreachable", m);
									WriteLine(false, m);
									BrokerUnreachable();
								}
								finally
								{
									if (cn != null)
									{
										try { cn.Dispose(); }
										catch { /* fall-through, forceable close causes early dispose and our call results in exception */ }
										Interlocked.CompareExchange(ref _cn, null, cn);
										ThreadPool.QueueUserWorkItem(new WaitCallback(unused => { ListeningStopped(); }));
									}
								}
								ResetStateAndRetryAfter(ref backoff_milliseconds_seed, rand);
							}
						}
						catch (BrokerUnreachableException bexp) 
						{
							_state.SetState(ListenerState.BrokerUnreachable);
							var m = String.Concat("Broker unreachable on Agent Start", topicName, "On exception ",bexp.Message);
							this.TraceData(TraceEventType.Error, m);
							YPMon.Warn("BrokerUnreachable", m);
							WriteLine(false, m);
							BrokerUnreachable();
							ResetStateAndRetryAfter(ref backoff_milliseconds_seed, rand);
						}
					}
				}
				finally
				{
					_state.TryTransition(ListenerState.Stopped, ListenerState.Stopping);
					WriteLine("Done listening on ", topicName, " with state: ", _state.CurrentState);
				}
			}
		}

		private void ResetStateAndRetryAfter(ref int backoff_milliseconds_seed, Random rand)
		{
			var state = _state.CurrentState;
			if (state < ListenerState.Stopping && _state.TryTransition(ListenerState.Resetting, state))
			{
				Thread.Sleep(rand.Next(backoff_milliseconds_seed, backoff_milliseconds_seed * CBackoffMillisecondsRangeMultiplier));
				if (backoff_milliseconds_seed < CMaxBackoffMilliseconds)
					backoff_milliseconds_seed += backoff_milliseconds_seed;
			}
			//return backoff_milliseconds_seed;
		}
		
		MessageBase CreateMessageFromEvent(IBasicProperties p, byte[] body)
		{
			MessageBase m = null;			
			if (p.ContentType == AgentProtocol.CContentType)
			{
				var enc = (String.IsNullOrEmpty(p.ContentEncoding)) ? UTF8Encoding.UTF8 : Encoding.GetEncoding(p.ContentEncoding);
				var json = new String(enc.GetChars(body));

				AgentMessageKind kind = (AgentMessageKind)Enum.Parse(typeof(AgentMessageKind), p.Type);
				switch (kind)
				{
					case AgentMessageKind.Mailing:
						m = JsonConvert.DeserializeObject<MailingMessage>(json);
						break;
					case AgentMessageKind.MailingDelayed:
						m = JsonConvert.DeserializeObject<DelayedMailingMessage>(json);
						break;
					case AgentMessageKind.MailingPaused:
						m = JsonConvert.DeserializeObject<PausedMailingMessage>(json);
						break;
				}
			}
			return m;
		}
		void DispatchMessage(IBasicProperties p, MessageBase m)
		{
			switch (m.Kind)
			{
				case AgentMessageKind.None:
					break;
				case AgentMessageKind.Mailing:
					Mailing(m as MailingMessage);
					break;
				case AgentMessageKind.MailingDelayed:
					DelayedMailing(m as DelayedMailingMessage);
					break;
				case AgentMessageKind.MailingPaused:
					PausedMailing(m as PausedMailingMessage);
					break;
				default:
					break;
			}
		}

		#region IDisposable Members
		~QueueListener()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing)
		{
			if (_state.IsLessThan(ListenerState.Disposed))
			{
				Stop();
				_state.SetState(ListenerState.Disposed);
			}
		}
		#endregion
	}
}
