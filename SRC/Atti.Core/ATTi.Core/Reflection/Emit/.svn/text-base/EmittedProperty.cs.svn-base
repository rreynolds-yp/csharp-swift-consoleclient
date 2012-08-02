using System;
using System.Reflection;
using System.Reflection.Emit;


namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with properties in the IL stream.
	/// </summary>
	public class EmittedProperty : EmittedMember, IPropertyRef
	{
		private EmittedMethod _getter;
		private EmittedMethod _setter;
		private IFieldRef _boundField;
		private bool _isStatic;
		
		public PropertyBuilder Builder { get; private set; }
		public PropertyAttributes Attributes { get; protected set; }
		public CallingConventions CallingConventions { get; protected set; }
		public TypeRef PropertyType { get; private set; }
		public Type[] ParameterTypes { get { return null; } }
		public bool IsReadonly { get; set; }
		public override bool IsStatic
		{
			get { return _isStatic; }
		}


		public EmittedProperty(EmittedClass type, string name, TypeRef propertyType, bool isStatic) : base(type, name) 
		{
			Contracts.Require.IsNotNull("propertyType", propertyType);
			
			this.PropertyType = propertyType;
			this._isStatic = isStatic;
		}

		public EmittedMethod AddGetter()
		{
			if (_getter != null) throw new InvalidOperationException("Getter already assigned");
			_getter = this.TargetClass.DefineMethod(String.Format("get_{0}", this.Name));
			if (IsStatic) _getter.IncludeAttributes(MethodAttributes.Static);
			_getter.IncludeAttributes(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.NewSlot);
			_getter.ReturnType = this.PropertyType;
			return _getter;
		}
		public EmittedMethod AddSetter()
		{
			if (_setter != null) throw new InvalidOperationException("Setter already assigned");
			_setter = this.TargetClass.DefineMethod(String.Format("set_{0}", this.Name));
			if (IsStatic) _setter.IncludeAttributes(MethodAttributes.Static);
			_setter.IncludeAttributes(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.NewSlot);
			_setter.DefineParameter("value", PropertyType);
			return _setter;
		}

		public void BindField(FieldInfo field)
		{
			Contracts.Require.IsNotNull("field", field);
			
			if (_boundField != null)
				throw new InvalidOperationException(String.Format("Already bound to field: {0}", _boundField.Name));
			if (IsStatic != field.IsStatic) throw new ArgumentException("Backing field and property scope must agree; either both static or both non-static");
			_boundField = new RawFieldRef(field);
		}

		public void BindField(EmittedField field)
		{
			Contracts.Require.IsNotNull("field", field);
			
			if (_boundField != null) 
				throw new InvalidOperationException(String.Format("Already bound to field: {0}", _boundField.Name));
			if (IsStatic != field.IsStatic) throw new ArgumentException("Backing field and property scope must agree; either both static or both non-static");
			_boundField = field;
		}

		protected internal override void OnCompile()
		{
			this.Builder = this.TargetClass.Builder.DefineProperty(this.Name
				, this.Attributes
				, this.PropertyType.Target
				, this.ParameterTypes);
			if (_getter == null && _boundField != null)
			{				
				_getter = AddGetter();
				_getter.DefineMethodBody((m,il) =>
					{
						il.LoadArg_0();
						_boundField.LoadValue(il);
					});
			}
			if (_getter != null)
			{
				if (!_getter.IsCompiled) _getter.Compile();
				Builder.SetGetMethod(_getter.Builder);
			}
			if (_setter == null && _boundField != null)
			{
				_setter = AddSetter();
				
				_setter.DefineMethodBody((m, il) =>
					{
						il.LoadArg_0();
						il.LoadArg_1();
						_boundField.StoreValue(il);
					});
			}
			if (_setter != null)
			{
				if (!_setter.IsCompiled) _setter.Compile();
				Builder.SetSetMethod(_setter.Builder);
			}
		}

		#region IPropertyRef Members

		public PropertyInfo GetPropertyInfo()
		{
			throw new NotImplementedException();
		}

		public void LoadValue(ILHelper il)
		{
			if (!IsCompiled) Compile();
			il.LoadProperty(Builder, true);
		}

		public void StoreValue(ILHelper il)
		{
			if (!IsCompiled) Compile();
			il.StoreProperty(Builder, true);			
		}

		public void LoadAddress(ILHelper il)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
