using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using ATTi.Core.Configuration;
using ATTi.Core.Configuration.Metadata;
using ATTi.Core.Contracts;
using ATTi.Core.Dto;
using ATTi.Core.Dto.Json;
using ATTi.Core.Dto.SPI;
using ATTi.Core.Factory.Ctor;
using ATTi.Core.Factory.Implementer;
using ATTi.Core.Factory.Metadata;
using ATTi.Core.Reflection;
using ATTi.Core.Reflection.Emit;
using ATTi.Core.Trace;
using ATTi.Core.Wireup;
using Newtonsoft.Json.Linq;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Static Factory for target type T.
	/// </summary>
	/// <typeparam name="T">The factory's target type.</typeparam>
	public static class Factory<T>
	{
		#region Declarations
		private readonly static string EmittedAssemblyNameFormat = "{0}v{1}__Factories_{2}";
		private readonly static string EmittedCreateInstanceMethodName = "CreateInstanceWithInitialization";
		private readonly static string EmittedPerformCreateInstanceMethodName = "PerformCreateInstance";

		private static readonly Exception __initializationException;
		private static readonly Type __tt = typeof(T);
		private static readonly FactoryWireup __wireup = new FactoryWireup();
		private static readonly Object __sync = new Object();
		private static readonly ICtor<T> __ctor;
		private static IFactory<T> __factory;
		private static FactoryStrategy<T> __strategy;
		private static ConfigurableAttribute _configAttribute;
		private static Object __configInstance;
		private static Dictionary<Type, IFactory> __implFactories;
		#endregion

		#region Constructors
		static Factory()
		{
			WireupCoordinator.CoordinateWireup(__tt.Assembly);
			try
			{
				if (!__tt.IsAbstract) __ctor = Ctor<T>.Singleton;

				if (__tt.IsDefined(typeof(ConfigurableTypeAttribute), true))
				{
					ConfigurableType<T>.ConfigureType();
				}
				_configAttribute = __tt.DiscoverFirstCustomAttribute<ConfigurableAttribute>(false);
				if (_configAttribute != null)
				{
					Factory<T>.InstanceAction += (FactoryInstanceEvent<T>)
						Delegate.CreateDelegate(typeof(FactoryInstanceEvent<T>), null
							, typeof(Factory<T>).GetMethod("InstanceCreated_Configure"
							, BindingFlags.NonPublic | BindingFlags.Static
							).MakeGenericMethod(_configAttribute.ConfigElementType));
				}
				foreach (Type it in __tt.DiscoverMostDerivedInterfacesByInheritance())
				{
					AttachEventHandlersForTypeFactory(it);
				}
				Type bt = __tt.BaseType;
				if (bt != null)
				{
					AttachEventHandlersForTypeFactory(bt);
				}
			}
			catch (Exception e)
			{
				__initializationException = e;
			}
		}
		#endregion

		#region Configuration Handling

		private static object ConfigObject
		{
			get
			{
				if (__configInstance == null)
				{
					__configInstance = ConfigurationUtil.EnsureSectionOrDefault<ConfigurationSection>(_configAttribute.ConfigSectionName);
				}
				return __configInstance;
			}
		}

		private static void InstanceCreated_Configure<TConfig>(Type sender, T instance, string name, FactoryAction action)
		{
			if (action == FactoryAction.NewInstance)
			{
				((IConfigurable<TConfig>)instance)
					.Configure((TConfig)Factory<T>.ConfigObject);
			}
		}

		internal static void SetInstanceConfig<TConfig>(TConfig config)
			where TConfig : class
		{
			__configInstance = config;
			Type tconfigurable = null;
			Type tconfig = typeof(TConfig);
			while (tconfig != null)
			{
				foreach (Type type in __tt.DiscoverGenericInterfaceImplementations(typeof(IConfigurable<>)))
				{
					if (Object.Equals(tconfig, type.GetGenericArguments()[0]))
					{
						tconfigurable = type;
						break;
					}
				}
				if (tconfigurable != null) break;
				tconfig = tconfig.BaseType;
			}
			if (tconfigurable == null)
				throw new FactoryException(String.Format(Properties.Resources.Error_InvalidInstanceConfigForFactory
					, __tt.FullName, typeof(TConfig)));
			Factory<T>.InstanceAction += (FactoryInstanceEvent<T>)
					Delegate.CreateDelegate(typeof(FactoryInstanceEvent<T>), null
						, typeof(Factory<T>).GetMethod("InstanceCreated_Configure"
						, BindingFlags.NonPublic | BindingFlags.Static
						).MakeGenericMethod(tconfig));
			__configInstance = config;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// The factory wireup instance for type T.
		/// </summary>
		public static FactoryWireup Wireup
		{
			get
			{
				Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

				return __wireup;
			}
		}

		/// <summary>
		/// The concrete class type of instaces produced by this factory. This type will
		/// correspond to the default factory's concrete type.
		/// </summary>
		public static Type ConcreteType
		{
			get
			{
				Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

				return DefaultFactory.ConcreteType;
			}
		}

		/// <summary>
		/// The default factory for target type T; creates instances of Factory&lt;T>.ConcreteType.
		/// </summary>
		public static IFactory<T> DefaultFactory
		{
			get
			{
				Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

				return Util.LazyInitializeWithLock<IFactory<T>>(ref __factory, __sync, () => ResolveFactory());
			}
		}

		/// <summary>
		/// Event fires when instances of the type are created by a factory.
		/// NOTE: This event fires on any factory event for the type, including
		/// subtypes. Each event hander will have a negative impact on overall
		/// performance of the system... moreso when associated with types that
		/// have many specializations. This occurs during the object-creation 
		/// cycle and should be considered an inner loop; placing too much logic 
		/// in an inner loop can have a seriously negative impact on performance!
		/// </summary>		
		public static event FactoryInstanceEvent<T> InstanceAction
		{
			add { __instanceAction += value; }
			remove { __instanceAction -= value; }
		}
		static FactoryInstanceEvent<T> __instanceAction;

		#region CreateInstance overloads

		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by the default factory.</exception>
		public static T CreateInstance()
		{
			return DefaultFactory.CreateInstance();
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A>(A a)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1>(A a, A1 a1)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2>(A a, A1 a1, A2 a2)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3>(A a, A1 a1, A2 a2, A3 a3)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4>(A a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <param name="a5">The concrete type must accept an argument of type A5 in position five.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4, A5>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4, a5);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <param name="a5">The concrete type must accept an argument of type A5 in position five.</param>
		/// <param name="a6">The concrete type must accept an argument of type A6 in position six.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4, A5, A6>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4, a5, a6);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <param name="a5">The concrete type must accept an argument of type A5 in position five.</param>
		/// <param name="a6">The concrete type must accept an argument of type A6 in position six.</param>
		/// <param name="a7">The concrete type must accept an argument of type A7 in position seven.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4, A5, A6, A7>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4, a5, a6, a7);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <param name="a5">The concrete type must accept an argument of type A5 in position five.</param>
		/// <param name="a6">The concrete type must accept an argument of type A6 in position six.</param>
		/// <param name="a7">The concrete type must accept an argument of type A7 in position seven.</param>
		/// <param name="a8">The concrete type must accept an argument of type A8 in position eight.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4, a5, a6, a7, a8);
		}
		/// <summary>
		/// Creates an instance of target type T with the default factory and factory strategy.
		/// </summary>
		/// <param name="a">The concrete type must accept an argument of type A in position zero.</param>
		/// <param name="a1">The concrete type must accept an argument of type A1 in position one.</param>
		/// <param name="a2">The concrete type must accept an argument of type A2 in position two.</param>
		/// <param name="a3">The concrete type must accept an argument of type A3 in position three.</param>
		/// <param name="a4">The concrete type must accept an argument of type A4 in position four.</param>
		/// <param name="a5">The concrete type must accept an argument of type A5 in position five.</param>
		/// <param name="a6">The concrete type must accept an argument of type A6 in position six.</param>
		/// <param name="a7">The concrete type must accept an argument of type A7 in position seven.</param>
		/// <param name="a8">The concrete type must accept an argument of type A8 in position eight.</param>
		/// <param name="a9">The concrete type must accept an argument of type A9 in position nine.</param>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be
		/// created by a compatible constructor. This failure indicates that no assignment-compatible
		/// constructor exists on the DefaultFactory's concrete-type.</exception>
		public static T CreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			return CreateInstanceFindCompatibleCallOnFailure(DefaultFactory, a, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}

		#endregion

		#region CreateForSubtype overloads
		/// <summary>
		/// Creates an derived type U and applies the factory strategy for type T.
		/// </summary>
		/// <returns>An instance of target type T.</returns>
		/// <exception cref="System.NotImplementedException">thrown if an instance cannot be created.</exception>
		public static T CreateForSubtype<U>() where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance(); });
		}
		public static T CreateForSubtype<U, A>(A a)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A>(a); });
		}
		public static T CreateForSubtype<U, A, A1>(A a, A1 a1)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1>(a, a1); });
		}
		public static T CreateForSubtype<U, A, A1, A2>(A a, A1 a1, A2 a2)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2>(a, a1, a2); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3>(A a, A1 a1, A2 a2, A3 a3)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3>(a, a1, a2, a3); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4>(A a, A1 a1, A2 a2, A3 a3, A4 a4)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4>(a, a1, a2, a3, a4); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4, A5>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4, A5>(a, a1, a2, a3, a4, a5); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4, A5, A6>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4, A5, A6>(a, a1, a2, a3, a4, a5, a6); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4, A5, A6, A7>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4, A5, A6, A7>(a, a1, a2, a3, a4, a5, a6, a7); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4, A5, A6, A7, A8>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8>(a, a1, a2, a3, a4, a5, a6, a7, a8); });
		}
		public static T CreateForSubtype<U, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
			where U : T
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => { return Factory<U>.CreateInstance<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(a, a1, a2, a3, a4, a5, a6, a7, a8, a9); });
		}

		#endregion

		#region CreateImplInstance overloads

		public static T CreateImplInstance(Type implType)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			if (__tt.IsSealed) throw new InvalidOperationException();
			return PerformCreateInstance(() => (T)GetImplFactory(implType).CreateUntyped());
		}
		public static T CreateImplInstance<A>(Type implType, A a)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			if (__tt.IsSealed) throw new InvalidOperationException();

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A>(GetImplFactory(implType), a));
		}
		public static T CreateImplInstance<A, A1>(Type implType, A a, A1 a1)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1>(GetImplFactory(implType), a, a1));
		}
		public static T CreateImplInstance<A, A1, A2>(Type implType, A a, A1 a1, A2 a2)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2>(GetImplFactory(implType), a, a1, a2));
		}
		public static T CreateImplInstance<A, A1, A2, A3>(Type implType, A a, A1 a1, A2 a2, A3 a3)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => { return CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3>(GetImplFactory(implType), a, a1, a2, a3); });
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4>(GetImplFactory(implType), a, a1, a2, a3, a4));
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4, A5>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5>(GetImplFactory(implType), a, a1, a2, a3, a4, a5));
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4, A5, A6>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6>(GetImplFactory(implType), a, a1, a2, a3, a4, a5, a6));
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4, A5, A6, A7>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7>(GetImplFactory(implType), a, a1, a2, a3, a4, a5, a6, a7));
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4, A5, A6, A7, A8>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7, A8>(GetImplFactory(implType), a, a1, a2, a3, a4, a5, a6, a7, a8));
		}
		public static T CreateImplInstance<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(Type implType, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
			Contracts.Require.IsNotNull<Type>("implType", implType);
			Contracts.Require.IsAssignableFrom<T>("implType", implType);

			return PerformCreateInstance(() => CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(GetImplFactory(implType), a, a1, a2, a3, a4, a5, a6, a7, a8, a9));
		}
		#endregion

		#region Copier nested class
		/// <summary>
		/// Utility for copy construction for the type U.
		/// </summary>
		/// <typeparam name="U">Target type U; must be assignable to target type T.</typeparam>
		public static class Copier<U> where U : T
		{
			/// <summary>
			/// Creates a strict copy of the source object. All properties of source type S must be present 
			/// and writable on target type U. 
			/// </summary>
			/// <typeparam name="S">Source type.</typeparam>
			/// <param name="source">An instance of type S to serve as a source of data for the copy.</param>
			/// <returns>A new instance of type U (upcast if necessary) to type T</returns>
			/// <exception cref="InvalidOperationException">thrown if source type S has a property that
			/// does not exist on target type U.</exception>
			public static T CopyStrict<S>(S source)
				where S : class
			{
				return StrictCopyHelper<S>.Copy(source);
			}

			/// <summary>
			/// Creates a projection that will perform a strict copy of source type S when called.
			/// </summary>
			/// <typeparam name="S">Source type S</typeparam>
			/// <returns>A projection that can perform a strict copy of source type S</returns>
			public static Func<S, T> CopyStrictProjection<S>()
				where S : class
			{
				return (s) => CopyStrict(s);
			}

			/// <summary>
			/// Creates a projection that will call the given function and perform a strict copy
			/// of its result.
			/// </summary>
			/// <typeparam name="S">Source type S</typeparam>
			/// <param name="projection">A method that returns an instance of source type S.</param>
			/// <returns>A projection that can invoke the given function and perform a strict copy of it's result.</returns>
			public static Func<T> DependentCopyStrictProjection<S>(Func<S> projection)
				where S : class
			{
				return () => CopyStrict(projection());
			}

			/// <summary>
			/// Copy constructor wherein public properties common between type
			/// U and type S are copied to the target. Properties that match by
			/// name are copied, if not by assignability then by cascading loose copy.
			/// </summary>
			/// <typeparam name="S">Source type.</typeparam>
			/// <param name="source">An instance of type S to serve as a source of data for the copy.</param>
			/// <returns>A new instance of type U upcast to type T</returns>
			public static T CopyLoose<S>(S source)
			{
				return LooseCopyHelper<S>.Copy(source);
			}

			/// <summary>
			/// Creates a projection that will perform a loose copy of source type S when called.
			/// </summary>
			/// <typeparam name="S">Source type S</typeparam>
			/// <returns>A projection that can perform a loose copy of source type S</returns>
			public static Func<S, T> CopyLooseProjection<S>()
				where S : class
			{
				return (s) => CopyLoose(s);
			}

			/// <summary>
			/// Creates a projection that will call the given function and perform a loose copy
			/// of its result.
			/// </summary>
			/// <typeparam name="S">Source type S</typeparam>
			/// <param name="projection">A method that returns an instance of source type S.</param>
			/// <returns>A projection that can invoke the given function and perform a loose copy of it's result.</returns>
			public static Func<T> DependentCopyLooseProjection<S>(Func<S> projection)
				where S : class
			{
				return () => CopyLoose(projection());
			}

			/// <summary>
			/// Creates an instance of target type T using data from the source XML element. This
			/// is equivelant to a loose copy... not all properties need be present in the XML.
			/// </summary>
			/// <param name="source">An XML element containing source data for the resulting instance.</param>
			/// <returns>An instance of target type T populated with data from the source XML element.</returns>
			public static T CopyFromXml(XElement source)
			{
				return XmlCopyHelper.Copy(source);
			}

			/// <summary>
			/// Creates an instance of target type T using data from the source XML element. 
			/// This is equivelant to a loose copy; not all properies need be present in the XML and
			/// additional data present in the XML will be copied to the instance if the instance
			/// is a data transfer object (supports IDataTransferObjectSPI).
			/// </summary>
			/// <param name="source">An XML element containing source data for the resulting instance.</param>
			/// <returns>An instance of target type T populated with data from the source XML element.</returns>
			public static T CopyFromXmlWithCapture(XElement source)
			{
				return XmlCopyHelper.CopyWithCapture(source);
			}

			public static T CopyFromJson(JObject source)
			{
				return JsonCopyHelper.Copy(source);
			}

			public static T CopyFromJsonWithCapture(JObject source)
			{
				return JsonCopyHelper.CopyWithCapture(source);
			}

			#region StrictCopyHelper nested class
			private static class StrictCopyHelper<S> where S : class
			{
				private static readonly Func<S, U> __ctor;
				private static readonly Exception __initializationException;

				internal static U Copy(S source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull<S>("source", source);

					return __ctor(source);
				}

				static StrictCopyHelper()
				{
					try
					{
						__ctor = BuildStrictCopyLoader();
						__initializationException = null;
					}
					catch (Exception e)
					{
						__ctor = null;
						__initializationException = e;
					}
				}

				private static Func<S, U> BuildStrictCopyLoader()
				{
					DynamicMethod method = new DynamicMethod("StrictCopyConstructor"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(S) }
						, typeof(S)
						, false
						);

					ILHelper ilh = new ILHelper(method.GetILGenerator());

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<U>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					foreach (var src in typeof(S).GetReadableProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
					{
						PropertyInfo dest = typeof(U).GetReadablePropertyWithAssignmentCompatablity(src.Name
							, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy
							, src.PropertyType
							);
						if (dest == null) throw new InvalidOperationException(
							String.Format("Cannot create a strong copy constructor because the objects do not have all properties in common: source [{0}], destination [{1}]"
							, typeof(S).FullName, typeof(U).FullName));

						//
						// result.<property name> = src.<property name>;
						//
						ilh.LoadLocal(result);
						ilh.LoadArg_0();
						ilh.CallVirtual(src.GetGetMethod());
						ilh.CallVirtual(dest.GetSetMethod());
					}
					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<S, U>)method.CreateDelegate(typeof(Func<S, U>));
				}
			}
			#endregion

			#region LooseCopyHelper nested class
			private static class LooseCopyHelper<S>
			{
				private static readonly Func<S, U> __ctor;
				private static readonly Exception __initializationException;

				internal static U Copy(S source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull<S>("source", source);

					return __ctor(source);
				}

				static LooseCopyHelper()
				{
					try
					{
						__ctor = BuildLooseCopyConstructor();
						__initializationException = null;
					}
					catch (Exception e)
					{
						__ctor = null;
						__initializationException = e;
					}
				}

				private static Func<S, U> BuildLooseCopyConstructor()
				{
					DynamicMethod method = new DynamicMethod("LooseCopyConstructor"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(S) }
						, typeof(U)
						, false
						);

					ILHelper ilh = new ILHelper(method.GetILGenerator());
					bool isDto = typeof(IDataTransferObject).IsAssignableFrom(typeof(U));

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<U>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					LocalBuilder localCeq = ilh.DeclareLocal(typeof(bool));
					Label branchLabel;

					foreach (var prop in from src in typeof(S).GetReadableProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
															 join dest in typeof(U).GetWritableProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
															 on src.Name equals dest.Name
															 select new
															 {
																 Source = src,
																 Destination = dest
															 })
					{
						if (prop.Destination.PropertyType.IsAssignableFrom(prop.Source.PropertyType))
						{
							//
							// result.<property-name> = src.<property-name>;
							//
							ilh.LoadLocal(result);
							ilh.LoadArg_0();
							ilh.CallVirtual(prop.Source.GetGetMethod());
							ilh.CallVirtual(prop.Destination.GetSetMethod());
						}
						else
						{
							//
							// <source-property-type> temp_<n>;
							// if (temp_<n> != null)
							// {
							//   result.<property-name> = Factory<destination-property-type>
							//     .Copier<destination-property-type>
							//     .LooseCopy<source-property-type>(temp_<n>);
							// }
							//
							LocalBuilder temp = ilh.DeclareLocal(prop.Source.PropertyType);
							branchLabel = ilh.DefineLabel();
							ilh.LoadArg_0();
							ilh.CallVirtual(prop.Source.GetGetMethod());
							ilh.StoreLocal(temp);
							ilh.LoadLocal(temp);
							ilh.LoadNull();
							ilh.CompareEqual();
							ilh.BranchIfTrue_ShortForm(branchLabel);

							Type copiertype = typeof(Factory<>).MakeGenericType(prop.Destination.PropertyType)
								.GetNestedType("Copier`1", BindingFlags.Public | BindingFlags.Static)
								.MakeGenericType(prop.Destination.PropertyType, prop.Destination.PropertyType);
							MethodInfo m = copiertype.DiscoverGenericMethodDefinitionWithArgumentCount("CopyLoose", BindingFlags.Static | BindingFlags.Public, 1)
								.MakeGenericMethod(prop.Source.PropertyType);

							ilh.LoadLocal(result);
							ilh.LoadLocal(temp);
							ilh.Call(m);
							ilh.CallVirtual(prop.Destination.GetSetMethod());
							ilh.Nop();
							ilh.MarkLabel(branchLabel);
						}
					}
					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<S, U>)method.CreateDelegate(typeof(Func<S, U>));
				}

			}
			#endregion

			#region XmlCopyHelper nested class
			private static class XmlCopyHelper
			{
				private static readonly Func<XElement, U> __ctor;
				private static readonly Func<XElement, U> __ctorWithCapture;
				private static PropertyInfo[] __cachedProperties;
				private static readonly Exception __initializationException;

				internal static U Copy(XElement source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull("source", source);

					return __ctor(source);
				}

				internal static U CopyWithCapture(XElement source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull("source", source);

					return __ctorWithCapture(source);
				}

				static XmlCopyHelper()
				{
					try
					{
						__ctor = BuildXElementCopyConstructor();
						__ctorWithCapture = BuildXElementCopyConstructorWithCapture();
						__initializationException = null;
					}
					catch (Exception e)
					{
						__ctor = null;
						__initializationException = e;
					}
				}

				private static Func<XElement, U> BuildXElementCopyConstructor()
				{
					DynamicMethod method = new DynamicMethod("XElementCopyConstructor"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(XElement) }
						, typeof(XmlCopyHelper)
						, false
						);

					ILHelper ilh = new ILHelper(method.GetILGenerator());

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<T>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					EmitXmlPropertyCopy(ilh, result);
					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<XElement, U>)method.CreateDelegate(typeof(Func<XElement, U>));
				}
				private static Func<XElement, U> BuildXElementCopyConstructorWithCapture()
				{
					DynamicMethod method = new DynamicMethod("XElementCopyConstructorWithCapture"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(XElement) }
						, typeof(XmlCopyHelper)
						, false
						);

					bool isDto = typeof(IDataTransferObjectSPI).IsAssignableFrom(typeof(U));
					ILHelper ilh = new ILHelper(method.GetILGenerator());

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<T>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					EmitXmlPropertyCopy(ilh, result);

					if (isDto)
					{
						MethodInfo capture = typeof(Factory<T>.Copier<U>.XmlCopyHelper)
							.GetMethod("CaptureExtraData"
								, BindingFlags.Static | BindingFlags.NonPublic
								, null
								, new Type[] { typeof(IDataTransferObjectSPI), typeof(XElement) }
								, null
								);
						ilh.LoadLocal(result);
						ilh.LoadArg_0();
						ilh.Call(capture);
					}

					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<XElement, U>)method.CreateDelegate(typeof(Func<XElement, U>));
				}

				private static void EmitXmlPropertyCopy(ILHelper ilh, LocalBuilder result)
				{
					LocalBuilder localCeq = ilh.DeclareLocal(typeof(bool));
					Label branchLabel;

					List<PropertyInfo> properties = new List<PropertyInfo>();
					foreach (var prop in typeof(U).GetWritableProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
					{
						if (prop != null && prop.GetSetMethod().IsPublic)
						{
							properties.Add(prop);
							MethodInfo tryReadNamedValue;
							LocalBuilder temp;

							if (prop.PropertyType.IsEnum)
							{
								//
								// <property-type> temp_<n>;
								// if (source.TryReadNamedValueAsEnum[<property-type>](<property-name>, out temp_<n>))
								// {
								//   result.<property-name> = temp_<n>;
								// }
								//
								tryReadNamedValue = typeof(XElementExtensions).DiscoverGenericMethodDefinitionWithArgumentCount("TryReadNamedValueAsEnum", BindingFlags.Static | BindingFlags.Public, 1);
								tryReadNamedValue = tryReadNamedValue.MakeGenericMethod(prop.PropertyType);
							}
							else if (Type.GetTypeCode(prop.PropertyType) == TypeCode.Object)
							{
								//
								// <property-type> temp_<n>;
								// if (source.TryReadNamedValue[<property-type>](<property-name>, out temp_<n>))
								// {
								//   result.<property-name> = temp_<n>;
								// }
								//
								tryReadNamedValue = typeof(XElementExtensions).DiscoverGenericMethodDefinitionWithArgumentCount("TryReadNamedValue", BindingFlags.Static | BindingFlags.Public, 1);
								tryReadNamedValue = tryReadNamedValue.MakeGenericMethod(prop.PropertyType);
							}
							else
							{
								//
								// <property-type> temp_<n>;
								// if (source.TryReadNamedValue(<property-name>, out temp_<n>))
								// {
								//   result.<property-name> = temp_<n>;
								// }
								//
								tryReadNamedValue = typeof(XElementExtensions).GetMethod("TryReadNamedValue", BindingFlags.Static | BindingFlags.Public,
									null, new Type[] { typeof(XElement), typeof(String), prop.PropertyType.MakeByRefType() }, null
									);
							}
							if (tryReadNamedValue != null)
							{
								branchLabel = ilh.DefineLabel();
								temp = ilh.DeclareLocal(prop.PropertyType);
								ilh.LoadArg_0();
								ilh.LoadValue(prop.Name);
								ilh.LoadLocalAddress(temp);
								ilh.Call(tryReadNamedValue);
								ilh.Load_I4_0();
								ilh.CompareEqual();
								ilh.StoreLocal(localCeq);
								ilh.LoadLocal(localCeq);
								ilh.BranchIfTrue_ShortForm(branchLabel);
								ilh.Nop();
								ilh.LoadLocal(result);
								ilh.LoadLocal(temp);
								ilh.CallVirtual(prop.GetSetMethod());
								ilh.Nop();
								ilh.MarkLabel(branchLabel);
							}
						}
					}
					if (__cachedProperties == null) __cachedProperties = properties.ToArray();
				}
				internal static void CaptureExtraData(IDataTransferObjectSPI instance, XElement elm)
				{
					// The attributes and elements are combined, with the attributes taking
					// precedence. What this means is that when there is a structure like the
					// following:
					//
					// <data name="kitty kitty">
					//   <name>Kitty Cat</name>
					// </data>
					//
					// The dto will have the extra data value of "kitty kitty" added under the key
					// "name" and the value from the element will be lost (not captured).
					//
					var attrs = (from a in elm.Attributes()
											 select new System.Collections.Generic.KeyValuePair<string, string>(a.Name.ToString(), a.Value)
											).Except(from a in elm.Attributes()
															 join p in __cachedProperties on a.Name equals p.Name
															 select new System.Collections.Generic.KeyValuePair<string, string>(a.Name.ToString(), a.Value));

					var elms = (from e in elm.Elements()
											select new System.Collections.Generic.KeyValuePair<string, string>(e.Name.ToString(), e.Value))
											.Except(from e in elm.Elements()
															join p in __cachedProperties on e.Name equals p.Name
															select new System.Collections.Generic.KeyValuePair<string, string>(e.Name.ToString(), e.Value))
											.Except(from a in attrs
															join e in elm.Elements() on a.Key equals e.Name
															select new System.Collections.Generic.KeyValuePair<string, string>(e.Name.ToString(), e.Value));
										
					foreach (var kvp in elms)
					{
						instance.AddExtraData<string>(kvp.Key, kvp.Value);
					}
				}
			}

			#endregion

			#region JsonCopyHelper nested class
			private static class JsonCopyHelper
			{
				private static readonly Func<JObject, U> __ctor;
				private static readonly Func<JObject, U> __ctorWithCapture;
				private static PropertyInfo[] __cachedProperties;
				private static readonly Exception __initializationException;

				internal static U Copy(JObject source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull("source", source);

					return __ctor(source);
				}

				internal static U CopyWithCapture(JObject source)
				{
					Contracts.Invariant.ThrowIfException(__initializationException, "static initializer failed");
					Contracts.Require.IsNotNull("source", source);

					return __ctorWithCapture(source);
				}

				static JsonCopyHelper()
				{
					try
					{
						__ctor = BuildJObjectCopyConstructor();
						__ctorWithCapture = BuildJObjectCopyConstructorWithCapture();
						__initializationException = null;
					}
					catch (Exception e)
					{
						__ctor = null;
						__initializationException = e;
					}
				}

				private static Func<JObject, U> BuildJObjectCopyConstructor()
				{
					DynamicMethod method = new DynamicMethod("JObjectCopyConstructor"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(JObject) }
						, typeof(JsonCopyHelper)
						, false
						);

					ILHelper ilh = new ILHelper(method.GetILGenerator());

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<T>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					EmitJPropertyCopy(ilh, result);
					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<JObject, U>)method.CreateDelegate(typeof(Func<JObject, U>));
				}
				
				private static Func<JObject, U> BuildJObjectCopyConstructorWithCapture()
				{
					DynamicMethod method = new DynamicMethod("JObjectCopyConstructorWithCapture"
						, MethodAttributes.Static | MethodAttributes.Public
						, CallingConventions.Standard
						, typeof(U)
						, new Type[] { typeof(JObject) }
						, typeof(JsonCopyHelper)
						, false
						);

					bool isDto = typeof(IDataTransferObjectSPI).IsAssignableFrom(typeof(U));
					ILHelper ilh = new ILHelper(method.GetILGenerator());

					//
					// U result = Factory<U>.CreateInstance();
					//		-- or --
					// U result = Factory<T>.CreateForSubtype<U>();
					//
					MethodInfo ufactoryCreateInstance = (typeof(U) == typeof(T))
						? typeof(Factory<U>).GetMethod("CreateInstance"
							, BindingFlags.Public | BindingFlags.Static
							, null
							, Type.EmptyTypes
							, null
							)
						: typeof(Factory<T>).DiscoverGenericMethodDefinitionWithArgumentCount("CreateForSubtype"
							, BindingFlags.Public | BindingFlags.Static
							, 1
							).MakeGenericMethod(typeof(U));

					LocalBuilder result = ilh.DeclareLocal(typeof(U));
					ilh.Call(ufactoryCreateInstance);
					ilh.StoreLocal(result);

					EmitJPropertyCopy(ilh, result);

					if (isDto)
					{
						MethodInfo capture = typeof(Factory<T>.Copier<U>.JsonCopyHelper)
							.GetMethod("CaptureExtraData"
								, BindingFlags.Static | BindingFlags.NonPublic
								, null
								, new Type[] { typeof(IDataTransferObjectSPI), typeof(JObject) }
								, null
								);
						ilh.LoadLocal(result);
						ilh.LoadArg_0();
						ilh.Call(capture);
					}

					if (InvariantContractHelper<U>.HasInvariantContract)
					{
						//
						// InvariantContractHelper<U>.InvokeContract(result);
						//
						MethodInfo invariantContract = typeof(InvariantContractHelper<U>).GetMethod("InvokeContract", BindingFlags.Static | BindingFlags.NonPublic);
						if (invariantContract != null)
						{
							ilh.LoadLocal(result);
							ilh.Call(invariantContract);
						}
					}
					//
					// return result;
					//
					ilh.LoadLocal(result);
					ilh.Return();

					// Create the delegate
					return (Func<JObject, U>)method.CreateDelegate(typeof(Func<JObject, U>));
				}

				private static void EmitJPropertyCopy(ILHelper ilh, LocalBuilder result)
				{
					LocalBuilder localCeq = ilh.DeclareLocal(typeof(bool));
					Label branchLabel;

					List<PropertyInfo> properties = new List<PropertyInfo>();
					foreach (var prop in typeof(U).GetWritableProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
					{
						if (prop != null && prop.GetSetMethod().IsPublic)
						{
							properties.Add(prop);
							MethodInfo getValue;
							LocalBuilder temp;

							if (Type.GetTypeCode(prop.PropertyType) == TypeCode.Object)
							{
								if (prop.PropertyType.IsArray)
								{ // special handling for arrays...
									if (prop.PropertyType.GetArrayRank() > 1)
										continue; // Skip properties with too many dimensions in the array

									var elmType = prop.PropertyType.GetElementType();
									if (Type.GetTypeCode(elmType) == TypeCode.Object)
									{
										getValue = typeof(JObjectExtensions)
											.DiscoverGenericMethodDefinitionWithArgumentCount("TryReadNamedValueAsArray", BindingFlags.Static | BindingFlags.Public, 1);
										getValue = getValue.MakeGenericMethod(elmType);
									}
									else
									{
										getValue = typeof(JObjectExtensions).GetMethod("TryReadNamedValueAsArray"
											, BindingFlags.Static | BindingFlags.Public
											, null, new Type[] { typeof(JObject), typeof(String), prop.PropertyType.MakeByRefType() }
											, null
											);
									}
								}
								else
								{
									//
									// <property-type> temp_<n>;
									// if (source.TryReadNamedValue[<property-type>](<property-name>, out temp_<n>))
									// {
									//   result.<property-name> = temp_<n>;
									// }
									//
									getValue = typeof(JObjectExtensions)
										.DiscoverGenericMethodDefinitionWithArgumentCount("TryReadNamedValue", BindingFlags.Static | BindingFlags.Public, 1);
									getValue = getValue.MakeGenericMethod(prop.PropertyType);
								}
							}
							else
							{
								//
								// <property-type> temp_<n>;
								// if (source.TryReadNamedValue(<property-name>, out temp_<n>))
								// {
								//   result.<property-name> = temp_<n>;
								// }
								//
								getValue = typeof(JObjectExtensions).GetMethod("TryReadNamedValue"
									, BindingFlags.Static | BindingFlags.Public
									,	null, new Type[] { typeof(JObject), typeof(String), prop.PropertyType.MakeByRefType() }
									, null
									);
							}
							if (getValue != null)
							{
								branchLabel = ilh.DefineLabel();
								temp = ilh.DeclareLocal(prop.PropertyType);
								ilh.LoadArg_0();
								ilh.LoadValue(prop.Name);
								ilh.LoadLocalAddress(temp);
								ilh.Call(getValue);
								ilh.Load_I4_0();
								ilh.CompareEqual();
								ilh.StoreLocal(localCeq);
								ilh.LoadLocal(localCeq);
								ilh.BranchIfTrue_ShortForm(branchLabel);
								ilh.Nop();
								ilh.LoadLocal(result);
								ilh.LoadLocal(temp);
								ilh.CallVirtual(prop.GetSetMethod());
								ilh.Nop();
								ilh.MarkLabel(branchLabel);
							}
						}
					}
					if (__cachedProperties == null) __cachedProperties = properties.ToArray();
				}				
				
				internal static void CaptureExtraData(IDataTransferObjectSPI instance, JObject elm)
				{
					var props = (from p in elm.Properties()
											 where !(from n in __cachedProperties
															 select n.Name).Contains(p.Name)
											 select p);
					foreach (var p in props)
					{
						switch (p.Type)
						{
							case JTokenType.Array:
								throw new NotImplementedException();
							case JTokenType.Boolean:
								instance.AddExtraData<bool>(p.Name, (bool)p.Value);
								break;
							case JTokenType.Date:
								instance.AddExtraData<DateTime>(p.Name, (DateTime)p.Value);
								break;
							case JTokenType.Float:
								instance.AddExtraData<decimal>(p.Name, (decimal)p.Value);
								break;
							case JTokenType.Integer:
								instance.AddExtraData<long>(p.Name, (long)p.Value);
								break;
							case JTokenType.Object:
								throw new NotImplementedException();								
							case JTokenType.Raw:
							case JTokenType.String:
								instance.AddExtraData<string>(p.Name, (string)p.Value);
								break;
							case JTokenType.None:
							case JTokenType.Undefined:
							case JTokenType.Null:
								instance.AddExtraData<object>(p.Name, null);
								break;
							default:
								throw new NotImplementedException();
						}
					}
				}
			}
			#endregion
		}
		#endregion

		#endregion

		#region Private members
		private static IFactory<T> ResolveFactory()
		{
			IFactory<T> result = null;
			if (__tt.IsAbstract)
			{
				if (Wireup.DefaultImplementation == null)
				{
					if (__tt.IsInterface)
					{
						Type t = null;
						foreach (Type it in __tt.DiscoverMostDerivedInterfacesByInheritance())
						{
							if (ImplementerRegistry.HasRegistration(it))
							{
								t = ImplementerRegistry.InvokeImplementorForType(it, __tt);
								if (t != null) break;
							}
						}
						if (t == null)
							throw new InvalidOperationException(String.Format("No concrete type registered for {0}", typeof(T).FullName));
						result = PerformResolveAbstractFactoryByReflectionEmit(t);
					}
				}
				else
				{
					result = PerformResolveAbstractFactoryByReflectionEmit(Wireup.DefaultImplementation);
				}
			}
			else if (__tt.IsGenericType)
			{
				result = PerformResolveFactoryForGenericTypeByReflectionEmit();
			}
			else
			{
				result = PerformResolveFactoryByReflectionEmit();
			}
			return result;
		}

		internal static FactoryStrategy<T> Strategy
		{
			get
			{
				return Util.LazyInitializeWithLock<FactoryStrategy<T>>(ref __strategy
					, __sync
					, () => ResolveFactoryStrategy()
					);
			}
		}

		private static FactoryStrategy<T> ResolveFactoryStrategy()
		{
			bool isAbstract = __tt.IsInterface || __tt.IsAbstract;

			switch (__wireup.Reuse)
			{
				case InstanceReusePolicy.Singleton:
					return new SingletonFactoryStrategy<T>();
				case InstanceReusePolicy.Cached:
					throw new NotImplementedException();
				case InstanceReusePolicy.None:
				default:
					return new DefaultFactoryStrategy<T>();
			}
		}

		private static T PerformCreateInstanceAndPopulate<U>(Func<U> ctor, Action<U> populateInstanceMethod) where U : T
		{
			Contracts.Require.IsNotNull("ctor", ctor);
			Contracts.Require.IsNotNull("populateInstanceMethod", populateInstanceMethod);

			FactoryAction action;
			T result = Strategy.PerformStrategy(ctor, out action);
			populateInstanceMethod((U)result);
			Factory<T>.NotifyInstanceAction(__tt, result, null, action);
			return result;
		}

		private static T PerformCreateInstance<U>(Func<U> ctor) where U : T
		{
			Contracts.Require.IsNotNull("ctor", ctor);
			FactoryAction action;
			T result = Strategy.PerformStrategy(ctor, out action);
			Factory<T>.NotifyInstanceAction(__tt, result, null, action);
			return result;
		}

		private static IFactory GetImplFactory(Type implType)
		{
			Contracts.Require.IsNotNull("implType", implType);
			IFactory result = null;
			Dictionary<Type, IFactory> factories = Util.LazyInitializeWithLock<Dictionary<Type, IFactory>>(ref __implFactories, __sync);
			lock (factories)
			{
				if (!factories.TryGetValue(implType, out result))
				{
					result = PerformResolveAbstractFactoryByReflectionEmit(implType);
					factories.Add(implType, result);
				}
			}
			return result;
		}

		#region CreateInstanceFindCompatibleCallOnFailure helper methods
		private static T CreateInstanceFindCompatibleCallOnFailure<A>(IFactory factory, A a)
		{
			IFactory<T, A> f = factory as IFactory<T, A>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1>(IFactory factory, A a, A1 a1)
		{
			IFactory<T, A, A1> f = factory as IFactory<T, A, A1>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2>(IFactory factory, A a, A1 a1, A2 a2)
		{
			IFactory<T, A, A1, A2> f = factory
				as IFactory<T, A, A1, A2>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3>(IFactory factory, A a, A1 a1, A2 a2, A3 a3)
		{
			IFactory<T, A, A1, A2, A3> f = factory
				as IFactory<T, A, A1, A2, A3>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			IFactory<T, A, A1, A2, A3, A4> f = factory
				as IFactory<T, A, A1, A2, A3, A4>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			IFactory<T, A, A1, A2, A3, A4, A5> f = factory
				as IFactory<T, A, A1, A2, A3, A4, A5>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4))
						&& args[6].IsAssignableFrom(typeof(A5)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4, a5 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			IFactory<T, A, A1, A2, A3, A4, A5, A6> f = factory
				as IFactory<T, A, A1, A2, A3, A4, A5, A6>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4))
						&& args[6].IsAssignableFrom(typeof(A5))
						&& args[7].IsAssignableFrom(typeof(A6)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4, a5, a6 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			IFactory<T, A, A1, A2, A3, A4, A5, A6, A7> f = factory
				as IFactory<T, A, A1, A2, A3, A4, A5, A6, A7>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4))
						&& args[6].IsAssignableFrom(typeof(A5))
						&& args[7].IsAssignableFrom(typeof(A6))
						&& args[8].IsAssignableFrom(typeof(A7)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4, a5, a6, a7 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7, A8>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8> f = factory
				as IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4))
						&& args[6].IsAssignableFrom(typeof(A5))
						&& args[7].IsAssignableFrom(typeof(A6))
						&& args[8].IsAssignableFrom(typeof(A7))
						&& args[9].IsAssignableFrom(typeof(A8)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4, a5, a6, a7, a8 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7, a8);
		}
		private static T CreateInstanceFindCompatibleCallOnFailure<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(IFactory factory, A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9> f = factory as IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>;
			if (f == null)
			{
				foreach (Type intf in factory.GetType().DiscoverGenericInterfaceImplementations(typeof(IFactory<,,,,,,,,,,>)))
				{
					Type[] args = intf.GetGenericArguments();
					if (args[1].IsAssignableFrom(typeof(A))
						&& args[2].IsAssignableFrom(typeof(A1))
						&& args[3].IsAssignableFrom(typeof(A2))
						&& args[4].IsAssignableFrom(typeof(A3))
						&& args[5].IsAssignableFrom(typeof(A4))
						&& args[6].IsAssignableFrom(typeof(A5))
						&& args[7].IsAssignableFrom(typeof(A6))
						&& args[8].IsAssignableFrom(typeof(A7))
						&& args[9].IsAssignableFrom(typeof(A8))
						&& args[10].IsAssignableFrom(typeof(A9)))
					{
						Traceable.TraceWarning(typeof(IFactory), WarnCallCompatability(intf.GetMethod("CreateInstanceWithInitialization")));
						return (T)intf.InvokeMember("CreateInstanceWithInitialization", BindingFlags.Instance | BindingFlags.Public
							, null, factory, new object[] { a, a1, a2, a3, a4, a5, a6, a7, a8, a9 });
					}
				}
				throw new FactoryException(String.Format(Properties.Resources.Warn_FactoryCallMissmatch, factory.ConcreteType.FullName));
			}
			return f.CreateInstanceWithInitialization(a, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}

		private static string WarnCallCompatability(MethodInfo method)
		{
			ParameterInfo[] parms = method.GetParameters();
			StringBuilder b = new StringBuilder(400);
			b.AppendFormat(Properties.Resources.Warn_FactoryCallMissmatch, __tt.FullName);
			b.Append(Environment.NewLine);
			b.Append(Properties.Resources.Warn_FactoryCallCastCompatability);
			b.Append(Environment.NewLine);
			b.AppendFormat("{0} {1}(", method.ReturnType.Name, method.Name);
			for (int i = 1; i < parms.Length; i++)
			{
				if (i > 1) b.AppendFormat(", {0} {1}", parms[i].ParameterType.Name, parms[i].Name);
				else b.AppendFormat("{0} {1}", parms[i].ParameterType.Name, parms[i].Name);
			}
			b.Append(")");
			return b.ToString();
		}
		#endregion

		#region Event Handling

		internal static void NotifyInstanceAction(Type requestedType, T instance, string name, FactoryAction action)
		{
			if (__instanceAction != null)
				__instanceAction(requestedType, instance, name, action);
		}
		private static void AttachEventHandlersForTypeFactory(Type t)
		{
			Type factoryType = typeof(Factory<>).MakeGenericType(t);
			Factory<T>.InstanceAction += ((FactoryInstanceEvent<T>)
				Delegate.CreateDelegate(typeof(FactoryInstanceEvent<T>)
				, factoryType.GetMethod("NotifyInstanceAction", BindingFlags.NonPublic | BindingFlags.Static))
				);
		}

		#endregion

		#region Factory Emit		
		private static IFactory<T> PerformResolveFactoryByReflectionEmit()
		{
			Assembly targetAsm = __tt.Assembly;
			string typeName = EmittedAssembly.FormatEmittedTypeName(__tt, "$Factory");			
			
			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(
							MakeEmittedAssemblyNameFromAssembly(EmittedAssemblyNameFormat, targetAsm)
							, (emittedAsm) => GenerateImplementationsForAssembly(emittedAsm, targetAsm)
							);
			Type ft = asm.GetType(typeName, true, false);
			return (ft != null) ? (IFactory<T>)Activator.CreateInstance(ft) : null;
		}
		private static IFactory<T> PerformResolveAbstractFactoryByReflectionEmit(Type uu)
		{
			Assembly targetAsm = __tt.Assembly;
			string typeName = EmittedAssembly.FormatEmittedTypeName(__tt, "$Factory");

			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(
					MakeEmittedAssemblyNameFromAssemblyForImplType(__tt, uu)
					, (emittedAsm) => GenerateAbstractFactoryImplementation(emittedAsm, typeName, __tt, uu)
					);
			Type ft = asm.GetType(typeName, true, false);
			return (ft != null) ? (IFactory<T>)Activator.CreateInstance(ft) : null;
		}
		private static IFactory<T> PerformResolveFactoryForGenericTypeByReflectionEmit()
		{
			Assembly targetAsm = __tt.Assembly;
			string typeName = EmittedAssembly.FormatEmittedTypeName(__tt, "$Factory");

			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(
					MakeEmittedAssemblyNameFromAssemblyForGenericType(__tt)
					, (emittedAsm) => GenerateImplementation(emittedAsm, __tt)
					);
			Type ft = asm.GetType(typeName, true, false);
			return (ft != null) ? (IFactory<T>)Activator.CreateInstance(ft) : null;
		}		
		private static void GenerateAbstractFactoryImplementation(EmittedAssembly asm, string typeName, Type tt, Type uu)
		{
			ConstructorInfo[] cc = uu.GetConstructors();
			Type baseFactoryType = typeof(AbstractFactory<,>).MakeGenericType(tt, uu);
			EmittedClass ctor = asm.DefineClass(EmittedAssembly.FormatEmittedTypeName(tt, "$Factory"), EmittedClass.DefaultTypeAttributes
				, baseFactoryType
				, Type.EmptyTypes
				);
			ctor.DefineDefaultCtor();

			GenerateFactoryImplementationsForConstructors(cc, tt, baseFactoryType, ctor);
		}		
		private static void GenerateImplementationsForAssembly(EmittedAssembly asm, Assembly targetAsm)
		{
			Type[] types = targetAsm.GetTypes();
			var concreteTypes = from t in types
													where t.IsPublic && t.IsClass
														&& t.IsGenericTypeDefinition == false
														&& t.IsAbstract == false
														&& t.IsArray == false
														&& t.IsPointer == false
														&& t.IsPrimitive == false
														&& t.IsEnum == false
														&& t.IsCOMObject == false
														&& t.IsDefined(typeof(FactoryIgnoreAttribute), false) == false
													select t;

			foreach (Type tt in concreteTypes)
			{
				GenerateImplementationsForType(asm, tt);
			}
		}
		private static void GenerateImplementationsForType(EmittedAssembly asm, Type tt)
		{
			GenerateImplementation(asm, tt);
			Type[] types = tt.GetNestedTypes();
			if (types.Length > 0)
			{
				var concreteTypes = from t in types
														where t.IsNestedPublic && t.IsClass
															&& t.IsGenericTypeDefinition == false 
															&& t.IsAbstract == false
															&& t.IsArray == false
															&& t.IsPointer == false
															&& t.IsPrimitive == false
															&& t.IsEnum == false
															&& t.IsCOMObject == false
															&& t.IsDefined(typeof(FactoryIgnoreAttribute), false) == false
														select t;

				foreach (Type ct in concreteTypes)
				{
					GenerateImplementationsForType(asm, ct);
				}
			}
		}
		private static void GenerateImplementation(EmittedAssembly asm, Type tt)
		{
			ConstructorInfo[] cc = tt.GetConstructors();
			Type baseFactoryType = typeof(ClassFactory<>).MakeGenericType(tt);
			EmittedClass ctor = asm.DefineClass(EmittedAssembly.FormatEmittedTypeName(tt, "$Factory"), EmittedClass.DefaultTypeAttributes,
				baseFactoryType, Type.EmptyTypes);
			ctor.DefineDefaultCtor();

			GenerateFactoryImplementationsForConstructors(cc, tt, baseFactoryType, ctor);
		}
		private static void GenerateFactoryImplementationsForConstructors(ConstructorInfo[] cc
			, Type tt
			, Type baseFactoryType
			, EmittedClass ctor)
		{

			EmittedMethod em;
			MethodInfo perform;
			foreach (ConstructorInfo c in cc)
			{
				bool skip = false;
				ParameterInfo[] parms = c.GetParameters();
				foreach (ParameterInfo p in parms)
				{
					if ((p.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out
						|| (p.Attributes & ParameterAttributes.Retval) == ParameterAttributes.Retval)
					{
						Traceable.TraceData(typeof(IFactory<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors with parameters marked as an [out] parameter: {0}", tt.FullName));
						skip = true;
						break;
					}
					if (p.ParameterType.IsArray || p.ParameterType.IsPointer)
					{
						Traceable.TraceData(typeof(IFactory<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors with array or pointer parameters: {0}", tt.FullName));
						skip = true;
						break;
					}
				}
				if (skip) continue;
				switch (parms.Length)
				{
					case 0: /* fall through, the method is implemented in the base class */ break;
					case 1:
						ctor.AddInterfaceImplementation(typeof(IFactory<,>)
							.MakeGenericType(tt, parms[0].ParameterType));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.Call(perform);
						});
						break;
					case 2:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,>)
							.MakeGenericType(tt
								, parms[0].ParameterType
								, parms[1].ParameterType
								));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.Call(perform);
						});
						break;
					case 3:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,>)
							.MakeGenericType(tt
								, parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.Call(perform);
						});
						break;
					case 4:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.Call(perform);
						});
						break;
					case 5:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								, parms[4].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.Call(perform);
						});
						break;
					case 6:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineParameter(parms[5].Name, parms[5].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								, parms[4].ParameterType
								, parms[5].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.Call(perform);
						});
						break;
					case 7:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							, parms[6].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineParameter(parms[5].Name, parms[5].ParameterType);
						em.DefineParameter(parms[6].Name, parms[6].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								, parms[4].ParameterType
								, parms[5].ParameterType
								, parms[6].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.Call(perform);
						});
						break;
					case 8:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							, parms[6].ParameterType
							, parms[7].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineParameter(parms[5].Name, parms[5].ParameterType);
						em.DefineParameter(parms[6].Name, parms[6].ParameterType);
						em.DefineParameter(parms[7].Name, parms[7].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								, parms[4].ParameterType
								, parms[5].ParameterType
								, parms[6].ParameterType
								, parms[7].ParameterType
						);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.LoadArg_ShortForm(8);
							il.Call(perform);
						});
						break;
					case 9:
						ctor.AddInterfaceImplementation(typeof(IFactory<,,,,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							, parms[6].ParameterType
							, parms[7].ParameterType
							, parms[8].ParameterType
							));
						em = ctor.DefineMethod(EmittedCreateInstanceMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineParameter(parms[5].Name, parms[5].ParameterType);
						em.DefineParameter(parms[6].Name, parms[6].ParameterType);
						em.DefineParameter(parms[7].Name, parms[7].ParameterType);
						em.DefineParameter(parms[8].Name, parms[8].ParameterType);
						perform = baseFactoryType.DiscoverGenericMethodDefinitionWithArgumentCount(EmittedPerformCreateInstanceMethodName
							, BindingFlags.NonPublic | BindingFlags.Instance
							, parms.Length).MakeGenericMethod(parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								, parms[3].ParameterType
								, parms[4].ParameterType
								, parms[5].ParameterType
								, parms[6].ParameterType
								, parms[7].ParameterType
								, parms[8].ParameterType
								);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_0();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.LoadArg_ShortForm(8);
							il.LoadArg_ShortForm(9);
							il.Call(perform);
							il.Return();
						});
						break;
					default:
						Traceable.TraceData(typeof(IFactory<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors having more than 9 arguments: {0}", tt.FullName));
						break;
				}
			}
		}
		
		private static AssemblyName MakeEmittedAssemblyNameFromAssemblyForImplType(Type tt, Type ii)
		{
			AssemblyName tasmName = tt.Assembly.GetName();
			AssemblyName asmName = new AssemblyName(String.Format("{0}v{1}$FactoryImpl$",
				tt,
				tasmName.Version,
				typeof(Factory).Assembly.GetName().Version,
				ii.Name
				));
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}

		private static AssemblyName MakeEmittedAssemblyNameFromAssembly(string anf, Assembly tasm)
		{
			AssemblyName tasmName = tasm.GetName();
			AssemblyName asmName = new AssemblyName(String.Format(anf
					, tasmName.Name
					, tasmName.Version
					, typeof(Factory).Assembly.GetName().Version).Replace('.', '_')
					);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}
		private static AssemblyName MakeEmittedAssemblyNameFromAssemblyForGenericType(Type tt)
		{
			AssemblyName tasmName = tt.Assembly.GetName();
			AssemblyName asmName = new AssemblyName(String.Format("{0}v{1}__Factory_{2}"
					, EmittedAssembly.MangleTypeName(tt)
					, tasmName.Version
					, typeof(Ctor<T>).Assembly.GetName().Version)
				);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}
		#endregion

		#endregion

		#region FactoryWireup nested class
		public class FactoryWireup
		{
			internal List<Type> Implementers = new List<Type>();
			internal InstanceReusePolicy Reuse = InstanceReusePolicy.None;
			internal SingletonReuseScope SingletonScope = SingletonReuseScope.LocalSharedByAll;
			internal Type DefaultImplementation = null;

			public Factory<T>.FactoryWireup SetInstanceReusePolicy(InstanceReusePolicy instanceReuse)
			{
				this.Reuse = instanceReuse;
				return this;
			}

			public Factory<T>.FactoryWireup WithSingletonScope(SingletonReuseScope singletonReuseScope)
			{
				this.SingletonScope = singletonReuseScope;
				return this;
			}

			public Factory<T>.FactoryWireup SetDefaultImplementation<U>()
				where U : T
			{
				DefaultImplementation = typeof(U);
				return this;
			}

			public Factory<T>.FactoryWireup SetDefaultImplementation(Type implType)
			{
				Require.IsAssignableFrom<T>("implType", implType);

				DefaultImplementation = implType;
				return this;
			}


			public Factory<T>.FactoryWireup AddImplemeter<TImplementer>()
				where TImplementer : IImplementer
			{
				this.Implementers.Add(typeof(TImplementer));
				return this;
			}
		}
		#endregion
	}
}
