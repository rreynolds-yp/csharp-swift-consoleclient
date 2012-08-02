using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ATTi.Core.Configuration;
using ATTi.Core.Properties;
using ATTi.Core.Reflection;
using ATTi.Core.Trace;

namespace ATTi.Core.Factory
{
	[Serializable]
	[XmlType("factoryBinding")]
	public class FactoryBinding : IFactoryBinding, ITraceable
	{
		public const string PropertyName_type = "type";
		public const string PropertyName_factoryType = "factoryType";
		public const string PropertyName_strategyType = "strategyType";
		public const string PropertyName_implementationType = "implementationType";
		public const string PropertyName_instanceConfig = "instanceConfig";

		Type _type;
		Type _factoryType;
		Type _implementationType;
		XmlSerializedInstance _instanceConfig;

		[XmlAttribute(FactoryBinding.PropertyName_type)]
		public string TypeName
		{
			get { return _type == null ? String.Empty : _type.GetSimpleAssemblyQualifiedName(); }
			set
			{
				try
				{
					if (String.IsNullOrEmpty(value)) _type = null;
					else _type = Type.GetType(value, false);
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, () =>
					{
						return new object[] {
						String.Format(Resources.Error_FactoryBindingTypeLoadException, PropertyName_type
						, value)
						, e };
					});
					throw;
				}
			}
		}
		[XmlAttribute(FactoryBinding.PropertyName_factoryType)]
		public string FactoryTypeName
		{
			get { return _factoryType == null ? String.Empty : _factoryType.GetSimpleAssemblyQualifiedName(); }
			set
			{
				try
				{
					if (String.IsNullOrEmpty(value)) _factoryType = null;
					else _factoryType = Type.GetType(value,  false);
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, () =>
					{
						return new object[] {
						String.Format(Resources.Error_FactoryBindingTypeLoadException, PropertyName_factoryType
						, value)
						, e };
					});
					throw;
				}
			}
		}
		
		//[XmlAttribute(FactoryBinding.PropertyName_strategyType)]
		//[DataMember(Name = FactoryBinding.PropertyName_strategyType)]
		//public FactoryStrategy StrategyTypeName
		//{
		//  get { return _strategyType; }
		//  set	{ _strategyType = value; }
		//}

		[XmlAttribute(FactoryBinding.PropertyName_implementationType)]
		public string ImplementationTypeName
		{
			get { return _implementationType == null ? String.Empty : _implementationType.GetSimpleAssemblyQualifiedName(); }
			set
			{
				try
				{
					if (String.IsNullOrEmpty(value)) _implementationType = null;
					else _implementationType = Type.GetType(value, false);
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, () =>
					{
						return new object[] {
						String.Format(Resources.Error_FactoryBindingTypeLoadException, PropertyName_implementationType
						, value)
						, e };
					});
					throw;
				}
			}
		}

		[XmlElement(FactoryBinding.PropertyName_instanceConfig)]
		public XmlSerializedInstance InstanceConfig
		{
			get { return this._instanceConfig; }
			set
			{
				_instanceConfig = value;
			}
		}

		#region IFactoryBinding Members

		Type IFactoryBinding.Type
		{
			get
			{
				if (_type == null && !String.IsNullOrEmpty(this.TypeName))
				{
					_type = Type.GetType(this.TypeName, false);
				}
				return _type;
			}
			set { _type = value; }
		}

		Type IFactoryBinding.FactoryType
		{
			get
			{
				if (_factoryType == null && !String.IsNullOrEmpty(this.FactoryTypeName))
				{
					_factoryType = Type.GetType(this.FactoryTypeName, false);
				}
				return _factoryType;
			}
			set { _factoryType = value; }
		}

		//FactoryStrategy IFactoryBinding.Strategy
		//{
		//  get { return _strategyType;	}
		//  set { _strategyType = value; }
		//}

		Type IFactoryBinding.ConcreteType
		{
			get
			{
				if (_implementationType == null && !String.IsNullOrEmpty(this.ImplementationTypeName))
				{
					_implementationType = Type.GetType(this.ImplementationTypeName, false);
				}
				return _implementationType;
			}
			set { _implementationType = value; }
		}

		object IFactoryBinding.GetInstanceConfig()
		{
			XmlSerializedInstance ielm = this.InstanceConfig;
			return (ielm == null) ? null : ielm.Instance;
		}
		#endregion
	}

	[Serializable]
	[XmlType("factory")]
	public class FactoryBindings
	{
		[XmlArray("bindings")]
		[XmlArrayItem("add", typeof(FactoryBinding))]
		public FactoryBinding[] Bindings;
	}
}
