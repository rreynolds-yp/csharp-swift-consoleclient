using System;
using System.Diagnostics;



namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with class members in the IL stream.
	/// </summary>
	public abstract class EmittedMember
	{
		public EmittedClass TargetClass { get; private set; }
		public bool IsCompiled { get; private set; }
		public string Name { get; protected set; }
		public abstract bool IsStatic { get; }

		protected EmittedMember(EmittedClass type, string name)
		{
			Contracts.Require.IsNotEmpty("name", name);

			this.TargetClass = type;
			this.Name = name;
		}

		internal void Compile()
		{
			if (!this.IsCompiled)
			{
				OnCompile();
				this.IsCompiled = true;
			}
		}

		internal protected virtual void OnCompile()
		{
			throw new NotImplementedException();
		}
	}
}
