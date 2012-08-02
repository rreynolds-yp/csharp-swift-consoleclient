using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TMailRestService
{
	public class Int64RouteConstraint : IRouteConstraint
	{
		#region IRouteConstraint Members

		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var value = values[parameterName];
			if (typeof(Int64).IsInstanceOfType(value)) return true;
			if (typeof(String).IsInstanceOfType(value))
			{
				Int64 l;
				return Int64.TryParse(value as String, out l);
			}
			return false;
		}

		#endregion
	}
}
