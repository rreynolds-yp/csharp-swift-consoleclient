using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ATTi.Core.Configuration;
using ATTi.Core.Factory;
using ATTi.Core.Reflection;
using ATTi.Core.Reflection.Emit;

namespace ATTi.Core.Mementos
{
	internal delegate void MementoRestoreDelegate<T>(IMementoContext ctx, ref T item, IMemento m);
		
	/// <summary>
	/// Utility class for capturing/restoring mementos.
	/// </summary>
	public static class Memento
	{
		/// <summary>
		/// Marker object, represents a null reference in the memento-object graph.
		/// </summary>
		public sealed class NullMemento : IMemento
		{
			internal NullMemento() { }
			public object Target { get { return null; } }
			public bool IsRestored { get; set; }
			public override bool Equals(object obj)
			{
				return obj is NullMemento;
			}
			public override int GetHashCode()
			{
				return "null".GetHashCode();
			}
			public override string ToString()
			{
				return "null";
			}
		}

		/// <summary>
		/// Placeholder memento, placed in the context to avoid recursion.
		/// </summary>
		private class MementoPlaceholder : IMemento
		{
			object _target;
			IMemento _inner;
			internal MementoPlaceholder(object t)
			{
				_target = t;
				_inner = null;
			}
			public object Target { get { return _target; } }
			public bool IsRestored { get { return _inner.IsRestored; } set { _inner.IsRestored = true; } }
			internal IMemento Inner { get { return _inner; } set { _inner = value; } }
		}
		private struct PrimitiveArrayMemento<T> : IMemento
		{
			T[] _target;
			T[] _copy;
			bool _isRestored;

			internal PrimitiveArrayMemento(T[] target)
			{
				_target = target;
				_copy = new T[target.Length];
				Array.Copy(_target, _copy, _target.Length);
				_isRestored = false;
			}

			public object Target { get { return _target; } }
			public bool IsRestored
			{
				get { return _isRestored; }
				set { _isRestored = value; }
			}

			internal void DoRestore(ref T[] array)
			{
				Array.Copy(_copy, _target, _copy.Length);
			}
		}
		private struct ArrayMemento<T> : IMemento
		{
			T[] _target;
			IMemento[] _copy;
			bool _isRestored;

			internal ArrayMemento(IMementoContext ctx, T[] target)
			{
				_target = target;
				_copy = new IMemento[target.Length];
				for (int i = 0; i < _target.Length; i++)
				{
					_copy[i] = CaptureMemento<T>(ctx, _target[i]);
				}
				_isRestored = false;
			}

			public object Target { get { return _target; } }
			public bool IsRestored
			{
				get { return _isRestored; }
				set { _isRestored = value; }
			}

			internal void DoRestore(IMementoContext ctx, ref T[] array)
			{
				for (int i = 0; i < _copy.Length; i++)
				{
					T item = (T)_copy[i].Target;
					RestoreMemento<T>(ctx, ref item, _copy[i]);
					_target[i] = item;
				}
			}
		}

		public static readonly IMemento NullMarker = new NullMemento();

