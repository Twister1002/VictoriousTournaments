using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BracketController : VictoriousController
    {
        [Route("Bracket")]
        // GET: Bracket
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [Route("Ajax/Bracket/Reset")]
        [HttpPost]
        public JsonResult Reset(int bracketId)
        {
            bool status = false;
            String message = "No action taken";
            String redirect = Url.Action("Tournament", "Tournament");

            object jsonResult = new { status = false, message = "No actiont taken" };
            if (Session["User.UserId"] != null)
            {
                BracketViewModel viewModel = new BracketViewModel(bracketId);
                if ((int)Session["User.UserId"] == viewModel.Model.Tournament.CreatedByID)
                {
                    if (viewModel.ResetBracket())
                    {
                        status = true;
                        message = "Bracket was reset";
                        redirect = Url.Action("Tournament", "Tournament", new { guid = viewModel.Model.Tournament.TournamentID });
                    }
                    else
                    {
                        status = false;
                        message = "Could not reset the bracket due to an error";
                        redirect = Url.Action("Tournament", "Tournament", new { guid = viewModel.Model.Tournament.TournamentID });
                    }
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = viewModel.Model.Tournament.TournamentID });
                }
            }
            else
            {
                status = false;
                message = "You must login to do this";
                redirect = Url.Action("Login", "Account");
            }

            Session["Message"] = message;
            Session["Message.Class"] = status ? ViewModel.ViewError.SUCCESS : ViewModel.ViewError.WARNING;

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                redirect = redirect
            }
            ));
        }
    }
}