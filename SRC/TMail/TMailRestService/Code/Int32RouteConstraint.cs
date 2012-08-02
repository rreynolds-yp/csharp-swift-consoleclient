using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TMailRestService
{
	public class Int32RouteConstraint : IRouteConstraint
	{
		#region IRouteConstraint Members

		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var value = values[parameterName];
			if (typeof(Int32).IsInstanceOfType(value)) return true;
			if (typeof(String).IsInstanceOfType(value))
			{
				int l;
				return int.TryParse(value as String, out l);
			}
			return false;
		}

		#endregion
	}
}
