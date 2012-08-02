using System.Reflection.Emit;
using System.Reflection;
using System;

namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with fields in the IL stream.
	/// </summary>
	public class EmittedField : EmittedMember, IFieldRef
	{
		public FieldBuilder Builder { get; private set; }
		public FieldAttributes Attributes { get; set; }
		public TypeRef FieldType { get; private set; }
		public override bool IsStatic
		{
			get { return (Attributes & FieldAttributes.Static) == FieldAttributes.Static; }
		}

		public EmittedField(EmittedClass type, string name, TypeRef fieldType)
			: base(type, name)
		{
			Contracts.Require.IsNotNull("type", type);
			Contracts.Require.IsNotNull("name", name);

			this.FieldType = fieldType;
			this.Attributes = FieldAttributes.Private;
		}

		public void ExcludeAttributes(FieldAttributes attr)
		{
			Attributes &= (~attr);
		}
		public void IncludeAttributes(FieldAttributes attr)
		{
			Attributes |= attr;
		}

		protected internal override void OnCompile()
		{
			this.Builder = this.TargetClass.Builder.DefineField(this.Name
					, this.FieldType.Target
					, this.Attributes);
		}

		public void LoadValue(ILHelper il)
		{
			if (!this.IsCompiled) throw new InvalidOperationException("EmittedField not compiled: {0}");
			il.LoadField(Builder);
		}

		public void StoreValue(ILHelper il)
		{
			if (!this.IsCompiled) throw new InvalidOperationException("EmittedField not compiled: {0}");
			il.StoreField(Builder);
		}

		public void LoadAddress(ILHelper il)
		{
			if (!this.IsCompiled) throw new InvalidOperationException("EmittedField not compiled: {0}");
			il.LoadFieldAddress(Builder);
		}

		#region IFieldRef Members

		public FieldInfo GetFieldInfo()
		{
			if (!this.IsCompiled) throw new InvalidOperationException("EmittedField not compiled: {0}");
			return Builder;
		}

		#endregion
	}
}
