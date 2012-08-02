using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ATTi.Core.Reflection.Emit
{
	public class EmittedGenericArgument
	{
		List<TypeRef> _interfaceConstraints = new List<TypeRef>();
		TypeRef _baseTypeConstraint;
		
		public string Name { get; set; }
		public int Position { get; set; }
		public GenericParameterAttributes Attributes { get; set; }

		public void AddInterfaceConstraint(Type typ)
		{
			Contracts.Require.IsNotNull("typ", typ);
			_interfaceConstraints.Add(new TypeRef(typ));
		}
		public void AddInterfaceConstraint(TypeRef typeRef)
		{
			Contracts.Require.IsNotNull("typeRef", typeRef);
			Contracts.Invariant.IsNull("BaseTypeConstraint", _baseTypeConstraint);
			_interfaceConstraints.Add(typeRef);
		}

		public void AddBaseTypeConstraint(Type type)
		{
			Contracts.Require.IsNotNull("type", type);
			Contracts.Invariant.IsNull("BaseTypeConstraint", _baseTypeConstraint);
			_baseTypeConstraint = new TypeRef(type);
		}
		public void AddBaseTypeConstraint(TypeRef typeRef)
		{
			Contracts.Require.IsNotNull("typeRef", typeRef);
			Contracts.Invariant.IsNull("BaseTypeConstraint", _baseTypeConstraint);
			_baseTypeConstraint = typeRef;
		}

		internal void FinishDefinition(GenericTypeParameterBuilder arg)
		{
			Contracts.Require.IsNotNull("arg", arg);
			arg.SetGenericParameterAttributes(Attributes);
			if (_baseTypeConstraint != null)
			{
				arg.SetBaseTypeConstraint(_baseTypeConstraint.Target);
			}
			if (_interfaceConstraints.Count > 0)
			{
				arg.SetInterfaceConstraints((from i in _interfaceConstraints
																		 select i.Target).ToArray());
			}
		}
	}
}
