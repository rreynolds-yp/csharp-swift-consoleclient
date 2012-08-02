using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.Data
{
	public abstract class DbContextCache<TKey>
	{
		DbContextCache<TKey> _next;

		internal void SetNextContextCache(DbContextCache<TKey> next)
		{
			_next = next;
		}

		public bool TryGetInstance<TValue>(TKey key, out TValue instance)
		{
			if (!PerformTryGetInstance<TValue>(key, out instance)
				&& _next != null)
			{
				return _next.TryGetInstance<TValue>(key, out instance);
			}
			instance = default(TValue);
			return false;
		}

		protected abstract bool PerformTryGetInstance<TValue>(TKey key, out TValue instance);
		public abstract void PutInstance<TValue>(TKey key, TValue instance);
		public abstract void RemoveInstance(TKey key);
	}

	public class DefaultContextCache<TKey> : DbContextCache<TKey>
	{
		Dictionary<TKey, object> _instances = new Dictionary<TKey, object>();

		protected override bool PerformTryGetInstance<TValue>(TKey key, out TValue instance)
		{
			object o;
			if (_instances.TryGetValue(key, out o))
			{
				if (typeof(TValue).IsInstanceOfType(o))
				{
					instance = (TValue)o;
					return true;
				}
			}
			instance = default(TValue);
			return false;
		}

		public override void PutInstance<TValue>(TKey key, TValue instance)
		{
			if (_instances.ContainsKey(key))
			{
				_instances[key] = instance;
			}
			else
			{
				_instances.Add(key, instance);
			}
		}

		public override void RemoveInstance(TKey key)
		{
			_instances.Remove(key);
		}
	}
}
