using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with constructors in the IL stream.
	/// </summary>
	public class EmittedConstructor : EmittedMethodBase
	{
		public ConstructorBuilder Builder { get; private set; }

		public EmittedConstructor(EmittedClass type, string name)
			: base(type, name)
		{
			this.IncludeAttributes(MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public);
			this.CallingConvention = CallingConventions.Standard;
		}

		protected internal override void OnCompile()
		{
			Builder = this.TargetClass.Builder.DefineConstructor(this.Attributes
				, this.CallingConvention, this.ParameterTypes
				);
			ILGenerator il = SetILGenerator(Builder.GetILGenerator());
			try
			{
				CompileParameters(Builder);
				CompileLocals(il);

				ILHelper msil = new ILHelper(il);
				base.EmitInstructions(msil);
				msil.Return();
			}
			finally
			{
				SetILGenerator(il);
			}
		}
		public override void EmitCall(ILHelper il)
		{
			if (!IsCompiled) Compile();
			il.Call(Builder);
		}
	}
}
