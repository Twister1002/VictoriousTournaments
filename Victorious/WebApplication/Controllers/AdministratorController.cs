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
            LoadAccount(Session);

            if (account.IsAdministrator())
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
            LoadAccount(Session);
            object jsonReturn = new {
                status = false,
                message = "No action was taken"
            };

            if (account.IsAdministrator())
            {
                Dictionary<String, String> json = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonData);
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
                    data = gameType.Select().Select(x => new { x.GameTypeID, x.Title }).ToList()
                };
            }

            return Json(JsonConvert.SerializeObject(jsonReturn));
        }

        [HttpPost]
        [Route("Ajax/Administrator/Platform")]
        public JsonResult Platform(String jsonData)
        {
            bool status = false;
            String message = "No action taken";

            Dictionary<String, String> json = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonData);
            PlatformTypeViewModel viewModel = new PlatformTypeViewModel();

            switch (json["function"])
            {
                case "add":
                    viewModel.Platform = json["Platform"];
                    status = viewModel.Create();
                    break;
                case "delete":
                    status = viewModel.Delete(int.Parse(json["PlatformId"]));
                    break;
            }

            message = "Was able to " + json["function"] + " " + (status ? "" : "un") + "successfully";


            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                platforms = viewModel.Select().Select(x => new { x.PlatformID, x.PlatformName }).ToList()
            }));
        }
    }
}