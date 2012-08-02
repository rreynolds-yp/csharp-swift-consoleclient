using System;
using System.Diagnostics;
using ATTi.Core.Contracts;
using ATTi.Core.Trace;
using ATTi.Core.Reflection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using ATTi.Core.Factory;
using ATTi.Core.MessageBus.Configuration;
using System.Configuration;

namespace ATTi.Core.MessageBus
{
	public interface IMessageBusProducerEndpoint: IDisposable, ITraceable
	{
		IMessageBusProducerEndpoint TransformProgrammaticType<T>(Func<MessageProperties, T, byte[]> transform);
		IMessageBusProducerEndpoint PublishMessage<T>(T message, string routingKey);
		IMessageBusProducerEndpoint PublishMessage<T>(T message);
		IMessageBusProducerEndpoint PublishRawMessage(byte[] messageBytes);
		IMessageBusProducerEndpoint UseTransactions();
	}

	internal class MessageBusProducerEndpoint : IMessageBusProducerEndpoint
	{
		abstract class TransformationHandlerBase
		{
			public Type TargetType { get; protected set; }			
		}
		class TransformationHandler<T> : TransformationHandlerBase
		{
			public TransformationHandler()
			{
				this.TargetType = typeof(T);
			}

			public Func<MessageProperties, T, byte[]> Transform { get; set; }
		}
		MessageBusConnection _connection;
		IModel _model;
		string _routingKey;
		bool _usingTransactions = false;
		string _exchangeName;
		Status<ModelStatus> _status = new Status<ModelStatus>(ModelStatus.Unknown);
		Dictionary<Type, TransformationHandlerBase> _transforms = new Dictionary<Type, TransformationHandlerBase>();

		public MessageBusProducerEndpoint(MessageBusConnection connection, string exchangeName, string routingKey)
		{
			Require.IsNotNull("connection", connection);
			Require.IsNotEmpty("exchangeName", exchangeName);

			_routingKey = routingKey;
			_connection = connection;
			_exchangeName = exchangeName;

			_model = OpenAndConnectModel(() => { _status.SetState(ModelStatus.Open); });

			_model.BasicReturn += new BasicReturnEventHandler(_model_BasicReturn);
			_model.CallbackException += new CallbackExceptionEventHandler(_model_CallbackException);
			_model.ModelShutdown += new ModelShutdownEventHandler(_model_ModelShutdown);
		}

		private IModel OpenAndConnectModel(Action actionOnOpen)
		{
			var exchange = MessageBusConfigurationSection.Instance.Exchanges[_exchangeName];
			if (exchange == null)
				throw new ConfigurationErrorsException(String.Concat("Messagebus exchange not configured: ", _exchangeName, "."));

			_model = _connection.ReliableCreateModel(exchange);			
			_model.CallbackException += new CallbackExceptionEventHandler(_model_CallbackException);
			_model.ModelShutdown += new ModelShutdownEventHandler(_model_ModelShutdown);
			actionOnOpen();
			return _model;
		}

		void _model_ModelShutdown(IModel model, ShutdownEventArgs reason)
		{
			_status.SetState(ModelStatus.Shutdown);
		}

		void _model_CallbackException(object sender, CallbackExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		void _model_BasicReturn(IModel model, BasicReturnEventArgs args)
		{	
		
		}

		#region IMessageBusProducerEndpoint Members

		public IMessageBusProducerEndpoint TransformProgrammaticType<T>(Func<MessageProperties, T, byte[]> transform)
		{
			Require.IsNotNull("transform", transform);
			Invariant.ExpectOneOf("Status", _status.CurrentState, ModelStatus.Open);

			TransformationHandlerBase handler;
			if (_transforms.TryGetValue(typeof(T), out handler))
			{
				((TransformationHandler<T>)handler).Transform = transform;
			}
			else
			{
				_transforms.Add(typeof(T), new TransformationHandler<T> { Transform = transform });
			}
			return this;
		}

		public IMessageBusProducerEndpoint PublishMessage<T>(T message)
		{
			return PublishMessage(message, _routingKey);
		}

		public IMessageBusProducerEndpoint PublishMessage<T>(T message, string routingKey)
		{
			var localRoutingKey = routingKey ?? _routingKey;

			TransformationHandlerBase handler;
			Invariant.AssertState(_transforms.TryGetValue(typeof(T), out handler), () =>
			{
				return String.Concat("transform required for ", typeof(T).GetReadableFullName());
			});
			TransformationHandler<T> strongTypedHandler = (TransformationHandler<T>)handler;

			MessageProperties props = new MessageProperties();
			if (_usingTransactions) props.DeliveryMode = 2;
			var rawMessage = strongTypedHandler.Transform(props, message);

			_model.BasicPublish(_exchangeName, routingKey, MakeBasicProperties(props), rawMessage);
			if (_usingTransactions) _model.TxCommit();

			return this;
		}
		
		private IBasicProperties MakeBasicProperties(MessageProperties props)
		{
			var copy = _model.CreateBasicProperties();
			if (!String.IsNullOrEmpty(props.AppId))	copy.AppId = props.AppId;
			if (!String.IsNullOrEmpty(props.ClusterId)) copy.ClusterId = props.ClusterId;
			if (!String.IsNullOrEmpty(props.ContentEncoding)) copy.ContentEncoding = props.ContentEncoding;
			if (!String.IsNullOrEmpty(props.ContentType)) copy.ContentType = props.ContentType;
			copy.DeliveryMode = props.DeliveryMode;
			if (!String.IsNullOrEmpty(props.MessageId)) copy.MessageId = props.MessageId;
			copy.Priority = props.Priority;
			if (!String.IsNullOrEmpty(props.ReplyTo)) copy.ReplyTo = props.ReplyTo;
			if (!String.IsNullOrEmpty(props.Type)) copy.Type = props.Type;
			if (!String.IsNullOrEmpty(props.UserId)) copy.UserId = props.UserId;
			return copy;
		}
				
		public IMessageBusProducerEndpoint PublishRawMessage(byte[] messageBytes)
		{
			throw new NotImplementedException();
		}

		public IMessageBusProducerEndpoint UseTransactions()
		{
			_model.TxSelect();
			_usingTransactions = true;
			return this;
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
