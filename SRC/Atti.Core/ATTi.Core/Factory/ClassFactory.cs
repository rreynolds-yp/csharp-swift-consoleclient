using System;
using ATTi.Core.Factory.Ctor;
using ATTi.Core.Factory;

namespace ATTi.Core.Factory
{
	public class ClassFactory<T> : IFactory<T>
		where T : class
	{
		private readonly Type _tt = typeof(T);		
		private ICtor<T> _ctor;	
		private FactoryStrategy<T> _strategy;

		protected ICtor<T> Ctor { get { return _ctor; } }
		
		protected ClassFactory()
		{
			_ctor = Ctor<T>.Singleton;
			_strategy = Factory<T>.Strategy;
		}

		#region PerformCreateInstance Generics
		protected T PerformCreateInstance<A>(A a)
		{
			ICtor<T, A> c = _ctor as ICtor<T, A>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A>(_ctor);
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.Construct(a);
			}, out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1>(A a, A1 a1)
		{
			ICtor<T, A, A1> c = _ctor as ICtor<T, A, A1>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1>(_ctor);
			FactoryAction action; T result = _strategy.PerformStrategy(() =>
			{
				return c.Construct(a, a1);
			}, out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2>(A a, A1 a1, A2 a2)
		{
			ICtor<T, A, A1, A2> c = _ctor as ICtor<T, A, A1, A2>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3>(A a, A1 a1, A2 a2, A3 a3)
		{
			ICtor<T, A, A1, A2, A3> c = _ctor as ICtor<T, A, A1, A2, A3>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4>(A a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			ICtor<T, A, A1, A2, A3, A4> c = _ctor as ICtor<T, A, A1, A2, A3, A4>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			ICtor<T, A, A1, A2, A3, A4, A5> c = _ctor as ICtor<T, A, A1, A2, A3, A4, A5>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4, A5>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4, a5), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			ICtor<T, A, A1, A2, A3, A4, A5, A6> c = _ctor as ICtor<T, A, A1, A2, A3, A4, A5, A6>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4, a5, a6), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			ICtor<T, A, A1, A2, A3, A4, A5, A6, A7> c = _ctor as ICtor<T, A, A1, A2, A3, A4, A5, A6, A7>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4, a5, a6, a7), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8> c = _ctor as ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7, A8>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4, a5, a6, a7, a8), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		protected T PerformCreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9> c = _ctor as ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>;
			if (c == null) c = Ctor<T>.AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(_ctor);
			FactoryAction action; 
			T result = _strategy.PerformStrategy(() => c.Construct(a, a1, a2, a3, a4, a5, a6, a7, a8, a9), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
			return result;
		}
		#endregion

		#region IFactory<T> Members

		public T CreateInstance()
		{
			FactoryAction action;
			T result = _strategy.PerformStrategy(() => _ctor.Construct(), out action);
			Factory<T>.NotifyInstanceAction(_tt, result, null, action);
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
			get { return _tt; }
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
