using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMailRestService
{
	public static class RequestExtensions
	{
		public static bool CanAcceptType(this HttpRequestBase request, string acceptType)
		{
			foreach (var a in request.AcceptTypes)
			{
				if (String.Equals(acceptType, a)) return true;
			}
			return false;
		}
	}
}
