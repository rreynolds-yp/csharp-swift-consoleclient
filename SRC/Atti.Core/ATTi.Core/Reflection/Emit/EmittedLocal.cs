using System;
using System.Reflection.Emit;

namespace ATTi.Core.Reflection.Emit
{	
	/// <summary>
	/// Helper class for working with local variables in the IL stream.
	/// </summary>
	public class EmittedLocal : IValueRef
	{
		LocalBuilder _builder;
		public string Name { get; private set; }
		public int LocalIndex { get; private set; }
		public TypeRef LocalType { get; private set; }

		public EmittedLocal(string name, int index, TypeRef localType)			
		{
			Name = name;
			LocalIndex = index;
			LocalType = localType;
		}

		public void Compile(ILGenerator il)
		{
			if (_builder == null)
			{
				_builder = il.DeclareLocal(LocalType.Target, false);
#if DEBUG
				//_builder.SetLocalSymInfo(Name);
#endif
			}			
		}

		public void LoadValue(ILHelper il)
		{
			if (_builder == null) throw new InvalidOperationException("EmittedLocal not compiled: {0}");
			il.LoadLocal(LocalIndex);
		}

		public void StoreValue(ILHelper il)
		{
			if (_builder == null) throw new InvalidOperationException("EmittedLocal not compiled: {0}");
			il.StoreLocal(LocalIndex);
		}

		public void LoadAddress(ILHelper il)
		{
			if (_builder == null) throw new InvalidOperationException("EmittedLocal not compiled: {0}");
			il.LoadLocalAddress(LocalIndex);
		}
	}
	
}
