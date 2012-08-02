using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using ATTi.Core.Contracts;



namespace ATTi.Core.Reflection.Emit
{
	public class EmittedAssembly
	{
		private Dictionary<string, EmittedClass> _classes;
		private AssemblyName _assemName;
		private AssemblyBuilder _assembly;
		private ModuleBuilder _module;

		public string Name { get { return _assemName.Name; } }
		public string RootNamespace { get; private set; }
		public string DLLName { get; private set; }
		public bool IsCompiled { get; private set; }

		public EmittedAssembly(string name, string rootNamespace)
			: this(name, rootNamespace, new Version(1, 0, 0, 0), new CultureInfo("en"), null, null)
		{
		}

		public EmittedAssembly(string name, string rootNamespace, Version version, CultureInfo culture)
			: this(name, rootNamespace, version, culture, null, null)
		{
		}

		public EmittedAssembly(string name, string rootNamespace, Version version, CultureInfo culture
			, byte[] publicKey, byte[] publicKeyToken)
		{
			Contracts.Require.IsNotNull("name", name);
			Contracts.Require.IsNotEmpty("name", name);
			Contracts.Require.IsNotNull("rootNamespace", rootNamespace);
			Contracts.Require.IsNotEmpty("rootNamespace", name);
			Contracts.Require.IsNotNull("version", version);
			Contracts.Require.IsNotNull("culture", culture);

			this._classes = new Dictionary<string, EmittedClass>();
			this.RootNamespace = rootNamespace ?? name;
			_assemName = new AssemblyName(name);
			_assemName.Version = version;
			_assemName.CultureInfo = culture;
			_assemName.SetPublicKey(publicKey);
			_assemName.SetPublicKeyToken(publicKeyToken);
#if DEBUG
			this._assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemName, AssemblyBuilderAccess.RunAndSave);
			this._module = this._assembly.DefineDynamicModule(name, name + ".dll", false);
#else
			this._assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemName, AssemblyBuilderAccess.Run);
			this._module = this._assembly.DefineDynamicModule(name, name + ".dll", false);
#endif

		}

		public EmittedAssembly(AssemblyName name, string rootNamespace)
		{
			Contracts.Require.IsNotNull("name", name);

			this._classes = new Dictionary<string, EmittedClass>();
			this.RootNamespace = rootNamespace ?? name.Name;
			this._assemName = name;
			this._assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemName, AssemblyBuilderAccess.RunAndSave);
			this._module = this._assembly.DefineDynamicModule(name.Name, name.Name + ".dll");

		}

		public Assembly Compile()
		{
			if (this.IsCompiled) throw new InvalidOperationException();
			foreach (EmittedClass c in this._classes.Values)
			{
				c.Compile();
			}
			this.IsCompiled = true;
			return this._assembly;
		}

		private void CheckClassName(string className)
		{
			Contracts.Require.IsNotNull("className", className);

			if (_classes.ContainsKey(className)) throw new InvalidOperationException(String.Format(
				@"Unable to generate duplicate class; the class name is already in use:
	class = {0}
	assembly = {1}", className, _assemName));
		}

		public EmittedClass DefineClass(string className)
		{
			Contracts.Require.IsNotNull("className", className);
			CheckClassName(className);

			EmittedClass cls = new EmittedClass(_module, className);
			_classes.Add(className, cls);
			return cls;
		}

		public EmittedClass DefineClass(string className, TypeAttributes attributes
			, Type supertype, Type[] interfaces)
		{
			Contracts.Require.IsNotNull("className", className);
			CheckClassName(className);

			EmittedClass cls = new EmittedClass(_module, className, attributes, supertype, interfaces);
			_classes.Add(className, cls);
			return cls;
		}

		internal Assembly Save()
		{
			Contracts.Invariant.AssertState(IsCompiled, "Assembly must be compiled before save");
			_assembly.Save(_assemName.Name + ".dll");
			return _assembly;
		}

		[InvariantContract]
		protected void ObjectInvariant()
		{
			Contracts.Invariant.IsNotNull("_module", _module);
		}

		public static string FormatEmittedTypeName(Type t, string append)
		{
			Contracts.Require.IsNotNull("t", t);
			
			return String.Concat(t.Namespace
				, ".generated"
				, MangleTypeName(t).Substring(t.Namespace.Length).Replace("+", "-")
				, append ?? String.Empty
				);
		}

		public static string MangleTypeName(Type t)
		{
			Contracts.Require.IsNotNull("t", t);

			string result;
			Type tt = (t.IsArray) ? t.GetElementType() : t;
			string simpleName = tt.Name;
			if (simpleName.Contains("`"))
			{
				simpleName = simpleName.Substring(0, simpleName.IndexOf("`"));
				var args = tt.GetGenericArguments();
				simpleName = String.Concat(simpleName, '\u2014');
				for (int i = 0; i < args.Length; i++)
				{
					if (i == 0) simpleName = String.Concat(simpleName, i, MangleTypeNameWithoutNamespace(args[i]));
					else simpleName = String.Concat(simpleName, '-', i, MangleTypeNameWithoutNamespace(args[i]));
				}
			}
			if (tt.IsNested)
			{
				result = String.Concat(tt.DeclaringType.GetReadableFullName(), "+", simpleName);
			}
			else
			{
				result = String.Concat(tt.Namespace, ".", simpleName);
			}
			return result;
		}
		private static string MangleTypeNameWithoutNamespace(Type t)
		{
			Type tt = (t.IsArray) ? t.GetElementType() : t;
			string simpleName = tt.Name;
			if (simpleName.Contains("`"))
			{
				simpleName = simpleName.Substring(0, simpleName.IndexOf("`"));
				var args = tt.GetGenericArguments();
				simpleName = String.Concat(simpleName, '\u2014');
				for (int i = 0; i < args.Length; i++)
				{
					if (i == 0) simpleName = String.Concat(simpleName, i, MangleTypeNameWithoutNamespace(args[i]));
					else simpleName = String.Concat(simpleName, '-', i, MangleTypeNameWithoutNamespace(args[i]));
				}

			}
			return simpleName;
		}
	}
}
