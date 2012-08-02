namespace TMailRestService
{

	using System;
	using System.IO;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using ATTi.Core;
	using ATTi.SSO.Client;
	using Newtonsoft.Json;
	using TMailRestService.Code;

	public static class ControllerExtensions
	{
		public static ActionResult PerformSecureAction(this Controller controller, Func<ActionResult> action)
		{
			if (!AuthenticationUtil.TryValidateAuthenticationForRequest(controller.HttpContext))
			{
				controller.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				//controller.Response.SubStatusCode = 1;
				var json = new JsonResult();
				json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
				json.Data = new { Message = "Please acquire a valid SSO auth-ticket and resubmit your request." };
				return json;
			}
			else
			{
				return action();
			}
		}

		private static ActionResult ErrorResult(this Controller controller, HttpStatusCode statusCode, object msg, Exception err)
		{
			controller.Response.StatusCode = (int)statusCode;
			controller.Response.TrySkipIisCustomErrors = true;
			if (msg != null)
			{
				var json = new JsonResult();
				json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
				json.Data = (controller.Request.IsLocal && err != null) ? new { Error = msg, Exception = err.FormatForLogging() } : json.Data = msg;
				return json;
			}
			else return new EmptyResult();
		}

		public static ActionResult Result_400_BadRequest(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.BadRequest, msg, err);			
		}

		public static ActionResult Result_401_Unauthorized(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.Unauthorized, msg, err);
		}

		public static ActionResult Result_404_NotFound(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.NotFound, msg, err);
		}

		/// <summary>
		/// Indicate that the client imposed restrictions that can't be met. Example is an accept header that doesn't
		/// include oure representation type.
		/// </summary>
		/// <param name="controller"></param>
		/// <returns></returns>
		public static ActionResult Result_406_NotAcceptable(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.NotAcceptable, msg, err);
		}

		public static ActionResult Result_409_Conflict(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.Conflict, msg, err);
		}

		public static ActionResult Result_410_Gone(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.Gone, msg, err);
		}

		public static ActionResult Result_500_InternalServerError(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.InternalServerError, msg, err);
		}

		public static ActionResult Result_503_ServiceUnavailable(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.ServiceUnavailable, msg, err);
		}

		public static ActionResult Result_501_NotImplemented(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.NotImplemented, msg, err);
		}

		public static ActionResult Result_504_GatewayTimeout(this Controller controller, object msg, Exception err)
		{
			return controller.ErrorResult(HttpStatusCode.GatewayTimeout, msg, err);
		}

		public static ActionResult Result_200_HealthTxtAvailable(this Controller controller, string path)
		{
			ActionResult ret = new EmptyResult();
			if (!string.IsNullOrEmpty(path))
			{
				controller.Response.StatusCode = (int)HttpStatusCode.OK;
				controller.Response.TrySkipIisCustomErrors = true;
				DownloadResult result = new DownloadResult(path,"health.txt");
				result.ExecuteResult(controller.ControllerContext);
				return result;
			}
			else
				controller.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return ret;			
		}
				
		public static bool TryDeserializePostData<T>(this Controller controller, out ResultEncoding encoding, out T data)
			where T: class
		{
			if (controller.Request.ContentType.Contains("application/json"))
			{
				using(var reader = new StreamReader(controller.Request.InputStream))
				{
					var postData = PerformTokenReplacement(controller.Request, reader.ReadToEnd());
					if (!String.IsNullOrEmpty(postData))
					{
						data = JsonConvert.DeserializeObject<T>(postData);
						if (data != null)
						{
							encoding = ResultEncoding.Json;
							return true;
						}
					}
				}
			}
			encoding = default(ResultEncoding);
			data = default(T);
			return false;
		}

		public static bool TryDeserializePostData<T>(this Controller controller, out ResultEncoding encoding, out T data, Func<T> makeDefault)
		{
			if (controller.Request.ContentType.Contains("application/json"))
			{
				using (var reader = new StreamReader(controller.Request.InputStream))
				{
					var postData = PerformTokenReplacement(controller.Request, reader.ReadToEnd());
					if (!String.IsNullOrEmpty(postData))
					{
						data = JsonConvert.DeserializeObject<T>(postData);
						encoding = ResultEncoding.Json;
						return true;
					}
				}
			}
			encoding = ResultEncoding.Json;
			data = makeDefault();
			return false;
		}

		public static bool TryDeserializePostData<T>(this Controller controller, out ResultEncoding encoding, out T data, T defa)
		{
			if (controller.Request.ContentType.Contains("application/json"))
			{
				using (var reader = new StreamReader(controller.Request.InputStream))
				{
					var postData = PerformTokenReplacement(controller.Request, reader.ReadToEnd());
					if (!String.IsNullOrEmpty(postData))
					{
						data = JsonConvert.DeserializeObject<T>(postData);
						encoding = ResultEncoding.Json;
						return true;
					}
				}
			}
			encoding = ResultEncoding.Json;
			data = defa;
			return false;
		}


		static readonly string CToken_UserHostAddress = "$(UserHostAddress)";
		static readonly string CToken_UserHostName = "$(UserHostName)";
		static readonly string CToken_ServerTimestamp = "$(ServerTimestamp)";
		

		private static string PerformTokenReplacement(HttpRequestBase request, string input)
		{
			if (String.IsNullOrEmpty(input)) return input;
			
			var result = input;

			if (result.Contains("$("))
			{
				if (result.Contains(CToken_UserHostAddress))
					result = result.Replace(CToken_UserHostAddress, (request.IsLocal) ? "localhost-ip-unknown" : request.UserHostAddress.Replace(":", " "));

				if (result.Contains(CToken_UserHostName))
					result = result.Replace(CToken_UserHostName, (request.IsLocal) ? "localhost" : request.UserHostName.Replace(":", " "));

				if (result.Contains(CToken_ServerTimestamp))
					result = result.Replace(CToken_ServerTimestamp, DateTime.UtcNow.ToString("u"));

			}
			return result;
		}		
	}
}