		private static MethodInfo __genericCapture = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("CaptureMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericRestore = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("RestoreMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericCaptureBase = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("CaptureBaseMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericRestoreBase = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("RestoreBaseMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericCaptureArray = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("CaptureArrayMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericRestoreArray = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("RestoreArrayMemento", BindingFlags.Public | BindingFlags.Static, 1);
		private static MethodInfo __genericCaptureSubclass = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("CaptureSubclass", BindingFlags.NonPublic | BindingFlags.Static, 2);
		private static MethodInfo __genericRestoreSubclass = typeof(Memento).DiscoverGenericMethodDefinitionWithArgumentCount("RestoreSubclass", BindingFlags.NonPublic | BindingFlags.Static, 2);

		private struct TypeRec
		{
			public Type TargetType;
			public Dictionary<Type, ImplementationRec> Implementations;

			public TypeRec(Type t)
			{
				TargetType = t;
				Implementations = new Dictionary<Type, ImplementationRec>();
			}
			public void GetImplementationRec(Type impl, out ImplementationRec rec)
			{
				Contracts.Require.IsAssignableFrom(TargetType, "impl", impl);
				lock (Implementations)
				{
					if (!Implementations.TryGetValue(impl, out rec))
					{
						Implementations.Add(impl, rec = new ImplementationRec(TargetType, impl));
					}
				}
			}
		}
		private struct ImplementationRec
		{
			public Delegate CaptureSubclass;
			public Delegate RestoreSubclass;

			public ImplementationRec(Type target, Type impl)
			{
				CaptureSubclass = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(IMementoContext), target, typeof(IMemento))
								, __genericCaptureSubclass.MakeGenericMethod(target, impl)
								);
				RestoreSubclass = Delegate.CreateDelegate(typeof(MementoRestoreDelegate<>).MakeGenericType(target)
								, __genericRestoreSubclass.MakeGenericMethod(target, impl)
								);
			}
		}

		private static Dictionary<Type, TypeRec> __types = new Dictionary<Type, TypeRec>();

		private static TypeRec GetTypeRec(Type t)
		{
			TypeRec result;
			lock (__types)
			{
				if (!__types.TryGetValue(t, out result))
				{
					__types.Add(t, result = new TypeRec(t));
				}
			}
			return result;
		}

		public static IMemento CaptureMemento<T>(IMementoContext ctx, T item)
		{
			if (Object.Equals(default(T), item)) return NullMarker;

			IMemento result;
			if (ctx != null)
			{
				if (ctx.TryGetMemento(item, out result))
				{
					return result;
				}
				else
				{
					MementoPlaceholder p = new MementoPlaceholder(item);
					ctx.PutMemento(item, p);
					p.Inner = DoCapture(ctx, item);
					return p;
				}
			}
			return DoCapture(ctx, item);
		}

		private static IMemento DoCapture<T>(IMementoContext ctx, T item)
		{
			if (typeof(T) == item.GetType())
			{
				return M<T>.CaptureMemento(ctx, item);
			}
			else
			{
				return CallCaptureSubtype<T>(ctx, item);
			}
		}

		private static IMemento CallCaptureSubtype<T>(IMementoContext ctx, T item)
		{
			TypeRec trec = GetTypeRec(typeof(T));
			ImplementationRec rec;
			trec.GetImplementationRec(item.GetType(), out rec);
			return (rec.CaptureSubclass as Func<IMementoContext, T, IMemento>)(ctx, item);
		}

		public static IMemento CaptureBaseMemento<T>(IMementoContext ctx, T item)
		{
			if (Object.Equals(default(T), item)) return NullMarker;
			return M<T>.CaptureBaseMemento(ctx, item);
		}

		public static void RestoreBaseMemento<T>(IMementoContext ctx, ref T item, IMemento m)
		{
			Contracts.Require.IsNotNull("m", m);
			if (Object.Equals(m, NullMarker)) item = default(T);
			else M<T>.RestoreBaseMemento(ctx, ref item, m);
		}

		public static void RestoreMemento<T>(IMementoContext ctx, ref T item, IMemento m)
		{
			Contracts.Require.IsNotNull("m", m);
			if (Object.Equals(m, NullMarker)) item = default(T);
			else
			{
				if (!Object.ReferenceEquals(item, m.Target))
				{
					item = (T)m.Target;
				}
				// Avoid cycles, set to restored before recursion begins.
				if (m.IsRestored) return;
				m.IsRestored = true;

				Type i = item.GetType();
				if (typeof(T) != i)
				{
					ImplementationRec rec;
					GetTypeRec(typeof(T)).GetImplementationRec(i, out rec);
					(rec.RestoreSubclass as MementoRestoreDelegate<T>)(ctx, ref item,
						(m is MementoPlaceholder) ? (m as MementoPlaceholder).Inner : m
						);
					return;
				}

				M<T>.RestoreMemento(ctx, ref item, (m is MementoPlaceholder) ? (m as MementoPlaceholder).Inner : m);
			}
		}

		private static IMemento CaptureSubclass<T, S>(IMementoContext ctx, T item)
			where S : T
		{
			return M<S>.CaptureMemento(ctx, (S)item);
		}

		private static void RestoreSubclass<T, S>(IMementoContext ctx, ref T item, IMemento m)
			where S : T
		{
			S sub = (S)item;
			M<S>.RestoreMemento(ctx, ref sub, m);
		}

		public static IMemento CaptureArrayMemento<T>(IMementoContext ctx, T[] array)
		{
			if (Object.Equals(default(T), array)) return NullMarker;

			IMemento result;
			if (ctx != null)
			{
				if (ctx.TryGetMemento(array, out result))
				{
					return result;
				}
				else
				{
					if (typeof(T).IsPrimitive)
					{
						result = new PrimitiveArrayMemento<T>(array);
					}
					else
					{
						result = new ArrayMemento<T>(ctx, array);
					}

					return ctx.PutMemento(array, result);
				}
			}
			return null;
		}

		public static void RestoreArrayMemento<T>(IMementoContext ctx, ref T[] array, IMemento m)
		{
			Contracts.Require.IsNotNull("m", m);
			if (Object.Equals(m, NullMarker)) array = null;
			else
			{
				if (!Object.ReferenceEquals(array, m.Target))
				{
					array = (T[])m.Target;
				}
				// Avoid cycles, set to restored before recursion begins.
				if (m.IsRestored) return;
				m.IsRestored = true;

				if (typeof(T).IsPrimitive)
				{
					((PrimitiveArrayMemento<T>)m).DoRestore(ref array);
				}
				else
				{
					((ArrayMemento<T>)m).DoRestore(ctx, ref array);
				}
			}
		}

		public static class Helper<T>
		{
			private static Type __helperType;

			public static void RegisterHelper<H>()
				where H : class, IMementoHelper<T>, new()
			{
				Contracts.Invariant.IsNull("HelperType", __helperType);
				__helperType = typeof(H);
			}
			public static void RegisterHelper(Type helperType)
			{
				Contracts.Require.IsAssignableFrom<IMementoHelper<T>>("helperType", helperType);
				Contracts.Invariant.IsNull("HelperType", __helperType);
				__helperType = helperType;
			}
			internal static IMementoHelper<T> Instance
			{
				get
				{
					return (__helperType != null) 
						? Factory<IMementoHelper<T>>.CreateImplInstance(__helperType)
						: DiscoverHelperType();
				}
			}

			private static IMementoHelper<T> DiscoverHelperType()
			{
				MementosConfigurationSection cnfg = ConfigurationUtil
					.EnsureSectionOrDefault<MementosConfigurationSection>(MementosConfigurationSection.SectionName);
				if (cnfg != null)
				{
					MementoConfigurationElement elm = cnfg.Helpers[typeof(T)];
					if (elm != null)
					{
						return Factory<IMementoHelper<T>>.CreateImplInstance(elm.ResolvedHelperType);
					}
					else if (typeof(T).IsGenericType)
					{
						// The source type is generic, see if there is a helper assigned to the type family...
						Type gtype = typeof(T).GetGenericTypeDefinition();
						elm = cnfg.Helpers[gtype];
						if (elm != null)
						{ // helper is configured, mirror type args from T to helper type...
							Type htype = elm.ResolvedHelperType.MakeGenericType(typeof(T).GetGenericArguments());
							return Factory<IMementoHelper<T>>.CreateImplInstance(htype);
						}
					}
				}

				return null; // No helper configured.
			}
		}

		private static class M<T>
		{
			readonly static string EmittedAssemblyNameFormat = "{0}v{1}__Memento_{2}";
			static Object __sync = new Object();
			static Type __mementoType;
			static Func<IMementoContext, T, IMemento> __capture;
			static MementoRestoreDelegate<T> __restore;

			internal static Type MementoType
			{
				get { return Util.LazyInitializeWithLock<Type>(ref __mementoType, __sync, () => ResolveMementoType()); }
			}

			/// <summary>
			/// Captures a memento containing the state of the given item.
			/// </summary>
			/// <param name="ctx">A memento context.</param>
			/// <param name="item">The item.</param>
			/// <returns>A memento containing the state of the given item.</returns>
			internal static IMemento CaptureMemento(IMementoContext ctx, T item)
			{
				Func<IMementoContext, T, IMemento> action = Util.LazyInitializeWithLock<Func<IMementoContext, T, IMemento>>(
					ref __capture, __sync, () => ResolveCapture()
					);

				return __capture(ctx, item);
			}

			internal static void RestoreMemento(IMementoContext ctx, ref T item, IMemento m)
			{
				MementoRestoreDelegate<T> action = Util.LazyInitializeWithLock<MementoRestoreDelegate<T>>(
					ref __restore, __sync, () => ResolveRestore()
					);

				__restore(ctx, ref item, m);
			}

			internal static IMemento CaptureBaseMemento(IMementoContext ctx, T item)
			{
				Func<IMementoContext, T, IMemento> action = Util.LazyInitializeWithLock<Func<IMementoContext, T, IMemento>>(
					ref __capture, __sync, () => ResolveCapture()
					);

				return __capture(ctx, item);
			}

			internal static void RestoreBaseMemento(IMementoContext ctx, ref T item, IMemento m)
			{
				MementoRestoreDelegate<T> action = Util.LazyInitializeWithLock<MementoRestoreDelegate<T>>(
					ref __restore, __sync, () => ResolveRestore()
					);

				__restore(ctx, ref item, m);
			}

			#region Emit

			private static Type ResolveMementoType()
			{
				Assembly targetAsm = typeof(T).Assembly;
				AssemblyName asmName = MakeEmittedAssemblyNameFromAssembly(targetAsm);

				if (typeof(T).IsGenericType)
				{
					typeof(T).GetGenericTypeDefinition();
				}
				string typeName = (typeof(T).IsGenericType) 
					? EmittedAssembly.FormatEmittedTypeName(typeof(T).GetGenericTypeDefinition(), "$Memento")
					: EmittedAssembly.FormatEmittedTypeName(typeof(T), "$Memento");

				Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(asmName,
					emittedAsm => GenerateImplementationsForAssembly(emittedAsm, targetAsm));

				Type result = asm.GetType(typeName, true, false);

				if (typeof(T).IsGenericType)
				{
					result = result.MakeGenericType(typeof(T).GetGenericArguments());
				}
				return result;
			}

			private static void GenerateImplementationsForAssembly(EmittedAssembly emittedAsm, Assembly targetAsm)
			{
				Type[] types = targetAsm.GetTypes();
				var concreteTypes = from t in types
														where t.IsEnum == false
														  && (t.IsPublic || t.IsNestedPublic)															
															&& t.IsPrimitive == false
															&& t.IsInterface == false
															&& (t.Namespace != null) // some .NET framework types like FXAssembly
															&& !t.IsDefined(typeof(MementoIgnoreAttribute), false)
															&& !t.DeclaresMethodWithAttribute<MementoCaptureAttribute>(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false)
														select t;

				foreach (Type tt in concreteTypes)
				{
					if (tt == typeof(void)) continue;
					GenerateImplementationsForType(emittedAsm, tt);
				}
			}

			private static void GenerateImplementationsForType(EmittedAssembly emittedAsm, Type tt)
			{
				GenerateImplementation(emittedAsm, tt);
				//Type[] types = tt.GetNestedTypes();
				//if (types.Length > 0)
				//{
				//  var concreteTypes = from t in types
				//                      where t.IsEnum == false
				//                        && t.IsPrimitive == false
				//                        && t.IsInterface == false
				//                        && (!t.IsAbstract && !t.IsSealed && t.Namespace != null) // some .NET framework types like FXAssembly
				//                        && !t.IsDefined(typeof(MementoIgnoreAttribute), false)
				//                        && !t.DeclaresMethodWithAttribute<MementoCaptureAttribute>(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false)
				//                      select t;

				//  foreach (Type ct in concreteTypes)
				//  {
				//    GenerateImplementationsForType(emittedAsm, ct);
				//  }
				//}
			}

			private static void GenerateImplementation(EmittedAssembly emittedAsm, Type tt)
			{
				//
				// Creates: 
				//
				//   public struct <type-name>Memento : IMemento
				//   {
				//     private <target-type> _target; // if a root type (directly derived from Object)
				//     public IMemento BaseMemento;   // if a derived type
				//     public <field-0-type> <field-0-name>; 
				//     public <field-1-type> <field-1-name>;
				//     public <field-n-type> <field-n-name>;
				//     ...
				//   }
				//
				if (tt.ContainsAnyFieldMatching(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
					(f) => { 
						Type ft = f.FieldType;
						if (ft.IsArray) ft = ft.GetElementType();
						return ft.IsPointer || (ft.IsPublic == false && ft.IsNestedPublic == false) || ft == typeof(IntPtr);
						}))
				{
					return;
				}
								
				EmittedClass cls = emittedAsm.DefineClass(EmittedAssembly.FormatEmittedTypeName(tt, "$Memento")
					, EmittedClass.DefaultTypeAttributes
					, typeof(ValueType)
					, new Type[] { typeof(IMemento) }
					);

				if (tt.IsGenericTypeDefinition)
				{
					cls.DefineGenericParamentersFromType(tt);
				}

				EmittedField isRestoredField = null;
				EmittedField targetField = null; ;
				EmittedField baseMementoField = null; ;
				if (tt.BaseType != null && tt.BaseType != typeof(Object) && tt.BaseType != typeof(ValueType))
				{
					baseMementoField = cls.DefineField("$BaseMemento", typeof(IMemento));
					baseMementoField.Attributes = FieldAttributes.Public;
				}
				else
				{
					targetField = cls.DefineField("$memento_target", tt);
					targetField.Attributes = FieldAttributes.Public;
					isRestoredField = cls.DefineField<bool>("$memento_isRestored");
				}

				foreach (FieldInfo field in tt.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					if (!field.IsLiteral)
					{
						EmittedField f = (Type.GetTypeCode(field.FieldType) == TypeCode.Object)
							? cls.DefineField(field.Name, typeof(IMemento))
							: cls.DefineField(field.Name, field.FieldType);
						f.Attributes = FieldAttributes.Public;
					}
				}

				EmittedProperty targetProp = cls.DefineProperty<object>("Target");
				EmittedMethod targetGetter = targetProp.AddGetter();
				targetGetter.DefineMethodBody((m, il) =>
					{
						il.LoadArg_0();
						if (targetField != null) il.LoadValue(targetField);
						else
						{
							il.LoadValue(baseMementoField);
							il.LoadProperty(typeof(IMemento).GetProperty("Target"), false);
						}
					}
					);

				EmittedProperty isRestoredProp = cls.DefineProperty<bool>("IsRestored");
				EmittedMethod isRestoredGetter = isRestoredProp.AddGetter();
				isRestoredGetter.DefineMethodBody((m, il) =>
					{
						il.Nop();
						il.LoadArg_0();
						if (isRestoredField != null) il.LoadValue(isRestoredField);
						else
						{
							il.LoadValue(baseMementoField);
							il.LoadProperty(typeof(IMemento).GetProperty("IsRestored"), false);
						}
						il.Nop();
					});
				EmittedMethod isRestoredSetter = isRestoredProp.AddSetter();
				isRestoredSetter.DefineMethodBody((m, il) =>
				{
					il.Nop();
					il.LoadArg_0();
					if (isRestoredField != null)
					{
						il.LoadArg_1();
						il.StoreValue(isRestoredField);
					}
					else
					{
						il.LoadValue(baseMementoField);
						il.LoadArg_1();
						il.StoreProperty(typeof(IMemento).GetProperty("IsRestored"), false);
					}
					il.Nop();
				});
			}
						
			private static AssemblyName MakeEmittedAssemblyNameFromAssembly(Assembly tasm)
			{
				AssemblyName tasmName = tasm.GetName();
				AssemblyName asmName = new AssemblyName(String.Format(
						EmittedAssemblyNameFormat
						, tasmName.Name
						, tasmName.Version
						, typeof(Memento).Assembly.GetName().Version).Replace('.', '_')
						);
				asmName.Version = tasmName.Version;
				asmName.CultureInfo = tasmName.CultureInfo;
				return asmName;
			}

			private static Func<IMementoContext, T, IMemento> ResolveCapture()
			{
				// Use the IMementoHelper<T> if one is configured...
				IMementoHelper<T> helper = Helper<T>.Instance;
				if (helper != null)
				{
					return new Func<IMementoContext, T, IMemento>(helper.CaptureMemento);
				}

				// Otherwise, see of the instance has an explicit capture. 
				// If so, use it...				
				MethodInfo resolve = (from m in typeof(T).DiscoverMethodsWithAttribute<MementoCaptureAttribute>(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false)
															select m).SingleOrDefault();
				if (resolve != null)
					return (Func<IMementoContext, T, IMemento>)
						Delegate.CreateDelegate(typeof(Func<IMementoContext, T, IMemento>)
						, resolve
						, true
						);

				// Otherwise, emit one...				
				FieldInfo incompatible = typeof(T).FindFirstFieldMatching(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
				(f) =>
				{
					Type ft = f.FieldType;
					if (ft.IsArray) ft = ft.GetElementType();
					return ft.IsPointer || ft == typeof(IntPtr);
				});
				if (incompatible != null)
				{
					throw new MementoNotSupportedException(String.Format(
						"Memento cannot be constructred for the type {0} because it contains a pointer. Consider a custom IMementoHelper implementation for {0}.", typeof(T).GetReadableFullName()));
				}
				incompatible = typeof(T).FindFirstFieldMatching(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
				(f) =>
				{
					Type ft = f.FieldType;
					if (ft.IsArray) ft = ft.GetElementType();
					return (ft.IsPublic == false && ft.IsNestedPublic == false);
				});
				if (incompatible != null)
				{
					throw new MementoNotSupportedException(String.Format(
						"Memento cannot be constructred for the type {0} because it refers to a non-public type: {1}. Consider a custom IMementoHelper implementation for {0}.", typeof(T).GetReadableFullName(), incompatible.FieldType.GetReadableFullName()));
				}
				
				Type mtype = MementoType;

				DynamicMethod method = new DynamicMethod("Om_CaptureMemento"
					, MethodAttributes.Static | MethodAttributes.Public
					, CallingConventions.Standard
					, typeof(IMemento)
					, new Type[] { typeof(IMementoContext), typeof(T) }
					, typeof(T)
					, false
					);

				ILHelper ilh = new ILHelper(method.GetILGenerator());
				LocalBuilder result = ilh.DeclareLocal(mtype);
				FieldInfo baseMemento = mtype.GetField("$BaseMemento", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (baseMemento != null)
				{
					Type baseTyp = typeof(T).BaseType;
					MethodInfo m = __genericCaptureBase.MakeGenericMethod(baseTyp);

					//
					// result.<field-name> = Memento.CaptureMemento(ctx, target.<field-name>);
					//
					ilh.LoadLocalAddressShort(result);
					ilh.LoadArg_0();
					ilh.LoadArg_1();
					ilh.Call(m);
					ilh.StoreField(baseMemento);
					ilh.Nop();
				}
				else
				{
					FieldInfo target = mtype.GetField("$memento_target", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					ilh.LoadLocalAddressShort(result);
					ilh.LoadArg_1();
					ilh.StoreField(target);					
				}

				foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
				{
					FieldInfo mfield = mtype.GetField(field.Name);
					if (mfield != null)
					{
						if (typeof(IMemento).IsAssignableFrom(mfield.FieldType))
						{
							MethodInfo m = (field.FieldType.IsArray)
								? __genericCaptureArray.MakeGenericMethod(field.FieldType.GetElementType())
								: __genericCapture.MakeGenericMethod(field.FieldType);
							//MethodInfo m = __genericCapture.MakeGenericMethod(field.FieldType);

								//
								// result.<field-name> = Memento.CaptureMemento(ctx, target.<field-name>);
								//
								ilh.LoadLocalAddress(result);
								ilh.LoadArg_0();
								if (!field.IsStatic) ilh.LoadArg_1();
								ilh.LoadField(field);
								ilh.Call(m);
								ilh.StoreField(mfield);
								ilh.Nop();
						}
						else
						{
							//
							// result.<field-name> = target.<field-name>;
							//
							ilh.LoadLocalAddress(result);
							if (!field.IsStatic) ilh.LoadArg_1();
							ilh.LoadField(field);
							ilh.StoreField(mfield);
						}
					}
				}

				ilh.LoadLocal(result);
				if (mtype.IsValueType)
					ilh.Box(mtype);
				ilh.Return();

				// Create the delegate
				return (Func<IMementoContext, T, IMemento>)method.CreateDelegate(typeof(Func<IMementoContext, T, IMemento>));
			}

			private static MementoRestoreDelegate<T> ResolveRestore()
			{
				// Use the IMementoHelper<T> if one is configured...
				IMementoHelper<T> helper = Helper<T>.Instance;
				if (helper != null)
				{
					return new MementoRestoreDelegate<T>(helper.RestoreMemento);
				}
				
				// Otherwise, see of the instance has an explicit restore. 
				// If so, use it...
				MethodInfo resolve = (from m in typeof(T).DiscoverMethodsWithAttribute<MementoRestoreAttribute>(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false)
															select m).SingleOrDefault();
				if (resolve != null)
					return (MementoRestoreDelegate<T>)
						Delegate.CreateDelegate(typeof(MementoRestoreDelegate<T>)
						, resolve
						, true
						);
					
				// Otherwise, try to emit one...
				Type mtype = MementoType;
				DynamicMethod method = new DynamicMethod("Om_RestoreMemento"
					, MethodAttributes.Static | MethodAttributes.Public
					, CallingConventions.Standard
					, null
					, new Type[] { typeof(IMementoContext), typeof(T).MakeByRefType(), typeof(IMemento) }
					, typeof(T)
					, false
					);
				
				ILHelper ilh = new ILHelper(method.GetILGenerator());				
				
				LocalBuilder memento = ilh.DeclareLocal(mtype);
				
				ilh.Nop();
				ilh.LoadArg_2();
				ilh.UnboxAny(mtype);
				ilh.StoreLocal(memento);

				FieldInfo baseMemento = mtype.GetField("$BaseMemento", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (baseMemento != null)
				{
					LocalBuilder baseRef = ilh.DeclareLocal(typeof(T).BaseType);
					ilh.LoadArg_1();
					ilh.LoadObjectRef();
					ilh.StoreLocal(baseRef);
					ilh.LoadArg_0();
					ilh.LoadLocalAddressShort(baseRef);
					ilh.LoadLocalAddressShort(memento);
					ilh.LoadField(baseMemento);
					ilh.Call(__genericRestoreBase.MakeGenericMethod(typeof(T).BaseType));
					ilh.Nop();
				}

				foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
				{
					FieldInfo mfield = mtype.GetField(field.Name);
					if (mfield != null)
					{
						if (typeof(IMemento).IsAssignableFrom(mfield.FieldType))
						{
							MethodInfo m = (field.FieldType.IsArray)
								? __genericRestoreArray.MakeGenericMethod(field.FieldType.GetElementType())
								: __genericRestore.MakeGenericMethod(field.FieldType);
							
							//
							// Memento.RestoreMemento<field-type>(ctx, ref item.<field-name>, memento.<field-name>);
							//
							ilh.LoadArg_0();
							if (field.IsStatic)
							{
								ilh.LoadFieldAddress(field);
							}
							else
							{
								ilh.LoadArg_1();
								ilh.LoadObjectRef();
								ilh.LoadFieldAddress(field);
							}
							ilh.LoadLocalAddress(memento);
							ilh.LoadField(mfield);
							ilh.Call(m);
							ilh.Nop();
						}
						else
						{
							//
							// item.<field-name> = memento.<field-name>;
							//
							if (field.IsStatic)
							{
								ilh.LoadLocalAddress(memento);
								ilh.LoadField(mfield);
								ilh.StoreField(field);
							}
							else
							{
								ilh.LoadArg_1();
								ilh.LoadObjectRef();
								ilh.LoadLocalAddress(memento);
								ilh.LoadField(mfield);
								ilh.StoreField(field);
							}
							ilh.Nop();
						}
					}
				}				
				
				ilh.Return();

				// Create the delegate
				return (MementoRestoreDelegate<T>)method.CreateDelegate(typeof(MementoRestoreDelegate<T>));
			}

			#endregion
		}
	}
}
