namespace ATTi.Core.Configuration
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;

	using ATTi.Core.Properties;
	using ATTi.Core.Reflection;

	[XmlType("instance")]
	public class XmlSerializedInstance
	{
		

		public const string PropertyName_caseSensitive = "caseSensitive";
		public const string PropertyName_instance = "instance";
		public const string PropertyName_type = "type";

		[XmlAttributeAttribute(XmlSerializedInstance.PropertyName_caseSensitive)]
		public bool CaseSensitive;
		[XmlAnyElement(XmlSerializedInstance.PropertyName_instance)]
		public XmlNode InstanceNode;
		[XmlAttributeAttribute(XmlSerializedInstance.PropertyName_type)]
		public string Type;

		private object _instance;
		private Type _resolvedType;

		

		#region Constructors

		public XmlSerializedInstance()
		{
		}

		public XmlSerializedInstance(object instance)
		{
			Contracts.Require.IsNotNull("instance", instance);

			this.Type = instance.GetType().GetSimpleAssemblyQualifiedName();
			this._instance = instance;
		}

		#endregion Constructors

		

		public object Instance
		{
			get
			{
				if (_instance == null && this.InstanceNode != null)
				{
					Type it = this.ResolvedType;

					if (this.InstanceNode != null)
					{
						if (it == null)
							throw new ApplicationException(
								String.Format(Resources.Error_ConfiguredTypeNotFound, this.Type)
									);
						string outerxml = this.InstanceNode.OuterXml
							.Replace("<instance ", String.Concat('<', it.Name))
							.Replace("</instance> ", String.Concat("</", it.Name, '>'));

						XmlSerializer serializer = new XmlSerializer(it);
						XmlTextReader reader = new XmlTextReader(new StringReader(outerxml));
						_instance = serializer.Deserialize(reader);
					}
				}
				return _instance;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Type ResolvedType
		{
			get
			{
				if (_resolvedType == null)
				{
					string typeName = this.Type;
					if (typeName != null && !string.IsNullOrEmpty(typeName))
					{
						lock (this)
						{
							if (_resolvedType == null)
							{
								this._resolvedType = System.Type.GetType(typeName, true, false);
							}
						}
					}
				}
				return this._resolvedType;
			}
		}

		
	}

	public sealed class XmlSerializedInstanceConfiguraionElement : ConfigurationElement
	{
		

		public const string PropertyName_caseSensitive = "caseSensitive";
		public const string PropertyName_instanceConfig = "instanceConfig";
		public const string PropertyName_type = "type";

		private object _instance;
		private Type _resolvedType;
		private string _targetNodeXml;

		

		

		[ConfigurationProperty(XmlSerializedInstanceConfiguraionElement.PropertyName_caseSensitive
			, IsRequired = false, DefaultValue = true)]
		public bool CaseSensitive
		{
			get { return (bool)this[PropertyName_caseSensitive]; }
			set { this[PropertyName_caseSensitive] = value; }
		}

		public object Instance
		{
			get
			{
				if (_instance == null)
				{
					if (!string.IsNullOrEmpty(_targetNodeXml))
					{
						if (ResolvedType == null)
							throw new ApplicationException(
								String.Format(Resources.Error_ConfiguredTypeNotFound, this.TypeName)
								);
						XmlSerializer serializer = new XmlSerializer(this.ResolvedType);
						XmlTextReader reader = new XmlTextReader(new StringReader(_targetNodeXml));
						_instance = serializer.Deserialize(reader);
						if (_instance != null && !_resolvedType.IsInstanceOfType(_instance))
							throw new ConfigurationErrorsException(
								String.Format(Resources.Error_InvalidOrIncompleteTypeReference, _instance.GetType())
								);
					}
				}
				return _instance;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[ConfigurationProperty(XmlSerializedInstanceConfiguraionElement.PropertyName_instanceConfig)]
		public XmlSerializedInstance InstanceConfig
		{
			get { return (XmlSerializedInstance)this[PropertyName_instanceConfig]; }
			set { this[PropertyName_instanceConfig] = value; }
		}

		public Type ResolvedType
		{
			get
			{
				if (_resolvedType == null)
				{
					string typeName = this.TypeName;
					if (typeName != null && !string.IsNullOrEmpty(typeName))
					{
						lock (this)
						{
							if (_resolvedType == null)
							{
								this._resolvedType = Type.GetType(typeName, true, false);
							}
						}
					}
				}
				return this._resolvedType;
			}
		}

		[ConfigurationProperty(XmlSerializedInstanceConfiguraionElement.PropertyName_type
			, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[PropertyName_type]; }
			set
			{
				this[PropertyName_type] = value;
				_resolvedType = null;
				_instance = null;
			}
		}

		

		

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			Type rt = this.ResolvedType;
			if (String.Equals(elementName, rt.Name))
			{
				_targetNodeXml = reader.ReadOuterXml();
				return true;
			}
			else
			{
				return base.OnDeserializeUnrecognizedElement(elementName, reader);
			}
		}

		
	}
}