using System;


namespace ATTi.Core.Factory
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class FactoryBindingAttribute : Attribute, IFactoryBinding
	{
		private Type _factoryType;
		//private FactoryStrategy _strategyType;
		private Type _implementationType;

		/// <summary>
		/// The IFactory implementation type bound by the attribute.
		/// </summary>
		public Type FactoryType
		{
			get { return _factoryType; }
			set
			{
				Contracts.Require.IsAssignableFrom<IFactory>("FactoryType", value);
				_factoryType = value;
			}
		}

		public Type ConcreteType
		{
			get { return _implementationType; }
			set
			{
				_implementationType = value;
			}
		}

		Type IFactoryBinding.Type
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		object IFactoryBinding.GetInstanceConfig()
		{
			return null;
		}
	}

}
