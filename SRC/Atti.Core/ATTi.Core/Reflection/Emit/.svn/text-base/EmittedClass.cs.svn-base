using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ATTi.Core.Contracts;

namespace ATTi.Core.Reflection.Emit
{
	public class EmittedClass : EmittedMember
	{
		#region Fields
		public static readonly TypeAttributes DefaultTypeAttributes = TypeAttributes.BeforeFieldInit | TypeAttributes.Public;
		public static readonly TypeAttributes StaticTypeAttributes = TypeAttributes.Sealed | TypeAttributes.Abstract;
		private Dictionary<string, List<EmittedMember>> _members;
		private Dictionary<string, EmittedField> _fields;
		private Dictionary<string, EmittedProperty> _properties;
		private Dictionary<string, List<EmittedMethodBase>> _methods;
		private Dictionary<string, EmittedClass> _types;
		private Dictionary<string, EmittedGenericArgument> _genericArguments;

		ModuleBuilder _module;
		EmittedClass _nestParent;
		string _name;
		Type _supertype;
		TypeAttributes _attributes;
		List<TypeRef> _implementedInterfaces;
		TypeRef _ref;
		#endregion

		#region Contracts
		[InvariantContract]
		protected void ObjectInvariant()
		{
			Contracts.Invariant.IsNotNull("_module", _module);
		}

		#endregion

		#region Constructors
		internal EmittedClass(ModuleBuilder module, string name)
			: this(module, name, TypeAttributes.BeforeFieldInit | TypeAttributes.Public, null, Type.EmptyTypes)
		{
		}

		internal EmittedClass(ModuleBuilder module, string name, TypeAttributes attributes)
			: this(module, name, attributes, null, Type.EmptyTypes)
		{
		}

		internal EmittedClass(ModuleBuilder module, string name, TypeAttributes attributes
			, Type supertype, Type[] interfaces)
			: base(null, name)
		{
			Contracts.Require.IsNotNull("module", module);
			Contracts.Require.IsNotNull("name", name);

			_module = module;
			_name = name;
			_supertype = supertype;
			_implementedInterfaces = (interfaces == null)
				? new List<TypeRef>()
				: new List<TypeRef>(from i in interfaces
														select new TypeRef(i));

			_attributes = attributes;
			_members = new Dictionary<string, List<EmittedMember>>();
			_fields = new Dictionary<string, EmittedField>();
			_properties = new Dictionary<string, EmittedProperty>();
			_methods = new Dictionary<string, List<EmittedMethodBase>>();
			_genericArguments = new Dictionary<string, EmittedGenericArgument>();
			_types = new Dictionary<string, EmittedClass>();
			_ref = new EmittedTypeRef(this);
		}

		internal EmittedClass(EmittedClass eclass, string name, TypeAttributes attributes
			, Type supertype, Type[] interfaces)
			: base(eclass, name)
		{
			Contracts.Require.IsNotNull("eclass", eclass);

			_nestParent = eclass;
			_name = name;
			_supertype = supertype;
			_implementedInterfaces = (interfaces == null)
				? new List<TypeRef>()
				: new List<TypeRef>(from i in interfaces
														select new TypeRef(i));

			_attributes = attributes;
			_members = new Dictionary<string, List<EmittedMember>>();
			_fields = new Dictionary<string, EmittedField>();
			_properties = new Dictionary<string, EmittedProperty>();
			_methods = new Dictionary<string, List<EmittedMethodBase>>();
			_genericArguments = new Dictionary<string, EmittedGenericArgument>();
			_types = new Dictionary<string, EmittedClass>();
			_ref = new EmittedTypeRef(this);
		}
		#endregion

		public TypeBuilder Builder { get; private set; }
		public TypeRef Ref { get { return _ref; } }

		#region EmittedMember overrides
		protected internal override void OnCompile()
		{
			if (_module != null)
			{
				this.Builder = _module.DefineType(_name, _attributes, _supertype ?? typeof(Object)
					, (from i in _implementedInterfaces
						 select i.Target).ToArray()
							);
			}
			else
			{
				_nestParent.Compile();
				this.Builder = _nestParent.Builder.DefineNestedType(_name, _attributes, _supertype ?? typeof(Object)
					, (from i in _implementedInterfaces
						 select i.Target).ToArray()
						);
			}
			if (_genericArguments.Count > 0)
			{
				foreach (var arg in this.Builder.DefineGenericParameters(
					(from a in _genericArguments.Values
					 orderby a.Position
					 select a.Name).ToArray()
					))
				{
					_genericArguments[arg.Name].FinishDefinition(arg);
				}				
			}
			foreach (EmittedMember m in _fields.Values)
			{
				m.Compile();
			}
			foreach (List<EmittedMethodBase> mm in _methods.Values)
			{
				foreach (EmittedMethodBase m in mm)
				{
					m.Compile();
				}
			}
			foreach (EmittedMember m in _properties.Values)
			{
				m.Compile();
			}
			foreach (EmittedMember m in _types.Values)
			{
				m.Compile();
			}
			Builder.CreateType();
		}

		/// <summary>
		/// Indicates whether the class is static.
		/// </summary>
		public override bool IsStatic
		{
			get { return (_attributes & StaticTypeAttributes) == StaticTypeAttributes; }
		}
		#endregion

