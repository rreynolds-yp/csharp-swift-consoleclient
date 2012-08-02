using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;



namespace ATTi.Core.Reflection.Emit
{
	public abstract class EmittedMethodBase : EmittedMember
	{
		List<EmittedParameter> _parameters;
		List<EmittedLocal> _locals;
		ILGenerator _generatorDuringCompile;

		public MethodAttributes Attributes { get; private set; }
		public CallingConventions CallingConvention { get; set; }
		public Type[] ParameterTypes
		{
			get
			{
				return (from prop in _parameters
								select prop.ParameterType.Target).ToArray<Type>();
			}
		}
		public override bool IsStatic
		{
			get { return (Attributes & MethodAttributes.Static) == MethodAttributes.Static; }
		}

		private List<Action<EmittedMethodBase, ILHelper>> _gen;

		public EmittedMethodBase(EmittedClass type, string name)
			: base(type, name)
		{
			_parameters = new List<EmittedParameter>();
			_locals = new List<EmittedLocal>();
		}

		public void ClearAttributes()
		{
			Attributes = (MethodAttributes)0;
		}
		public void ExcludeAttributes(MethodAttributes attr)
		{
			Attributes &= (~attr);
			if ((Attributes & MethodAttributes.Static) == MethodAttributes.Static)
			{
				this.CallingConvention &= (~CallingConventions.HasThis);
			}
			else this.CallingConvention |= CallingConventions.HasThis;
		}
		public void IncludeAttributes(MethodAttributes attr)
		{
			Attributes |= attr;
			if ((Attributes & MethodAttributes.Static) == MethodAttributes.Static)
			{
				this.CallingConvention &= (~CallingConventions.HasThis);
			}
			else this.CallingConvention |= CallingConventions.HasThis;
		}		

		protected virtual void EmitInstructions(ILHelper il)
		{
			if (_gen != null)
			{
				foreach (Action<EmittedMethodBase, ILHelper> gen in _gen)
				{
					gen(this, il);
				}
			}
		}

		public void DefineMethodBody(Action<EmittedMethodBase, ILHelper> gen)
		{
			if (_gen == null) _gen = new List<Action<EmittedMethodBase, ILHelper>>();
			_gen.Add(gen);
		}

		public abstract void EmitCall(ILHelper il);

		public EmittedParameter DefineParameter(string name, TypeRef typeRef)
		{
			Contracts.Require.IsNotNull("name", name);
			
			if ((from p in _parameters
					 where String.Equals(name, p.Name)
					 select p).SingleOrDefault() != null)
				throw new InvalidOperationException(String.Format(
	@"Method already contains a parameter by the same name:
	name = {0}
	unfinished method = {1}", name, this.UnfinishedSignature()));

			EmittedParameter result = new EmittedParameter(_parameters.Count, name, typeRef);
			_parameters.Add(result);
			return result;
		}

		public EmittedParameter DefineParameter(string name, Type type)
		{
			Contracts.Require.IsNotNull("name", name);
			Contracts.Require.IsNotNull("type", type);

			return DefineParameter(name, new TypeRef(type));
		}

		public EmittedLocal DefineLocal(string name, Type type)
		{
			return DefineLocal(name, new TypeRef(type));
		}
		public EmittedLocal DefineLocal(string name, TypeRef type)
		{
			Contracts.Require.IsNotNull("name", name);
			if ((from l in _locals
					 where String.Equals(name, l.Name)
					 select l).SingleOrDefault() != null)
				throw new InvalidOperationException(String.Format(
					@"Method already contains a local by the same name:
					name = {0}
					unfinished method = {1}", name, this.UnfinishedSignature()));

			EmittedLocal result;
			_locals.Add(result = new EmittedLocal(name, _locals.Count, type));
			if (_generatorDuringCompile != null)
			{
				result.Compile(_generatorDuringCompile);
			}
			return result;
		}
		private string UnfinishedSignature()
		{
			/// TODO Output the parameters as they are defined at the time of the call.
			return String.Concat(this.TargetClass.Builder.FullName, '.', this.Name, "(...)");
		}

		protected void CompileLocals(ILGenerator il)
		{
			foreach (EmittedLocal l in _locals)
			{
				l.Compile(il);
			}
		}
		protected void CompileParameters(MethodBuilder m)
		{
			foreach (EmittedParameter p in _parameters)
			{
				p.Compile(m);
			}
		}
		protected void CompileParameters(ConstructorBuilder c)
		{
			foreach (EmittedParameter p in _parameters)
			{
				p.Compile(c);
			}
		}
		protected ILGenerator SetILGenerator(ILGenerator il)
		{
			return _generatorDuringCompile = il;			
		}
	}

	public class EmittedMethod : EmittedMethodBase
	{
		public static readonly MethodAttributes PublicInterfaceImplementationAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

		MethodInfo _checkOverride;

		public MethodBuilder Builder { get; protected set; }
		public TypeRef ReturnType { get; set; }
				
		public EmittedMethod(EmittedClass type, string name)
			: base(type, name)
		{
			this.CallingConvention = CallingConventions.Standard | CallingConventions.HasThis;
		}

		protected internal override void OnCompile()
		{
			Builder = this.TargetClass.Builder.DefineMethod(this.Name
				, this.Attributes
				, this.CallingConvention
				, (this.ReturnType != null) ? this.ReturnType.Target : null
				, this.ParameterTypes
				);
			ILGenerator il = SetILGenerator(Builder.GetILGenerator());
			try
			{
				CompileParameters(Builder);
				CompileLocals(il);
				
				ILHelper msil = new ILHelper(il);
				EmitInstructions(msil);
				if (_checkOverride != null)
				{
					if (_checkOverride.IsAbstract)
					{
						this.TargetClass.Builder.DefineMethodOverride(Builder, _checkOverride);
					}
				}
				msil.Return();
			}
			finally
			{
				SetILGenerator(null);
			}
		}

		public override void EmitCall(ILHelper il)
		{
			if (!IsCompiled) Compile();
			il.Call(Builder);
		}


		internal void CheckOverrideOnCompile(MethodInfo methodInfo)
		{
			_checkOverride = methodInfo;
		}
	}		
}
