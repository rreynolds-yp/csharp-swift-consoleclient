using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ATTi.Core.Factory.Implementer
{
	public static class ImplementerRegistry
	{
		static readonly Object __lock = new Object();
		static readonly Dictionary<Type, ITypeRegistration> __registrations = new Dictionary<Type, ITypeRegistration>();

		internal interface ITypeRegistration
		{
			Type TargetType { get; }
			Type ImplementerType { get; }
		}
		public class TypeRegistration<T> : ITypeRegistration
		{
			public Type ImplementerType { get; private set; }
			public Type TargetType { get { return typeof(T); } }

			public TypeRegistration<T> AutoImplementUsing<TImpl>()
				where TImpl : class, IImplementer, new()
			{
				if (this.ImplementerType != null) 
					throw new InvalidOperationException("Implementer already established for auto-implement purposes");
				this.ImplementerType = typeof(TImpl);
				return this;
			}
		}

		public static bool HasRegistration<T>()
		{
			return HasRegistration(typeof(T));
		}

		public static bool HasRegistration(Type t)
		{
			lock (__lock)
			{
				return __registrations.ContainsKey(t);
			}			
		}
		
		public static TypeRegistration<T> ForType<T>()
		{
			ITypeRegistration result;
			lock (__lock)
			{
				if (!__registrations.TryGetValue(typeof(T), out result))
				{
					__registrations.Add(typeof(T), result = new TypeRegistration<T>());
				}
			}
			return (TypeRegistration<T>)result;
		}


		internal static Type InvokeImplementorForType(Type intf, Type requestedType)
		{
			ITypeRegistration result;
			lock (__lock)
			{
				if (!__registrations.TryGetValue(intf, out result)) throw new InvalidOperationException();
			}
			IImplementer imp = Factory<IImplementer>.CreateImplInstance(result.ImplementerType);
			if (!imp.CanGenerateImplementationForType(requestedType)) 
				throw new InvalidOperationException(String.Format("Unable to generate implementation for type: {0}"
					, requestedType.FullName));
			return imp.GenerateImplementationForType(requestedType);
		}
	}
}
