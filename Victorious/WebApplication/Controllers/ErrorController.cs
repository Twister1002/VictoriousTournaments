using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class ErrorController : VictoriousController
    {
        // GET: Error
        [Route("Error")]
        [Route("Error/Error")]
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            return View("Error");
        }

        [Route("Error/NotFound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        [Route("Error/Forbidden")]
        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View("Forbidden");
        }

        [Route("Error/AccessDenied")]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 401;
            return View("AccessDenied");
        }
    }
}