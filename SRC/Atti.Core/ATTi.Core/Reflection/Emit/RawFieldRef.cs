using System.Reflection;


namespace ATTi.Core.Reflection.Emit
{
	internal class RawFieldRef : IFieldRef
	{
		FieldInfo _field;

		public RawFieldRef(FieldInfo field)
		{
			Contracts.Require.IsNotNull("field", field);

			_field = field;
		}
		public string Name { get { return _field.Name; } }
		public FieldInfo GetFieldInfo()
		{
			return _field;
		}
		public void LoadValue(ILHelper il)
		{
			Contracts.Require.IsNotNull("il", il);

			il.LoadField(_field);
		}
		public void StoreValue(ILHelper il)
		{
			Contracts.Require.IsNotNull("il", il);

			il.StoreField(_field);
		}
		public void LoadAddress(ILHelper il)
		{
			Contracts.Require.IsNotNull("il", il);

			il.LoadFieldAddress(_field);
		}
	}
	
}
