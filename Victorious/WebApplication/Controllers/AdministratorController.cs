using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Models;
using DatabaseLib;
using WebApplication.Models.Administrator;

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

        [HttpPost]
        [Route("Ajax/Administrator/Games")]
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
                GameTypeViewModel gameType = new GameTypeViewModel();
                bool result = false;

                switch (json["function"])
                {
                    case "add":
                        gameType.Title = json["title"];
                        result = gameType.Create();
                        break;
                    case "delete":
                        result = gameType.Delete(int.Parse(json["gameid"]));
                        break;
                }

                jsonReturn = new
                {
                    status = result,
                    message = "Was able to " + json["function"] + " " + (result ? "successfully" : "unsuccessfully"),
                    data = gameType.Select()
                };
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

            return (Permission)userModel.Account.PermissionLevel;
        }
    }
}