		private void CheckMemberName(string name)
		{
			if (_members.ContainsKey(name)) throw new InvalidOperationException(String.Format(
	@"Type already contains a member by the same name:
	member name = {0}
	type = {1}", name, this.Name));
		}

		private void AddField(EmittedField field)
		{
			CheckMemberName(field.Name);
			_fields.Add(field.Name, field);
			this.AddMember(field);
		}
		private void AddMethod(EmittedMethodBase method)
		{
			List<EmittedMethodBase> methods;
			if (_methods.TryGetValue(method.Name, out methods))
			{
				methods.Add(method);
			}
			else
			{
				methods = new List<EmittedMethodBase>();
				_methods.Add(method.Name, methods);
				methods.Add(method);
			}
			this.AddMember(method);
		}
		private void AddProperty(EmittedProperty prop)
		{
			CheckMemberName(prop.Name);
			_properties.Add(prop.Name, prop);
			this.AddMember(prop);
		}
		private void AddMember(EmittedMember m)
		{
			List<EmittedMember> members;
			if (_members.TryGetValue(m.Name, out members))
			{
				members.Add(m);
			}
			else
			{
				members = new List<EmittedMember>();
				_members.Add(m.Name, members);
				members.Add(m);
			}
		}
		private void AddGenericArgument(EmittedGenericArgument arg)
		{
			_genericArguments.Add(arg.Name, arg);
		}

		public EmittedClass DefineNestedType()
		{
			throw new NotImplementedException();
		}

		public EmittedConstructor DefineCtor()
		{
			EmittedConstructor result = new EmittedConstructor(this, "ctor");
			this.AddMethod(result);
			return result;
		}

		public EmittedConstructor DefineDefaultCtor()
		{
			EmittedConstructor result = new EmittedConstructor(this, "ctor");
			this.AddMethod(result);
			result.DefineMethodBody((m, il) =>
				{
					if (_supertype != null)
					{
						ConstructorInfo superCtor = _supertype.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
							, null, Type.EmptyTypes, null
							);
						if (superCtor != null && !superCtor.IsPrivate)
						{
							il.LoadArg_0();
							il.Call(superCtor);
							il.Nop();
						}
					}
				});
			return result;
		}


		public EmittedConstructor DefineCCtor()
		{
			EmittedConstructor result = new EmittedConstructor(this, "cctor");
			result.ExcludeAttributes(MethodAttributes.Public);
			result.IncludeAttributes(MethodAttributes.Private | MethodAttributes.Static);
			this.AddMethod(result);
			return result;
		}

		public EmittedMethod DefineMethod(string methodName)
		{
			Contracts.Require.IsNotNull("methodName", methodName);

			EmittedMethod method = new EmittedMethod(this, methodName);
			this.AddMethod(method);
			return method;
		}
		public EmittedMethod DefineOverrideMethod(MethodInfo methodInfo)
		{
			EmittedMethod method = new EmittedMethod(this, methodInfo.Name);
			method.ClearAttributes();
			// mask off the abstract and vtable attributes if the overriden method is abstract
			method.IncludeAttributes(methodInfo.Attributes & ~(MethodAttributes.Abstract | MethodAttributes.VtableLayoutMask));
			method.CallingConvention = methodInfo.CallingConvention;
			method.ReturnType = new TypeRef(methodInfo.ReturnType);
			foreach (ParameterInfo parm in methodInfo.GetParameters())
			{
				EmittedParameter p = method.DefineParameter(parm.Name, parm.ParameterType);
				p.ClearAttributes();
				p.IncludeAttributes(parm.Attributes);
			}
			method.CheckOverrideOnCompile(methodInfo);
			this.AddMethod(method);
			return method;
		}

		public EmittedProperty DefineProperty<T>(string propertyName)
		{
			return DefineProperty(propertyName, typeof(T));
		}
		public EmittedProperty DefinePropertyWithBackingField<T>(string propertyName)
		{
			return DefinePropertyWithBackingField(propertyName, typeof(T));
		}
		public EmittedProperty DefineProperty(string propertyName, Type propertyType)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);
			Contracts.Require.IsNotNull("propertyType", propertyType);

			EmittedProperty prop = new EmittedProperty(this, propertyName, new TypeRef(propertyType), false);
			this.AddProperty(prop);
			return prop;
		}
		public EmittedProperty DefinePropertyWithBackingField(string propertyName, Type propertyType)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);
			Contracts.Require.IsNotNull("propertyType", propertyType);

			EmittedProperty prop = new EmittedProperty(this, propertyName, new TypeRef(propertyType), false);
			prop.BindField(DefineField(String.Concat("_backing", propertyName), propertyType));
			this.AddProperty(prop);
			return prop;
		}

		public EmittedField DefineField<T>(string fieldName)
		{
			return DefineField(fieldName, typeof(T));
		}
		public EmittedField DefineField(string fieldName, Type fieldType)
		{
			Contracts.Require.IsNotNull("fieldName", fieldName);
			Contracts.Require.IsNotNull("fieldType", fieldType);

			EmittedField fld = new EmittedField(this, fieldName, new TypeRef(fieldType));
			this.AddField(fld);
			return fld;
		}

		public void AddInterfaceImplementation(Type interfaceType)
		{
			Contracts.Require.IsNotNull("interfaceType", interfaceType);

			_implementedInterfaces.Add(new TypeRef(interfaceType));
		}

		/// <summary>
		/// Defines generic arguments that mirrors the generic type definition from the given type.
		/// </summary>
		/// <param name="t"></param>
		public void DefineGenericParamentersFromType(Type t)
		{
			Contracts.Require.IsNotNull("t", t);

			foreach(var a in t.GetGenericArguments())
			{
				EmittedGenericArgument arg = new EmittedGenericArgument { Name = a.Name, Position = a.GenericParameterPosition, Attributes = a.GenericParameterAttributes };
				AddGenericArgument(arg);
				foreach(var c in a.GetGenericParameterConstraints())
				{
					if (c.IsInterface)
					{
						arg.AddInterfaceConstraint(c);
					}
					else
					{
						arg.AddBaseTypeConstraint(c);
					}
				}
			}
		}
	}
}
