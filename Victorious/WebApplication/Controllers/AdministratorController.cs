using DataLib;
using Newtonsoft.Json;
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
            dynamic jsonReturn = new { status = false, message = "No action was taken" };

            if (IsAdministrator())
            {
                Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                GameModel gameModel = new GameModel();
                DbError result = DbError.NONE;

                if (json["function"] == "add")
                {
                    gameModel.Title = json["title"];
                    result = db.AddGame(gameModel);
                    gameModel = db.GetAllGames().First(x => x.Title == json["title"]);
                }
                else if (json["function"] == "delete")
                {
                    gameModel = db.GetAllGames().First(x => x.Title == json["title"]);
                    result = db.DeleteGame(gameModel);
                }

                if (result == DbError.SUCCESS)
                {
                    jsonReturn = new {
                        status = true,
                        function = json["function"],
                        message = "Was able to " + json["function"] + " successfully",
                        data = new
                        {
                            model = gameModel
                        }
                    };
                }
                else
                {
                    jsonReturn = new
                    {
                        status = false,
                        message = "An error occured while taking action",
                        Exception = db.interfaceException.Message
                    };
                }
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
            UserModel userModel = db.GetUserById((int)Session["User.UserId"]);

            return userModel.SitePermission.Permission;
        }
    }
}