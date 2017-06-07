﻿using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : VictoriousController
    {

        [Route("")]
        public ActionResult Index()
        {
            Home model = new Home(work);

            return View("Index", model);
        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            Home model = new Home(work);
            return View("FAQ", model);
        }

        [Route("Blog")]
        public ActionResult Blog()
        {
            Session["Message"] = "The blog is currently not working.";
            Session["Message.Class"] = ViewError.ERROR;
            return RedirectToAction("Index");
        }

        [Route("About")]
        public ActionResult About()
        {
            Home model = new Home(work);
            return View("About", model);
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            Home model = new Home(work);
            return View("Contact", model);
        }

        [Route("Rules")]
        public ActionResult Rules()
        {
            Home model = new Home(work);
            return View("Rules", model);
        }
    }
}