using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace TMailRestService
{
    public partial class _04missing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This aspx is used by the customErrors section in the web.config
            // to return a custom 404 response, without losing the 404 status code
            Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
