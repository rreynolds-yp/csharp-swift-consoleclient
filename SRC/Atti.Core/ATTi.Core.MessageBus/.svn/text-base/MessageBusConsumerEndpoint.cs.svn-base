using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using ATTi.Core.Factory;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;
using ATTi.Core.Trace;
using ATTi.Core.Contracts;
using ATTi.Core.MessageBus.Configuration;
using System.Configuration;

namespace ATTi.Core.MessageBus
{
	internal class MessageBusConsumerEndpoint : IMessageBusConsumerEndpoint, ITraceable
	{		
		abstract class TransformationHandlerBase
		{
			public Type TargetType { get; protected set; }
			public abstract MessageReturnAction RawHandler(MessageProperties properties, MessageDeliveryInfo routing, byte[] bytes);
		}
		class TransformationHandler<T> : TransformationHandlerBase
		{
			public TransformationHandler()
			{
				this.TargetType = typeof(T);
			}

			public Func<MessageProperties, byte[], T> Transform { get; set; }
			public Func<MessageProperties, MessageDeliveryInfo, T, MessageReturnAction> Handler { get; set; }

			public override MessageReturnAction RawHandler(MessageProperties properties, MessageDeliveryInfo routing, byte[] bytes)
			{
				return Handler(properties, routing, Transform(properties, bytes));
			}
		}
		MessageBusConnection _connection;
		Status<ModelStatus> _status = new Status<ModelStatus>();
		QueueingBasicConsumer _consumer;
		Thread _backgroundThread;
		IModel _model;
		bool _usingTransactions;
		string _queueName;
		string _actualQueueName;
		string _routingKey;
		Predicate<MessageProperties> _match;
		TransformationHandlerBase _transformationHandler;
		Func<MessageProperties, MessageDeliveryInfo, byte[], MessageReturnAction> _rawHandler;

		internal MessageBusConsumerEndpoint(MessageBusConnection connection, string queueName, string routingKey)
		{
			Require.IsNotNull("connection", connection);
			Require.IsNotEmpty("queueName", queueName);
			Require.IsNotNull("routingKey", routingKey);

			_queueName = queueName;
			_routingKey = routingKey;
			_connection = connection;
			_model = OpenAndConnectModel(() => { _status.SetState(ModelStatus.Open); });

			_match = new Predicate<MessageProperties>((p) => { return true; });
			_rawHandler = new Func<MessageProperties, MessageDeliveryInfo, byte[], MessageReturnAction>((p, r, b) =>
			{
				return new MessageReturnAction();
			});
		}

		private IModel OpenAndConnectModel(Action actionOnOpen)
		{
			var queue = MessageBusConfigurationSection.Instance.Queues[_queueName];
			if (queue == null)
				throw new ConfigurationErrorsException(String.Concat("Messagebus queue not configured: ", _queueName, "."));

			_model = _connection.ReliableCreateModel(queue, _routingKey);
			_actualQueueName = queue.ActualQueueName;
			_model.CallbackException += new CallbackExceptionEventHandler(_model_CallbackException);
			_model.ModelShutdown += new ModelShutdownEventHandler(_model_ModelShutdown);
			actionOnOpen();
			return _model;
		}

		void _model_ModelShutdown(IModel model, ShutdownEventArgs reason)
		{
			if (reason.Initiator == ShutdownInitiator.Peer 
				&& _status.IsLessThan(ModelStatus.Stopping))
			{
				var status = _status.CurrentState & ModelStatus.Started;
				_status.SetState(status & ModelStatus.Reopening);
				try
				{
					_model = OpenAndConnectModel(() =>
						{
							_status.SetState(ModelStatus.Open);
							// If it was started previously then restart it...
							if ((status & ModelStatus.Started) == ModelStatus.Started)
							{
								Startup();
							}
						});
				}
				catch
				{
					/* fall through and signal the shutdown */
				}
			}
			_status.SetState(ModelStatus.Shutdown);
		}

		void _model_CallbackException(object sender, CallbackExceptionEventArgs e)
		{
		}
				
		#region IMessageBusConsumerEndpoint Members

		public IMessageBusConsumerEndpoint MatchMessagesWithProperties(Predicate<MessageProperties> predicate)
		{
			Require.IsNotNull("predicate", predicate);
			Invariant.ExpectOneOf("Status", _status.CurrentState, ModelStatus.Open);
			_match = predicate;
			return this;
		}

		public IMessageBusConsumerEndpoint TransformMessageBody<T>(Func<MessageProperties, byte[], T> transform)
		{
			Require.IsNotNull("transform", transform);
			Invariant.ExpectOneOf("Status", _status.CurrentState, ModelStatus.Open);

			if (_transformationHandler == null)
			{
				CreateTransformHandler<T>(transform);
			}
			else
			{
				Invariant.IsAssignableFrom(_transformationHandler.TargetType, "typeof(T)", typeof(T));
				AddTransformToHandler<T>(transform);
			}
			return this;
		}

		private void AddTransformToHandler<T>(Func<MessageProperties, byte[], T> transform)
		{
			var handler = (TransformationHandler<T>)_transformationHandler;
			handler.Transform = transform;
		}

		private void CreateTransformHandler<T>(Func<MessageProperties, byte[], T> transform)
		{
			var handler = new TransformationHandler<T>();
			handler.Transform = transform;
			_transformationHandler = handler;
			_rawHandler = handler.RawHandler;
		}

