using System;
using System.Reflection;



namespace ATTi.Core.Reflection.Emit
{
	internal interface IPropertyRef : IValueRef
	{
		PropertyInfo GetPropertyInfo();
	}

	internal class RawPropertyRef : IPropertyRef
	{
		PropertyInfo _prop;

		public RawPropertyRef(PropertyInfo prop)
		{
			Contracts.Require.IsNotNull("prop", prop);
			
			_prop = prop;
		}
		public string Name { get { return _prop.Name; } }
		public PropertyInfo GetPropertyInfo()
		{
			return _prop;
		}
		public void LoadValue(ILHelper il)
		{
			Contracts.Require.IsNotNull("il", il);

			il.LoadProperty(_prop, false);
		}
		public void StoreValue(ILHelper il)
		{
			Contracts.Require.IsNotNull("il", il);

			il.StoreProperty(_prop, false);
		}
		public void LoadAddress(ILHelper il)
		{
			throw new NotImplementedException();
		}
	}
}
