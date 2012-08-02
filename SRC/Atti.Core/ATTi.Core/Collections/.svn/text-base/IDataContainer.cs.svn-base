namespace ATTi.Core.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	/// <summary>
	/// Simple interface for container of keyed data. Elements in this container
	/// are identified identified by unique keys of type K.
	/// </summary>
	/// <typeparam name="K">Type K of the Key</typeparam>
	public interface IDataContainer<K> : IEnumerable<IKeyValuePair<K, IUntypedTuple>>
	{
		/// <summary>
		/// Number of elements in the container.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Gets and sets an untyped tuple containing the data element identified by the key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IUntypedTuple this[K key]
		{
			get; set;
		}

		/// <summary>
		/// Adds data element to the container.
		/// </summary>
		/// <typeparam name="V"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Add<V>(K key, V value);

		/// <summary>
		/// Determines if a data element identified by the key given is in the container.
		/// </summary>
		/// <param name="key">The data element's key</param>
		/// <returns><em>true</em> if the data element is present in the container; otherwise <em>false</em></returns>
		bool Contains(K key);

		/// <summary>
		/// Gets a new instance containing the data elements that are not also
		/// contained in the other instance given.
		/// </summary>
		/// <param name="other">Another data container.</param>
		/// <returns>A new instance containing the difference between the two data containers.</returns>
		IDataContainer<K> Diff(IDataContainer<K> other);

		/// <summary>
		/// Gets a data element's value; throws ExpectedDataMissingException if the element
		/// is not in the container.
		/// </summary>
		/// <typeparam name="V">The element's value type V</typeparam>
		/// <param name="key">The data element's key</param>
		/// <returns>The element's value.</returns>
		V Expect<V>(K key);

		V Get<V>(K key);

		void Put<V>(K key, V value);

		bool TryGet<V>(K key, out V value);

		IDataContainer<K> Union(IDataContainer<K> other);
	}

	public class DataContainer<K> : IDataContainer<K>
	{
		

		Dictionary<K, IUntypedTuple> _inner = new Dictionary<K, IUntypedTuple>();

		

		

		public int Count
		{
			get { return _inner.Count; }
		}

		public IUntypedTuple this[K key]
		{
			get { return _inner[key]; }
			set { _inner[key] = value; }
		}

		public void Add<V>(K key, V value)
		{
			_inner.Add(key, new Tuple<V>(value));
		}

		public bool Contains(K key)
		{
			return _inner.ContainsKey(key);
		}

		public IDataContainer<K> Diff(IDataContainer<K> other)
		{
			throw new NotImplementedException();
		}

		public V Expect<V>(K key)
		{
			IUntypedTuple tuple;
			if (_inner.TryGetValue(key, out tuple))
			{
				if (tuple is ITuple<V>) return ((ITuple<V>)tuple).Value;
				try
				{
					return (V)Convert.ChangeType(tuple.UntypedValue, typeof(V));
				}
				catch (Exception e)
				{
					throw new InvalidCastException(
						String.Format("Cannot convert value: value is {0}, expected type is {1}"
						, tuple.GetType().FullName
						, typeof(V).FullName),
						e
						);
				}
			}
			throw new ArgumentOutOfRangeException("key");
		}

		public V Get<V>(K key)
		{
			IUntypedTuple tuple = _inner[key];
			if (tuple is ITuple<V>) return ((ITuple<V>)tuple).Value;
			else return (V)Convert.ChangeType(tuple.UntypedValue, typeof(V));
		}

		public IEnumerator<IKeyValuePair<K, IUntypedTuple>> GetEnumerator()
		{
			foreach (var kvp in _inner)
			{
				yield return new KeyValuePair<K, IUntypedTuple>(kvp.Key, kvp.Value);
			}
		}

		IDataContainer<K> IDataContainer<K>.Union(IDataContainer<K> other)
		{
			throw new NotImplementedException();
		}

		public void Put<V>(K key, V value)
		{
			_inner[key] = new Tuple<V>(value);
		}

		public bool Remove(K key)
		{
			return _inner.Remove(key);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TryGet<V>(K key, out V value)
		{
			IUntypedTuple tuple;
			if (_inner.TryGetValue(key, out tuple))
			{
				if (tuple is ITuple<V>) value = ((ITuple<V>)tuple).Value;
				else value = (V)Convert.ChangeType(tuple.UntypedValue, typeof(V));
				return true;
			}
			value = default(V);
			return false;
		}

		
	}

	public class XElementDataContainer : IDataContainer<string>
	{
		

		XElement _data;

		

		#region Constructors

		public XElementDataContainer(XElement data)
		{
			_data = new XElement("data", data);
		}

		#endregion Constructors

		

		public int Count
		{
			get { return _data.Attributes().Count() + _data.Elements().Count(); }
		}

		

		#region Indexers

		public IUntypedTuple this[string key]
		{
			get
			{
				return new Tuple<string>(Get<string>(key));
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion Indexers

		

		public void Add<V>(string key, V value)
		{
			throw new NotImplementedException();
		}

		public bool Contains(string key)
		{
			return _data.Attribute(key) != null || _data.Element(key) != null;
		}

		public IDataContainer<string> Diff(IDataContainer<string> other)
		{
			throw new NotImplementedException();
		}

		public V Expect<V>(string key)
		{
			string v;
			if (_data.TryReadNamedValue(key, out v))
			{
				try
				{
					return (V)Convert.ChangeType(v, typeof(V));
				}
				catch (Exception e)
				{
					throw new InvalidCastException(
						String.Format("Cannot convert value: value is '{0}', expected type is {1}"
						, v, typeof(V).FullName),
						e
						);
				}
			}
			throw new ArgumentOutOfRangeException("key");
		}

		public V Get<V>(string key)
		{
			V v;
			if (_data.TryReadNamedValue<V>(key, out v))
			{
				return v;
			}
			throw new ArgumentOutOfRangeException("key");
		}

		public IEnumerator<IKeyValuePair<string, IUntypedTuple>> GetEnumerator()
		{
			foreach (var kvp in (from a in _data.Attributes()
													 select new KeyValuePair<string, IUntypedTuple>(a.Name.LocalName, new Tuple<string>(a.Value)))
							.Union(from e in _data.Elements()
										 select new KeyValuePair<string, IUntypedTuple>(e.Name.LocalName, new Tuple<string>(e.Value))))
			{
				yield return kvp;
			}
		}

		public void Put<V>(string key, V value)
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TryGet<V>(string key, out V value)
		{
			return _data.TryReadNamedValue<V>(key, out value);
		}

		public IDataContainer<string> Union(IDataContainer<string> other)
		{
			throw new NotImplementedException();
		}

		
	}
}