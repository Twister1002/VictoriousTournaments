using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AdministratorController : VictoriousController
    {
        // GET: Administrator
        [Route("Administrator")]
        public ActionResult Index()
        {

            return View("Index", new AdministratorViewModel());
        }
    }
}