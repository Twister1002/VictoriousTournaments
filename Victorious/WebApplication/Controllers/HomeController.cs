using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        [Route("Tournaments/{name?}")]
        public ActionResult Tournaments(String name)
        {
            return View(name);
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}