using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ATTi.SSO.Client;
using ATTi.TMail.Common;

namespace TMailRestService
{
	public class RequiresSsoAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase ctx)
		{
			IPrincipal principal;
			HttpCookie ssoCookie = ctx.Request.Cookies["SSO"];
            try
            {
                if (ssoCookie != null
                    && AuthenticationUtil.TryValidateAuthenticationForRequest(ctx.Request.UserHostAddress, ssoCookie, out principal))
                {
                    ctx.User = principal;
                    Thread.CurrentPrincipal = principal;
                    return true;
                }
            }
            catch (Exception) 
            {
                // Not authorized. Either no auth-token, invalid auth-token, or expired
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ctx.Response.StatusDescription = @"Invalid/expired SSO auth-ticket encountered.";
                // ctx.Response.SubStatusCode = 1;
                ctx.Response.TrySkipIisCustomErrors = true;
                ctx.Response.Write("{\"Error\":\"Please acquire a valid SSO auth-ticket and resubmit your request.\"}");
                return false;
            }
            //finally
            //{
            //    // Not authorized. Either no auth-token, invalid auth-token, or expired
            //    ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //   // ctx.Response.SubStatusCode = 1;
            //    ctx.Response.TrySkipIisCustomErrors = true;
            //    ctx.Response.Write("{\"Error\":\"Please acquire a valid/unexpired SSO auth-ticket and resubmit your request.\"}");
            //}
            return false;
            
		}
	}
}
