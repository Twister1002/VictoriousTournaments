using DataLib;
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
            if (IsAdministrator())
            {
                return View("Index", new AdministratorViewModel());
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [Route("Administrator/Games")]
        public ActionResult Games()
        {
            if (IsAdministrator())
            {
                return View("Games", new AdministratorViewModel());
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }


        [HttpPost]
        [Route("Administrator/Ajax/Games")]
        public JsonResult Games(String jsonData)
        {
            return Json("No functionality for this has been made");
        }

        private bool IsAdministrator()
        {
            if (Session["User.UserId"] != null && UserPermission() == Permission.SITE_ADMINISTRATOR)
            {
                return true;
            }
            else
            {
                Session["Message"] = "You do not have access to see this.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;

                return false;
            }
        }

        public Permission UserPermission()
        {
            UserModel userModel = db.GetUserById((int)Session["User.UserId"]);

            return userModel.SitePermission.Permission;
        }
    }
}