		public IMessageBusConsumerEndpoint HandleMessages<T>(Func<MessageProperties, MessageDeliveryInfo, T, MessageReturnAction> handler)
		{
			Require.IsNotNull("handler", handler);
			Invariant.ExpectOneOf("Status", _status.CurrentState, ModelStatus.Open);

			if (_transformationHandler == null)
			{
				CreateTransformHandler<T>(handler);
			}
			else
			{
				Invariant.IsAssignableFrom(_transformationHandler.TargetType, "typeof(T)", typeof(T));
				AddTransformToHandler<T>(handler);
			}
			return this;
		}

		private void AddTransformToHandler<T>(Func<MessageProperties, MessageDeliveryInfo, T, MessageReturnAction> handler)
		{
			var h = (TransformationHandler<T>)_transformationHandler;
			h.Handler = handler;
		}

		private void CreateTransformHandler<T>(Func<MessageProperties, MessageDeliveryInfo, T, MessageReturnAction> handler)
		{
			var h = new TransformationHandler<T>();
			h.Handler = handler;
			_transformationHandler = h;
			_rawHandler = h.RawHandler;
		}

		public IMessageBusConsumerEndpoint HandleRawMessages(Func<MessageProperties, MessageDeliveryInfo, byte[], MessageReturnAction> handler)
		{
			throw new NotImplementedException();
		}

		public IMessageBusConsumerEndpoint UseTransactions()
		{
			_model.TxSelect();
			_usingTransactions = true;
			return this;
		}

		public void Startup()
		{
			Invariant.ExpectOneOf("Status", _status.CurrentState, ModelStatus.Open);

			_status.TryTransition(ModelStatus.Starting, ModelStatus.Open, PerformStart);
		}

		public void Shutdown()
		{
			if (_status.SetStateIfLessThan(ModelStatus.Stopping, ModelStatus.Stopping))
			{
				_model.Close();
				_status.TrySpinWaitForState(ModelStatus.Shutdown, s => {
					if (s < ModelStatus.Disposed)
					{
						Thread.Sleep(200);
						return true;
					}
					return false;										
				});
			}
		}

		public void WaitForShutdown()
		{
			_status.SpinWaitForState(ModelStatus.Shutdown, () => Thread.Sleep(200));
		}

		public bool WaitForShutdown(TimeSpan timeout)
		{
			long timeoutTicks = DateTime.UtcNow.Add(timeout).Ticks;
			while (_status.CurrentState != ModelStatus.Shutdown)
			{
				var remainingTicks = timeoutTicks - DateTime.UtcNow.Ticks;
				if (remainingTicks > 0)
				{ // pulse at least every 100 milliseconds...
					Thread.Sleep(Math.Min(TimeSpan.FromTicks(remainingTicks).Milliseconds, 100));
				}
				else return false;
			}
			return true;
		}

		private void PerformStart()
		{
			_backgroundThread = new Thread(new ThreadStart(() =>
			{
				_status.SetState(ModelStatus.Started);
				_consumer = new QueueingBasicConsumer(_model);
				_model.BasicConsume(_actualQueueName, false, null, _consumer);
				var actions = default(MessageReturnAction);

				while ((actions & MessageReturnAction.Shutdown) != MessageReturnAction.Shutdown)
				{
					BasicDeliverEventArgs evt;
					try
					{
						evt = (BasicDeliverEventArgs)_consumer.Queue.Dequeue();
						try
						{
							var props = Factory<MessageProperties>.Copier<MessageProperties>.CopyLoose(evt.BasicProperties);
							var routing = new MessageDeliveryInfo
							{
								Exchange = evt.Exchange,
								Queue = _actualQueueName,
								RoutingKey = evt.RoutingKey,
								Redelivered = evt.Redelivered								
							};

							actions = _rawHandler(props, routing, evt.Body);
						}
						catch (Exception e)
						{
							var props = evt.BasicProperties;
							var body = evt.Body;

							this.TraceData(TraceEventType.Error,
								String.Concat("Uncaught exception processing request: Properties=", props.ToString(),
								Environment.NewLine, "\tBody (octets)=", Util.ConvertBytesToOctets(body, 0, body.Length)), e);
						}

						// Ack if instructed to do so by the message handler...
						if ((actions & MessageReturnAction.Ack) == MessageReturnAction.Ack)
						{							
							// Commit the transaction if instructed to do so by the message handler...
							if (_usingTransactions)
							{
								if ((actions & MessageReturnAction.CommitTx) == MessageReturnAction.CommitTx)
								{
									_model.BasicAck(evt.DeliveryTag, false);
									_model.TxCommit();
								}
								else
								{
									_model.TxRollback();
								}
							}
							else
							{
								_model.BasicAck(evt.DeliveryTag, false);
							}
						}
					}
					catch (EndOfStreamException)
					{
						break;
					}
				}
				_status.SetStateIfLessThan(ModelStatus.Stopping, ModelStatus.Stopping);
			}));
			_backgroundThread.Start();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			try
			{
				Shutdown();
				Util.Dispose(ref _model);
				_status.SetStateIfLessThan(ModelStatus.Disposed, ModelStatus.Disposed);
			}
			catch (Exception e)
			{
				this.TraceData(TraceEventType.Error, "Error during dispose method", e.FormatForLogging());
			}
		}

		#endregion

	}
	
}
