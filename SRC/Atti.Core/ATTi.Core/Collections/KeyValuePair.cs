using System;
using System.Collections.Generic;
using System.Linq;

namespace ATTi.Core.Collections
{
	public interface IKeyValuePair
	{
		object GetKey();
		bool TryGetValue<V>(out V value);
		V ConvertValue<V>();
	}

	public interface IKeyValuePair<K> : IKeyValuePair
	{
		K Key { get; }
	}

	public interface IKeyValuePair<K, V> : IKeyValuePair<K>
	{
		V Value { get; }
	}

	public interface IKeyMultiValue<K, V> : IKeyValuePair<K>
	{
		V ValueAt(int index);
	}

	public struct KeyValuePair<K, V> : IKeyValuePair<K, V>
	{
		public static readonly KeyValuePair<K, V> Empty = new KeyValuePair<K, V>(default(K), default(V));

		private readonly K _key;
		private readonly V _value;

		public KeyValuePair(K key, V value)
		{
			_key = key;
			_value = value;
		}

		public K Key { get { return _key; } }
		public V Value { get { return _value; } }

		#region IKeyValuePair Members

		object IKeyValuePair.GetKey()
		{
			return this.Key;
		}

		bool IKeyValuePair.TryGetValue<T>(out T value)
		{
			if (typeof(T).IsAssignableFrom(typeof(V)))
			{
				value = (T)((object)_value); // Implicit conversion by boxing/unboxing
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		T IKeyValuePair.ConvertValue<T>()
		{
			if (typeof(T).IsInstanceOfType(_value))
			{
				return (T)((object)_value); // Implicit conversion by boxing/unboxing
			}
			else
			{
				return (T)Convert.ChangeType(_value, typeof(T));
			}
		}

		#endregion
	}

	public struct KeyMultiValue<K, V> : IKeyMultiValue<K, V>, IEnumerable<V>
	{
		public static readonly KeyMultiValue<K, V> Empty = new KeyMultiValue<K, V>(default(K), new V[0]);

		private readonly K _key;
		private readonly V[] _values;

		public KeyMultiValue(K key, IEnumerable<V> values)			
		{
			_key = key;
			_values = values.ToArray();
		}

		public int Count { get { return _values.Length; } }

		public V this[int index]
		{
			get { return _values[index]; }
		}

		#region IEnumerable<V> Members

		public IEnumerator<V> GetEnumerator()
		{
			return (IEnumerator<V>)_values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IKeyMultiValue<K,V> Members

		public V ValueAt(int index)
		{
			return _values[index];
		}

		#endregion

		#region IKeyValuePair<K> Members

		public K Key
		{
			get { return _key; }
		}

		#endregion

		#region IKeyValuePair Members

		object IKeyValuePair.GetKey()
		{
			return this.Key;
		}

		bool IKeyValuePair.TryGetValue<T>(out T value)
		{
			if (typeof(T).IsAssignableFrom(typeof(V)) && _values.Length > 0)
			{
				value = (T)((object)_values[0]); // Implicit conversion by boxing/unboxing
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		T IKeyValuePair.ConvertValue<T>()
		{
			if (_values.Length > 0)
			{
				if (typeof(T).IsInstanceOfType(_values[0]))
				{
					return (T)((object)_values[0]); // Implicit conversion by boxing/unboxing
				}
				else
				{
					return (T)Convert.ChangeType(_values[0], typeof(T));
				}
			}
			return default(T);
		}

		#endregion
	}
}
