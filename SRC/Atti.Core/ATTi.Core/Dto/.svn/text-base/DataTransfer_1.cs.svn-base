using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using ATTi.Core.Factory;
using ATTi.Core.Reflection.Emit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ATTi.Core.Dto
{
	public static class DataTransfer<T>
	{
		readonly static string EmittedAssemblyNameFormat = "{0}v{1}__DTO_{2}";
		static Object __sync = new Object();
		static Type __dtoType;
		static Func<XElement, T> __fromXml;
		static Func<XElement, T> __fromXmlWithCapture;
		static Func<JObject, T> __fromJson;
		static Func<JObject, T> __fromJsonWithCapture;

		static DataTransfer()
		{
			if (typeof(T).IsClass)
			{
				__dtoType = typeof(T);
			}
		}

		public static Type ImplementationType
		{
			get { return Util.LazyInitializeWithLock<Type>(ref __dtoType, __sync, () => ResolveDtoType()); }
		}

		#region Emit

		private static Type ResolveDtoType()
		{
			Assembly targetAsm = typeof(T).Assembly;
			AssemblyName asmName = MakeEmittedAssemblyNameFromAssembly(targetAsm);

			string typeName = EmittedAssembly.FormatEmittedTypeName(typeof(T), "$DTO");

			Assembly asm = RuntimeAssemblies.GetEmittedAssemblyWithEmitWhenNotFound(asmName,
				emittedAsm => GenerateImplementationsForAssembly(emittedAsm, targetAsm));

			return asm.GetType(typeName, true, false);
		}

		private static void GenerateImplementationsForAssembly(EmittedAssembly emittedAsm, Assembly targetAsm)
		{
			Type[] types = targetAsm.GetTypes();
			var concreteTypes = from t in types
													where t.IsPublic
													select t;

			foreach (Type tt in concreteTypes)
			{
				GenerateImplementationsForType(emittedAsm, tt);
			}
		}

		private static void GenerateImplementationsForType(EmittedAssembly emittedAsm, Type tt)
		{
			if (tt.IsInterface && tt.IsDefined(typeof(DtoAttribute), true))
			{
				GenerateImplementation(emittedAsm, tt);
			}
			Type[] types = tt.GetNestedTypes();
			if (types.Length > 0)
			{
				var concreteTypes = from t in types
														where t.IsNestedPublic
														select t;

				foreach (Type ct in concreteTypes)
				{
					GenerateImplementationsForType(emittedAsm, ct);
				}
			}
		}

		private static void GenerateImplementation(EmittedAssembly emittedAsm, Type tt)
		{
			EmittedClass cls = emittedAsm.DefineClass(EmittedAssembly.FormatEmittedTypeName(tt, "$DTO"), EmittedClass.DefaultTypeAttributes,
				typeof(DataTransferObject), new Type[] { tt });

			cls.DefineDefaultCtor();

			List<Type> interfaces = new List<Type>((tt.GetInterfaces()).Except(typeof(DataTransferObject).GetInterfaces()));
			foreach (var intf in interfaces)
			{
				if (intf != typeof(DataTransferObject))
				{
					EmitReadWritePropertiesFor(cls, intf);
					EmitMethodStubs(cls, intf);
				}
			}
			EmitReadWritePropertiesFor(cls, tt);
			EmitMethodStubs(cls, tt);
		}

		private static void EmitMethodStubs(EmittedClass cls, Type tt)
		{
			foreach (MethodInfo m in tt.GetMethods())
			{
				// Don't stub the getters and setters.
				if ((m.Name.StartsWith("get_") || m.Name.StartsWith("set_"))
					&& tt.GetProperty(m.Name.Substring(4)) != null)
					continue;

				EmittedMethod em = cls.DefineOverrideMethod(m);
				em.DefineMethodBody((mb, il) =>
				{
					il.Nop();
					il.NewObj(typeof(NotImplementedException).GetConstructor(Type.EmptyTypes));
					il.Throw(typeof(NotImplementedException));
				});
			}
		}

		private static void EmitReadWritePropertiesFor(EmittedClass cls, Type tt)
		{
			foreach (PropertyInfo iprop in tt.GetProperties())
			{
				EmittedField cfield = cls.DefineField(String.Format("_backing_field_{0}", iprop.Name), iprop.PropertyType);
				EmittedProperty cprop = cls.DefineProperty(iprop.Name, iprop.PropertyType);
				cprop.BindField(cfield);
			}
		}

		private static AssemblyName MakeEmittedAssemblyNameFromAssembly(Assembly tasm)
		{
			AssemblyName tasmName = tasm.GetName();
			AssemblyName asmName = new AssemblyName(String.Format(
					EmittedAssemblyNameFormat
					, tasmName.Name
					, tasmName.Version
					, typeof(DataTransfer<T>).Assembly.GetName().Version).Replace('.', '_')
					);
			asmName.Version = tasmName.Version;
			asmName.CultureInfo = tasmName.CultureInfo;
			return asmName;
		}

		private static Func<XElement, T> ResolveFromXml()
		{
			DynamicMethod method = new DynamicMethod("DtoFromXml"
				, MethodAttributes.Static | MethodAttributes.Public
				, CallingConventions.Standard
				, typeof(T)
				, new Type[] { typeof(XElement) }
				, typeof(DataTransfer<T>)
				, false
				);

			ILHelper ilh = new ILHelper(method.GetILGenerator());

			Type copiertype = typeof(Factory<T>)
						.GetNestedType("Copier`1", BindingFlags.Public | BindingFlags.Static)
						.MakeGenericType(typeof(T), ImplementationType);
			MethodInfo copyFromXml = copiertype.GetMethod("CopyFromXml", BindingFlags.Static | BindingFlags.Public);

			ilh.LoadArg_0();
			ilh.Call(copyFromXml);
			ilh.Return();

			// Create the delegate
			return (Func<XElement, T>)method.CreateDelegate(typeof(Func<XElement, T>));
		}

		private static Func<XElement, T> ResolveFromXmlWithCapture()
		{
			DynamicMethod method = new DynamicMethod("DtoFromXmlWithCapture"
				, MethodAttributes.Static | MethodAttributes.Public
				, CallingConventions.Standard
				, typeof(T)
				, new Type[] { typeof(XElement) }
				, typeof(DataTransfer<T>)
				, false
				);

			ILHelper ilh = new ILHelper(method.GetILGenerator());

			Type copiertype = typeof(Factory<T>)
						.GetNestedType("Copier`1", BindingFlags.Public | BindingFlags.Static)
						.MakeGenericType(typeof(T), ImplementationType);
			MethodInfo copyFromXml = copiertype.GetMethod("CopyFromXmlWithCapture", BindingFlags.Static | BindingFlags.Public);

			ilh.LoadArg_0();
			ilh.Call(copyFromXml);
			ilh.Return();

			// Create the delegate
			return (Func<XElement, T>)method.CreateDelegate(typeof(Func<XElement, T>));
		}

		private static Func<JObject, T> ResolveFromJson()
		{
			DynamicMethod method = new DynamicMethod("DtoFromJson"
				, MethodAttributes.Static | MethodAttributes.Public
				, CallingConventions.Standard
				, typeof(T)
				, new Type[] { typeof(JObject) }
				, typeof(DataTransfer<T>)
				, false
				);

			ILHelper ilh = new ILHelper(method.GetILGenerator());

			Type copiertype = typeof(Factory<T>)
						.GetNestedType("Copier`1", BindingFlags.Public | BindingFlags.Static)
						.MakeGenericType(typeof(T), ImplementationType);
			MethodInfo copyFromJson = copiertype.GetMethod("CopyFromJson", BindingFlags.Static | BindingFlags.Public);

			ilh.LoadArg_0();
			ilh.Call(copyFromJson);
			ilh.Return();

			// Create the delegate
			return (Func<JObject, T>)method.CreateDelegate(typeof(Func<JObject, T>));
		}

		private static Func<JObject, T> ResolveFromJsonWithCapture()
		{
			DynamicMethod method = new DynamicMethod("DtoFromJsonWithCapture"
				, MethodAttributes.Static | MethodAttributes.Public
				, CallingConventions.Standard
				, typeof(T)
				, new Type[] { typeof(JObject) }
				, typeof(DataTransfer<T>)
				, false
				);

			ILHelper ilh = new ILHelper(method.GetILGenerator());

			Type copiertype = typeof(Factory<T>)
						.GetNestedType("Copier`1", BindingFlags.Public | BindingFlags.Static)
						.MakeGenericType(typeof(T), ImplementationType);
			MethodInfo copyFromJson = copiertype.GetMethod("CopyFromJsonWithCapture", BindingFlags.Static | BindingFlags.Public);

			ilh.LoadArg_0();
			ilh.Call(copyFromJson);
			ilh.Return();

			// Create the delegate
			return (Func<JObject, T>)method.CreateDelegate(typeof(Func<JObject, T>));
		}

		#endregion

		public static T FromXml(XElement item)
		{
			Func<XElement, T> action = Util.LazyInitializeWithLock<Func<XElement, T>>(ref __fromXml, __sync, () => ResolveFromXml());
			return action(item);
		}

		public static T FromXmlWithCaptureExtraData(XElement item)
		{
			Func<XElement, T> action = Util.LazyInitializeWithLock<Func<XElement, T>>(ref __fromXmlWithCapture, __sync, () => ResolveFromXmlWithCapture());
			return action(item);
		}

		public static T FromJson(string json)
		{
			var item = JObject.Parse(json);
			Func<JObject, T> action = Util.LazyInitializeWithLock<Func<JObject, T>>(
				ref __fromJson, __sync, () => ResolveFromJson());
			return action(item);
		}

		public static T FromJsonWithCaptureExtraData(string json)
		{
			var item = JObject.Parse(json);
			Func<JObject, T> action = Util.LazyInitializeWithLock<Func<JObject, T>>(
				ref __fromJsonWithCapture, __sync, () => ResolveFromJsonWithCapture());
			return action(item);
		}

		internal static T FromJson(JObject item)
		{
			Func<JObject, T> action = Util.LazyInitializeWithLock<Func<JObject, T>>(
				ref __fromJson, __sync, () => ResolveFromJson());
			return action(item);
		}

		internal static T FromJsonWithCaptureExtraData(JObject item)
		{
			Func<JObject, T> action = Util.LazyInitializeWithLock<Func<JObject, T>>(
				ref __fromJsonWithCapture, __sync, () => ResolveFromJsonWithCapture());
			return action(item);
		}
	}	
}
