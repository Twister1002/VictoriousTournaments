using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Models;
using DatabaseLib;

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
        [Route("Ajax/Games")]
        public JsonResult Games(String jsonData)
        {
            object jsonReturn = new {
                status = false,
                message = "No action was taken"
            };

            if (IsAdministrator())
            {
                Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                AdministratorViewModel adminModel = new AdministratorViewModel();
                GameTypeModel gameType;
                DbError result = DbError.NONE;

                //if (json["function"] == "add")
                //{
                //    gameType = new GameType()
                //    {
                //        Title = json["title"],
                //    };

                //    adminModel.CreateGame(gameType);
                //}
                //else if (json["function"] == "delete")
                //{
                   
                //}

                //if (result == DbError.SUCCESS)
                //{
                //    jsonReturn = new {
                //        status = true,
                //        function = json["function"],
                //        message = "Was able to " + json["function"] + " successfully",
                //        data = new
                //        {
                //            model = gameModel
                //        }
                //    };
                //}
                //else
                //{
                //    jsonReturn = new
                //    {
                //        status = false,
                //        message = "An error occured while taking action",
                //        Exception = db.interfaceException.Message
                //    };
                //}
            }

            return Json(JsonConvert.SerializeObject(jsonReturn));
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
            AccountViewModel userModel = new AccountViewModel((int)Session["User.UserId"]);

            return (Permission)userModel.Model.PermissionLevel;
        }
    }
}