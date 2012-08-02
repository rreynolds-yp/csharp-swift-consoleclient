using System;
using System.Runtime.Remoting;

namespace ATTi.Core.Factory
{	
	
	public abstract class FactoryStrategy<T>
	{
		public abstract T PerformStrategy<U>(Func<U> ctor, out FactoryAction action)
			where U : T;
	}

	internal class DefaultFactoryStrategy<T> : FactoryStrategy<T>
	{
		public override T PerformStrategy<U>(Func<U> ctor, out FactoryAction action)
		{
			action = FactoryAction.NewInstance;
			return ctor();
		}
	}

	internal class SingletonFactoryStrategy<T> : FactoryStrategy<T>
	{
		WeakReference _singletonRef;
		private readonly Object _lock = new Object();

		public override T PerformStrategy<U>(Func<U> ctor, out FactoryAction action)
		{
			lock (_lock)
			{
				if (_singletonRef != null && _singletonRef.IsAlive)
				{
					action = FactoryAction.CachedInstance;
					return (T)_singletonRef.Target;
				}
				else
				{ // Not the singleton/cached instance
					T result = ctor();
					action = FactoryAction.NewInstance;
					_singletonRef = new WeakReference(result, true);
					return result;
				}
			}
		}
	}

	internal class RemoteFactoryStrategy<T> : FactoryStrategy<T>
	{
		Type __t = typeof(T);

		public string NetworkLocation { get; set; }
		public string ChannelConfig { get; set; }
		
		public override T PerformStrategy<U>(Func<U> ctor, out FactoryAction action)
		{
			T activated = (T)Activator.GetObject(__t, NetworkLocation, ChannelConfig);
			action = FactoryAction.SharedInstance;
			return activated;
		}
	}
}
