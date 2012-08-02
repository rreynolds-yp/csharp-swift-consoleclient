using System;
using System.Linq;
using System.Diagnostics;
using ATTi.Core.Contracts;
using ATTi.Core.MessageBus.Configuration;
using ATTi.Core.Trace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Configuration;

namespace ATTi.Core.MessageBus
{
	internal class MessageBusConnection : IMessageBusConnection, ITraceable
	{		
		Status<MessageBusConnectionStatus> _status = new Status<MessageBusConnectionStatus>();
		IConnection _connection;
		AmqpConnectionConfigurationElement _configElm;
		
		struct ShutdownCallback
		{
			public ShutdownCallback(Action<IMessageBusConnection, object> action, object handback)
			{
				this.Action = action;
				this.Handback = handback;
			}
			public Object Handback;
			public Action<IMessageBusConnection, object> Action;
		}
		object _shutdownCallbacks = new ShutdownCallback[0];

		internal MessageBusConnection(string connectionName)
		{
			Require.IsNotEmpty("connectionName", connectionName);

			_configElm = MessageBusConfigurationSection.Instance.Connections[connectionName];
			if (_configElm == null)
				throw new ConfigurationErrorsException(String.Concat("Messagebus connection not configured: ", connectionName, "."));
			
			_connection = _configElm.CreateConnection();
			_connection.ConnectionShutdown += new ConnectionShutdownEventHandler(_connection_ConnectionShutdown);
			_connection.CallbackException += new CallbackExceptionEventHandler(_connection_CallbackException);
		}

		public MessageBusConnectionStatus Status { get { return _status.CurrentState; } }

		void _connection_CallbackException(object sender, CallbackExceptionEventArgs e)
		{
		}

		void _connection_ConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
		{			
			_status.SetState(MessageBusConnectionStatus.Shutdown);
			var callbacks = (ShutdownCallback[])Thread.VolatileRead(ref _shutdownCallbacks);
			foreach (var c in callbacks)
			{
				c.Action(this, c.Handback);
			}
		}

		#region IMessageBusConnection Members

		public IMessageBusConsumerEndpoint OpenQueueAsConsumer(string queueName, string routingKey)
		{
			Require.IsNotEmpty("queueName", queueName);
			Invariant.AssertState(_status.IsLessThan(MessageBusConnectionStatus.Shutdown), "connection must be open");

			return new MessageBusConsumerEndpoint(this, queueName, routingKey);			
		}

		public IMessageBusProducerEndpoint OpenExchangeAsProducer(string exchangeName, string routingKey)
		{
			Require.IsNotEmpty("exchangeName", exchangeName);
			Invariant.AssertState(_status.IsLessThan(MessageBusConnectionStatus.Shutdown), "connection must be open");

			return new MessageBusProducerEndpoint(this, exchangeName, routingKey);
		}

		public void CallbackOnShutdown(Action<IMessageBusConnection, object> shutdownHandler, object handback)
		{
			var adding = new ShutdownCallback[] { new ShutdownCallback(shutdownHandler, handback) };
			while (true)
			{
				var current = (ShutdownCallback[])Thread.VolatileRead(ref _shutdownCallbacks);
				var result = current.Union(adding).ToArray();
				if (Interlocked.CompareExchange(ref _shutdownCallbacks, result, current) == current)
					break;
			}
		}

		#endregion

		#region IDisposable Members

		~MessageBusConnection()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			try
			{
				Util.Dispose(ref _connection);
				_status.SetStateIfLessThan(MessageBusConnectionStatus.Disposed, MessageBusConnectionStatus.Disposed);
			}
			catch (Exception e)
			{
				this.TraceData(TraceEventType.Error, "Error during dispose method", e.FormatForLogging());
			}
		}

		#endregion

		internal IModel ReliableCreateModel(AmqpQueueConfigurationElement queue, string routingKey)
		{
			var model = _connection.CreateModel();
			try
			{
				if (!String.IsNullOrEmpty(queue.ExchangeName))
				{
					var exchange = MessageBusConfigurationSection.Instance.Exchanges[queue.ExchangeName];
					if (exchange == null)
						throw new ConfigurationErrorsException(String.Concat("Messagebus exchange not configured: ", queue.ExchangeName, "."));
					exchange.DeclareExchange(model);
				}
				queue.DeclareQueue(model);
				queue.BindQueue(model, routingKey, null);
			}
			catch
			{
				model.Dispose();
				throw;
			}
			return model;
		}

		internal IModel ReliableCreateModel(AmqpExchangeConfigurationElement exchange)
		{
			var model = _connection.CreateModel();
			try
			{
				exchange.DeclareExchange(model);				
			}
			catch
			{
				model.Dispose();
				throw;
			}
			return model;
		}
	}	
}
