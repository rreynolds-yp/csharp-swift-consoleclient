namespace ATTi.Core.Collections
{
	using System;

	/// <summary>
	/// Interface for a typed tuple.
	/// </summary>
	/// <typeparam name="T">The tuple's value type T</typeparam>
	public interface ITuple<T> : IUntypedTuple
	{
		/// <summary>
		/// Gets the tuple's value.
		/// </summary>
		T Value
		{
			get;
		}

		/// <summary>
		/// Gets the tuple's value by conversion to type V.
		/// </summary>
		/// <typeparam namue type V.</typeparam>
		/// <returns>The tuple's value, converted to type V.</returns>
		V ConvertValue<V>();

		/// <summary>
		/// Tries to get the tuple's value.
		/// </summary>
		/// <typeparam name="V">Value type V</typeparam>
		/// <param name="valion to write the value.</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em>.</returns>
		bool TryGetValue<V>(out V value);
	}

	public interface ITuple<T, T2> : ITuple<T>
	{
		T2 Value2
		{
			get;
		}
	}

	/// <summary>
	/// Interface for an untyped tuple.
	/// </summary>
	public interface IUntypedTuple
	{
		object UntypedValue { get; }

		bool ValueIs<T>();
	}

	/// <summary>
	/// Basic implementation of a tuple.
	/// </summary>
	/// <typeparam name="T">The tuple's value type T.</typeparam>
	public struct Tuple<T> : ITuple<T>
	{
		

		/// <summary>
		/// An empty tuple.
		/// </summary>
		public static readonly Tuple<T> Empty = new Tuple<T>(default(T));

		private readonly T _value;

		

		#region Constructors

		/// <summary>
		/// Creates a new tuple with the given value.
		/// </summary>
		/// <param name="value">The tuple's value.</param>
		public Tuple(T value)
		{
			_value = value;
		}

		#endregion Constructors

		

		/// <summary>
		/// Gets the tuple's value.
		/// </summary>
		public T Value
		{
			get { return _value; }
		}

		object IUntypedTuple.UntypedValue
		{
			get { return this.Value; }
		}

		

		

		public V ConvertValue<V>()
		{
			if (typeof(T).IsInstanceOfType(_value))
			{
				return (V)((object)_value); // Implicit conversion by boxing/unboxing
			}
			else
			{
				return (V)Convert.ChangeType(_value, typeof(T));
			}
		}

		public bool TryGetValue<V>(out V value)
		{
			if (typeof(T).IsAssignableFrom(typeof(V)))
			{
				value = (V)((object)_value); // Implicit conversion by boxing/unboxing
				return true;
			}
			else
			{
				value = default(V);
				return false;
			}
		}

		public bool ValueIs<V>()
		{
			return typeof(V).IsAssignableFrom(typeof(T));
		}

		
	
	}

	/// <summary>
	/// Implementation of the tuple associating two values.
	/// </summary>
	/// <typeparam name="T">The tuple's value type T</typeparam>
	/// <typeparam name="T2">The tuple's second value type T2</typeparam>
	public class Tuple<T, T2> : ITuple<T, T2>
	{
		

		/// <summary>
		/// An empty tuple.
		/// </summary>
		public static readonly Tuple<T, T2> Empty = new Tuple<T, T2>(default(T), default(T2));

		private readonly T _value;
		private readonly T2 _value2;

		

		#region Constructors

		/// <summary>
		/// Creates a new tuple with the two values given.
		/// </summary>
		/// <param name="value">The tuple's first value.</param>
		/// <param name="value2">The tuple's second value.</param>
		public Tuple(T value, T2 value2)
		{
			_value = value;
			_value2 = value2;
		}

		#endregion Constructors

		

		/// <summary>
		/// Gets the tuple's value.
		/// </summary>
		public T Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Gets the tuple's second value.
		/// </summary>
		public T2 Value2
		{
			get { return _value2; }
		}

		object IUntypedTuple.UntypedValue
		{
			get { return this.Value; }
		}

		

		

		public V ConvertValue<V>()
		{
			if (typeof(T).IsInstanceOfType(_value))
			{
				return (V)((object)_value); // Implicit conversion by boxing/unboxing
			}
			else
			{
				return (V)Convert.ChangeType(_value, typeof(T));
			}
		}

		public bool TryGetValue<V>(out V value)
		{
			if (typeof(T).IsAssignableFrom(typeof(V)))
			{
				value = (V)((object)_value); // Implicit conversion by boxing/unboxing
				return true;
			}
			else
			{
				value = default(V);
				return false;
			}
		}

		public bool ValueIs<V>()
		{
			return typeof(V).IsAssignableFrom(typeof(T));
		}

		
	}
}