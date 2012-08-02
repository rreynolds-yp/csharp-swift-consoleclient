using System;
using ATTi.Core.Factory.Ctor;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Abstract implementation of the IFacotry&lt;T> interface.
	/// </summary>
	/// <typeparam name="T">The factory's target type T</typeparam>
	/// <typeparam name="U">The factory's concrete type U; must be assignable to type T</typeparam>
	public class AbstractFactory<T, U> : IFactory<T>		
		where U : T
	{
		private readonly Type _tt = typeof(T);
		private readonly Type _uu;
		private IFactory<U> _factory;
		private FactoryStrategy<T> _strategy;

		protected AbstractFactory()			
		{
			_factory = Factory<U>.DefaultFactory;
			_strategy = Factory<T>.Strategy;
			_uu = _factory.ConcreteType;
			while (_uu.IsAbstract)
			{
				IFactory uf = _uu.GetFactory();
				_uu = uf.ConcreteType;
			}
		}

		#region PerformCreateInstance Generics
		protected T PerformCreateInstance<A>(A a)
		{
			IFactory<U, A> c = _factory as IFactory<U, A>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1>(A a, A1 a1)
		{
			IFactory<U, A, A1> c = _factory as IFactory<U, A, A1>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2>(A a, A1 a1, A2 a2)
		{
			IFactory<U, A, A1, A2> c = _factory as IFactory<U, A, A1, A2>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3>(A a, A1 a1, A2 a2, A3 a3)
		{
			IFactory<U, A, A1, A2, A3> c = _factory as IFactory<U, A, A1, A2, A3>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4>(A a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			IFactory<U, A, A1, A2, A3, A4> c = _factory as IFactory<U, A, A1, A2, A3, A4>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			IFactory<U, A, A1, A2, A3, A4, A5> c = _factory as IFactory<U, A, A1, A2, A3, A4, A5>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			IFactory<U, A, A1, A2, A3, A4, A5, A6> c = _factory as IFactory<U, A, A1, A2, A3, A4, A5, A6>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			IFactory<U, A, A1, A2, A3, A4, A5, A6, A7> c = _factory as IFactory<U, A, A1, A2, A3, A4, A5, A6, A7>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			IFactory<U, A, A1, A2, A3, A4, A5, A6, A7, A8> c = _factory as IFactory<U, A, A1, A2, A3, A4, A5, A6, A7, A8>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7, a8);
			}, out action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			IFactory<U, A, A1, A2, A3, A4, A5, A6, A7, A8, A9> c = _factory as IFactory<U, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>;
			if (c == null) throw new NotImplementedException();
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7, a8, a9);
			}, out action);
			return result;
		}
		#endregion

		#region IFactory<T> Members

		public T CreateInstance()
		{
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => { return _factory.CreateInstance(); }, out action);			
			return result;
		}
		public T CreateInstance(string name)
		{
			throw new NotImplementedException();
		}
		public System.Collections.Generic.IEnumerable<T> GetAllInstances()
		{
			throw new NotImplementedException();
		}
		public event FactoryInstanceEvent<T> InstanceAction
		{
			add { Factory<T>.InstanceAction += value; }
			remove { Factory<T>.InstanceAction -= value; }
		}

		#endregion

		#region IFactory Members
		public Type TargetType
		{
			get { return _tt; }
		}
		public Type ConcreteType
		{
			get { return _uu; }
		}
		object IFactory.CreateUntyped()
		{
			return this.CreateInstance();
		}				
		public object CreateUntyped(string name)
		{
			return this.CreateInstance(name);
		}
		public System.Collections.Generic.IEnumerable<object> GetUntypedInstances()
		{
			foreach (T item in this.GetAllInstances())
			{
				yield return item;
			}
		}
		#endregion
	}
}
