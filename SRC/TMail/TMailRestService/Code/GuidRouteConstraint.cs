using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TMailRestService
{
	public class GuidRouteConstraint : IRouteConstraint
	{
		#region IRouteConstraint Members

		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var value = values[parameterName];
			if (value is Guid) return true;
			if (value is String && !String.IsNullOrEmpty((string)value))
			{
				try
				{
					var guid = new Guid((string)value);
					return true;
				}
				catch
				{ 
					// TODO: Relying on a parse exception is costly, implement a try-parse method instead.
				}
			}
			return false;
		}

		#endregion
	}
}
