using System.Reflection;
using System.Reflection.Emit;

namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with parameters in the IL stream.
	/// </summary>
	public class EmittedParameter
	{
		public ParameterBuilder Builder { get; private set; }
		public int Index { get; private set; }
		public string Name { get; private set; }
		public ParameterAttributes Attributes { get; private set; }
		public TypeRef ParameterType { get; private set; }

		internal EmittedParameter(int index, string name, TypeRef type)
		{
			this.Index = index;
			this.Name = name;
			this.ParameterType = type;
			this.Attributes = ParameterAttributes.None;
		}

		public void ClearAttributes()
		{
			Attributes = (ParameterAttributes)0;
		}

		public void ExcludeAttributes(ParameterAttributes attr)
		{
			Attributes &= (~attr);
		}
		public void IncludeAttributes(ParameterAttributes attr)
		{
			Attributes |= attr;
		}

		internal void Compile(MethodBuilder m)
		{
			if (Builder == null)
			{
				int ofs = (m.IsStatic ? 0 : 1);
				Builder = m.DefineParameter(this.Index + ofs, this.Attributes, this.Name);
			}
		}

		internal void Compile(ConstructorBuilder c)
		{
			if (Builder == null)
			{
				int ofs = (c.IsStatic ? 0 : 1);
				Builder = c.DefineParameter(this.Index + ofs, this.Attributes, this.Name);
			}
		}
		
	}
}
