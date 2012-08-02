using System.Web.Mvc;
using System.Web.Routing;
using ATTi.Core.Trace;

namespace TMailRestService
{
	public class MvcApplication : System.Web.HttpApplication, ITraceable
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("get-mailing", "1/mailings/{app}/{env}/{id}", new { controller = "Mailings", action = "GetMailing" }
				, new { id = new GuidRouteConstraint() }
				);
			routes.MapRoute("post-mailing", "1/mailings/{app}/{env}", new { controller = "Mailings", action = "PostMailing" });
			routes.MapRoute("get-mailings", "1/mailings/{app}/{env}", new { controller = "Mailings", action = "GetMailings" });
         	routes.MapRoute(
					"Default", // Route name
					"{controller}/{action}/{id}", // URL with parameters
					new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			MvcHandler.DisableMvcResponseHeader = true; 

			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);
		}

	}
}