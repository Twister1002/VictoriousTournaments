using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : VictoriousController
    {
        // GET: Home
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            return View(model);
        }
    }
}