using System;
using ATTi.Core.Contracts;

namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// A wrapper object for a type reference.
	/// </summary>
	public class TypeRef
	{
		public static readonly TypeRef Empty = new TypeRef();

		private Type _type;
		public virtual Type Target { get { return _type; } }
		protected TypeRef() { }
		public TypeRef(Type t)
		{
			Contracts.Require.IsNotNull("t", t);

			_type = t;
		}

		public static TypeRef Instance<T>()
		{
			return new TypeRef(typeof(T));
		}
	}

	/// <summary>
	/// A specialized TypeRef for emitted types.
	/// </summary>
	public class EmittedTypeRef : TypeRef
	{
		private EmittedClass _eclass;

		[InvariantContract]
		protected void ObjectInvariant()
		{
			Contracts.Invariant.IsNotNull("_eclass", _eclass);
		}

		public override Type Target
		{
			get
			{
				if (!_eclass.IsCompiled) _eclass.Compile();
				return _eclass.Builder;
			}
		}

		public EmittedTypeRef(EmittedClass eclass) 			
		{
			Contracts.Require.IsNotNull("eclass", eclass);

			_eclass = eclass;
		}
	}
}
