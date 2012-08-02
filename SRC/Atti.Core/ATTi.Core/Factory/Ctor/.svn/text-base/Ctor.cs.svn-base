using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using ATTi.Core.Reflection;
using ATTi.Core.Reflection.Emit;
using ATTi.Core.Trace;

namespace ATTi.Core.Factory.Ctor
{
	/// <summary>
	/// Utility class for compiling ICtor<> wrappers for target type T.
	/// </summary>
	/// <typeparam name="T">The target type.</typeparam>
	internal static class Ctor<T>
	{
		private readonly static Type __tt = typeof(T);
		private readonly static string EmittedAssemblyNameFormat = "{0}v{1}__Ctors_{2}";
		private readonly static string EmittedConstructorMethodName = "Construct";
		private static Object __sync = typeof(T).Assembly.GetSafeLock();
		private static ICtor<T> __ctor;
		private static Dictionary<Type, Type> __contravariants = new Dictionary<Type, Type>();

		/// <summary>
		/// Accesses the singleton instance of the emitted ICtor wrapper for target type T.
		/// </summary>
		public static ICtor<T> Singleton
		{
			get
			{
				return Util.LazyInitializeWithLock<ICtor<T>>(ref __ctor, __sync, () => ResolveCtor());
			}
		}

		private static ICtor<T> ResolveCtor()
		{
			if (__tt.IsGenericType) return ResolveCtorForGenericType();
			
			Assembly targetAsm = __tt.Assembly;
			AssemblyName asmName = MakeEmittedAssemblyNameFromAssembly(targetAsm);

			string typeName = EmittedAssembly.FormatEmittedTypeName(__tt, "$Ctor");

			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(asmName,
				(emittedAsm) =>
				{
					GenerateImplementationsForAssembly(emittedAsm, targetAsm);
				});

			Type ct = asm.GetType(typeName, true, false);
			return (ct != null) ? (ICtor<T>)Activator.CreateInstance(ct) : null;
		}

		private static ICtor<T> ResolveCtorForGenericType()
		{
			Assembly targetAsm = __tt.Assembly;
			AssemblyName asmName = MakeEmittedAssemblyNameFromAssemblyForGenericType(__tt);

			string typeName = EmittedAssembly.FormatEmittedTypeName(__tt, "$Ctor");

			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(asmName,
				(emittedAsm) =>
				{
					GenerateImplementationsForGenericType(emittedAsm, __tt);
				});

			Type ct = asm.GetType(typeName, true, false);
			return (ct != null) ? (ICtor<T>)Activator.CreateInstance(ct) : null;
		}
		private static AssemblyName MakeEmittedAssemblyNameFromAssemblyForGenericType(Type tt)
		{
			AssemblyName tasmName = tt.Assembly.GetName();
			AssemblyName asmName = new AssemblyName(String.Format("{0}v{1}__Ctor_{2}"
					, EmittedAssembly.MangleTypeName(tt)
					, tasmName.Version
					, typeof(Ctor<T>).Assembly.GetName().Version)
				);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}
		private static AssemblyName MakeEmittedAssemblyNameFromAssembly(Assembly tasm)
		{
			AssemblyName tasmName = tasm.GetName();
			AssemblyName asmName = new AssemblyName(String.Format(
					EmittedAssemblyNameFormat
					, tasmName.Name
					, tasmName.Version
					, typeof(Ctor<T>).Assembly.GetName().Version).Replace('.', '_')
					);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}

