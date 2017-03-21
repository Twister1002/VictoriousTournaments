using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MatchController : VictoriousController
    {

        [Route("Match")]
        public ActionResult Index()
        {
            Session["Message"] = "You do not belong here";
            Session["Message.Error"] = ViewModel.ViewError.WARNING;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Match/Ajax/Match")]
        public JsonResult Match(String tournament, String match)
        {
            int tournamentId;
            int matchId;

            if (int.TryParse(tournament, out tournamentId) && int.TryParse(match, out matchId))
            {
                MatchViewModel viewModel = new MatchViewModel(tournamentId, matchId);
                var html = PartialView("_MatchUpdate", viewModel);

                return Json(html);
            }

            return Json("Unable to process information that was given");
        }

        [HttpPost]
        [Route("Match/Ajax/Update")]
        public JsonResult Update(JsonObject FormData)
        {
            // Verify session data
            if (Session["User.UserId"] != null)
            {
                int tournamentId = -1;
                int matchId = -1;
                int bracketId;

                
                    
                // Get the tournament data
                MatchViewModel viewModel = new MatchViewModel(tournamentId, matchId);
                
            }

            return Json("");
        }
    }
}