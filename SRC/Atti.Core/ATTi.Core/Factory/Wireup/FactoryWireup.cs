using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ATTi.Core.Factory.Wireup
{
	public interface IFactoryWireup
	{
		Type TargetType { get; set; }
		InstanceReusePolicy Reuse { get; set; }
		SingletonReuseScope SingletonScope { get; set; }
	}	
}