		internal static ICtor<T, A> AssertContravariantWrapper<A>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A>), out ct))
				{
					Type ii = __tt.DiscoverFirstCompatibleGenericInterfaceImplementation(typeof(ICtor<,>)
						, typeof(T)
						, typeof(A));
					if (ii != null)
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						ct = typeof(ContravariantCtor<,,>).MakeGenericType(__tt
							, typeof(A)
							, genericArgTypes[1]);
						__contravariants.Add(typeof(ICtor<T, A>), ct);
					}
					else
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1> AssertContravariantWrapper<A, A1>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							)
						{
							ct = typeof(ContravariantCtor<,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2> AssertContravariantWrapper<A, A1, A2>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3> AssertContravariantWrapper<A, A1, A2, A3>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4> AssertContravariantWrapper<A, A1, A2, A3, A4>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4, A5> AssertContravariantWrapper<A, A1, A2, A3, A4, A5>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4, A5>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							&& genericArgTypes[6].IsAssignableFrom(typeof(A5))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								, typeof(A5), genericArgTypes[6]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4, A5>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4, A5>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4, A5, A6> AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							&& genericArgTypes[6].IsAssignableFrom(typeof(A5))
							&& genericArgTypes[7].IsAssignableFrom(typeof(A6))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								, typeof(A5), genericArgTypes[6]
								, typeof(A6), genericArgTypes[7]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4, A5, A6>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4, A5, A6, A7> AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							&& genericArgTypes[6].IsAssignableFrom(typeof(A5))
							&& genericArgTypes[7].IsAssignableFrom(typeof(A6))
							&& genericArgTypes[8].IsAssignableFrom(typeof(A7))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								, typeof(A5), genericArgTypes[6]
								, typeof(A6), genericArgTypes[7]
								, typeof(A7), genericArgTypes[8]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4, A5, A6, A7>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8> AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7, A8>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							&& genericArgTypes[6].IsAssignableFrom(typeof(A5))
							&& genericArgTypes[7].IsAssignableFrom(typeof(A6))
							&& genericArgTypes[8].IsAssignableFrom(typeof(A7))
							&& genericArgTypes[9].IsAssignableFrom(typeof(A8))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								, typeof(A5), genericArgTypes[6]
								, typeof(A6), genericArgTypes[7]
								, typeof(A7), genericArgTypes[8]
								, typeof(A8), genericArgTypes[9]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8>)Activator.CreateInstance(ct, ctor);
		}
		internal static ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9> AssertContravariantWrapper<A, A1, A2, A3, A4, A5, A6, A7, A8, A9>(object ctor)
		{
			Type ct;
			lock (__contravariants)
			{
				if (!__contravariants.TryGetValue(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>), out ct))
				{
					foreach (Type ii in __tt.DiscoverGenericInterfaceImplementations(typeof(ICtor<,,,,,,,,,,>)))
					{
						Type[] genericArgTypes = ii.GetGenericArguments();
						if (__tt.IsAssignableFrom(genericArgTypes[0])
							&& genericArgTypes[1].IsAssignableFrom(typeof(A))
							&& genericArgTypes[2].IsAssignableFrom(typeof(A1))
							&& genericArgTypes[3].IsAssignableFrom(typeof(A2))
							&& genericArgTypes[4].IsAssignableFrom(typeof(A3))
							&& genericArgTypes[5].IsAssignableFrom(typeof(A4))
							&& genericArgTypes[6].IsAssignableFrom(typeof(A5))
							&& genericArgTypes[7].IsAssignableFrom(typeof(A6))
							&& genericArgTypes[8].IsAssignableFrom(typeof(A7))
							&& genericArgTypes[9].IsAssignableFrom(typeof(A8))
							&& genericArgTypes[10].IsAssignableFrom(typeof(A9))
							)
						{
							ct = typeof(ContravariantCtor<,,,,,,,,,,,,,,,,,,,,>).MakeGenericType(__tt
								, typeof(A), genericArgTypes[1]
								, typeof(A1), genericArgTypes[2]
								, typeof(A2), genericArgTypes[3]
								, typeof(A3), genericArgTypes[4]
								, typeof(A4), genericArgTypes[5]
								, typeof(A5), genericArgTypes[6]
								, typeof(A6), genericArgTypes[7]
								, typeof(A7), genericArgTypes[8]
								, typeof(A8), genericArgTypes[9]
								, typeof(A9), genericArgTypes[10]
								);
							__contravariants.Add(typeof(ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>), ct);

							break;
						}
					}
					if (ct == null)
					{
						StringBuilder signatureMessage = new StringBuilder(400);
						signatureMessage.Append("Constructor ").Append(__tt.FullName).Append("(")
							.Append(typeof(A).FullName).Append(" unamed) could not be found, use one of the following:");
						AppendSupportedSignaturesStatement(signatureMessage, ctor);
						throw new MissingMethodException(signatureMessage.ToString());
					}
				}
			}
			return (ICtor<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9>)Activator.CreateInstance(ct, ctor);
		}

		private static void AppendSupportedSignaturesStatement(StringBuilder signatureMessage, object wrapper)
		{
		}

		private static void GenerateImplementationsForAssembly(EmittedAssembly asm, Assembly targetAsm)
		{
			Type[] types = targetAsm.GetTypes();
			var concreteTypes = from t in types
													where t.IsPublic && t.IsClass
														&& typeof(Delegate).IsAssignableFrom(t) == false
														&& t.IsAbstract == false
														&& t.IsArray == false
														&& t.IsPointer == false
														&& t.IsPrimitive == false
														&& t.IsEnum == false
														&& t.IsCOMObject == false
														&& t.IsGenericTypeDefinition == false
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
															&& typeof(Delegate).IsAssignableFrom(t) == false
															&& t.IsAbstract == false
															&& t.IsArray == false
															&& t.IsPointer == false
															&& t.IsPrimitive == false
															&& t.IsEnum == false
															&& t.IsCOMObject == false
															&& t.IsGenericTypeDefinition == false
														select t;

				foreach (Type ct in concreteTypes)
				{
					GenerateImplementationsForType(asm, ct);
				}
			}
		}

		private static void GenerateImplementationsForGenericType(EmittedAssembly asm, Type tt)
		{
			GenerateImplementation(asm, tt);
		}

		private static void GenerateImplementation(EmittedAssembly asm, Type tt)
		{
			ConstructorInfo d = tt.GetConstructor(Type.EmptyTypes);
			ConstructorInfo[] cc = tt.GetConstructors();
			EmittedClass ctor = asm.DefineClass(EmittedAssembly.FormatEmittedTypeName(tt, "$Ctor"), EmittedClass.DefaultTypeAttributes,
				null, new Type[] { typeof(ICtor<>).MakeGenericType(tt) });
			ctor.DefineDefaultCtor();
			EmittedMethod em = ctor.DefineMethod(EmittedConstructorMethodName);
			em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
			em.ReturnType = new TypeRef(tt);
			if (d != null)
			{ // Call the default constructor
				em.DefineMethodBody((m, il) =>
				{
					il.Nop();
					il.NewObj(d);
				});
			}
			else
			{ // Throw not implemented exception
				em.DefineMethodBody((m, il) =>
				{
					il.Nop();
					il.LoadValue(String.Format("Type does not expose a default constructor: {0}", tt.FullName));
					il.NewObj(typeof(NotImplementedException).GetConstructor(new Type[] { typeof(String) }));
					il.Throw(typeof(NotImplementedException));
				});
			}

			foreach (ConstructorInfo c in cc)
			{
				bool skip = false;
				ParameterInfo[] parms = c.GetParameters();
				foreach (ParameterInfo p in parms)
				{
					if ((p.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out
						|| (p.Attributes & ParameterAttributes.Retval) == ParameterAttributes.Retval)
					{
						Traceable.TraceData(typeof(Ctor<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors with parameters marked as an [out] parameter: {0}", tt.FullName));
						skip = true;
						break;
					}
					if (p.ParameterType.IsArray || p.ParameterType.IsPointer)
					{
						Traceable.TraceData(typeof(Ctor<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors with array or pointer parameters: {0}", tt.FullName));
						skip = true;
						break;
					}
				}
				if (skip) continue;
				switch (parms.Length)
				{
					case 0: /* fall through, we've already got a matching ctor */ break;
					case 1:
						ctor.AddInterfaceImplementation(typeof(ICtor<,>)
							.MakeGenericType(tt, parms[0].ParameterType));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineMethodBody((m, il) =>
							{
								il.Nop();
								il.LoadArg_1();
								il.NewObj(c);
							});
						break;
					case 2:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,>)
							.MakeGenericType(tt
								, parms[0].ParameterType
								, parms[1].ParameterType
								));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.NewObj(c);
						});
						break;
					case 3:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,>)
							.MakeGenericType(tt
								, parms[0].ParameterType
								, parms[1].ParameterType
								, parms[2].ParameterType
								));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.NewObj(c);
						});
						break;
					case 4:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.NewObj(c);
						});
						break;
					case 5:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.NewObj(c);
						});
						break;
					case 6:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
						em.ClearAttributes();
						em.IncludeAttributes(EmittedMethod.PublicInterfaceImplementationAttributes);
						em.ReturnType = new TypeRef(tt);
						em.DefineParameter(parms[0].Name, parms[0].ParameterType);
						em.DefineParameter(parms[1].Name, parms[1].ParameterType);
						em.DefineParameter(parms[2].Name, parms[2].ParameterType);
						em.DefineParameter(parms[3].Name, parms[3].ParameterType);
						em.DefineParameter(parms[4].Name, parms[4].ParameterType);
						em.DefineParameter(parms[5].Name, parms[5].ParameterType);
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.NewObj(c);
						});
						break;
					case 7:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							, parms[6].ParameterType
							));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
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
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.NewObj(c);
						});
						break;
					case 8:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,,,,,>).MakeGenericType(tt
							, parms[0].ParameterType
							, parms[1].ParameterType
							, parms[2].ParameterType
							, parms[3].ParameterType
							, parms[4].ParameterType
							, parms[5].ParameterType
							, parms[6].ParameterType
							, parms[7].ParameterType
							));
						em = ctor.DefineMethod(EmittedConstructorMethodName);
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
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.LoadArg_ShortForm(8);
							il.NewObj(c);
						});
						break;
					case 9:
						ctor.AddInterfaceImplementation(typeof(ICtor<,,,,,,,,,>).MakeGenericType(tt
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
						em = ctor.DefineMethod(EmittedConstructorMethodName);
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
						em.DefineMethodBody((m, il) =>
						{
							il.Nop();
							il.LoadArg_1();
							il.LoadArg_2();
							il.LoadArg_3();
							il.LoadArg_ShortForm(4);
							il.LoadArg_ShortForm(5);
							il.LoadArg_ShortForm(6);
							il.LoadArg_ShortForm(7);
							il.LoadArg_ShortForm(8);
							il.LoadArg_ShortForm(9);
							il.NewObj(c);
						});
						break;
					default:
						Traceable.TraceData(typeof(Ctor<T>), TraceEventType.Warning
							, String.Format("Cannot create factory wrapper for constructors having more than 9 arguments: {0}", tt.FullName));
						break;
				}
			}
		}
	}
}
