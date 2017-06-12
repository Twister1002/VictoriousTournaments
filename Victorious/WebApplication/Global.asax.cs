using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequst(object sender, EventArgs e)
        {
            Response.AddHeader("Cache-Control", "max-age=0,no-cache,no-store,must-revalidate");
            Response.AddHeader("Pragma", "cache");
            Response.AddHeader("Expires", "Tue, 01 Jan 1970 00:00:00 GMT");
        }
    }
}
