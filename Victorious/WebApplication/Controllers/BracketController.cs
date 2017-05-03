using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication.Models;
using Tournament.Structure;
using DatabaseLib;

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

        [HttpPost]
        [Route("Ajax/Bracket/MatchReset")]
        public JsonResult Reset(int bracketId, int matchNum)
        {
            bool status = false;
            String message = "No action taken.";
            object data = new { };

            if (Session["User.UserId"] != null)
            {
                BracketViewModel viewModel = new BracketViewModel(bracketId);
                MatchViewModel matchViewModel = new MatchViewModel(viewModel.Bracket.GetMatch(matchNum));

                if (viewModel.TournamentPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    List<int> matchesAffected = viewModel.MatchesAffectedList(matchNum);
                    List<object> matchResponse = new List<object>();

                    viewModel.Bracket.ResetMatchScore(matchNum);

                    foreach (int match in matchesAffected)
                    {
                        // Remove the games associated with this match.
                        MatchViewModel matchModel = new MatchViewModel(viewModel.Bracket.GetMatch(match).Id);
                        matchModel.RemoveGames();

                        // Reset the model
                        matchModel = new MatchViewModel(viewModel.Bracket.GetMatch(match));

                        // Update this match in the database according to the reset from the bracket
                        if (!matchModel.Update())
                        {
                            return Json("Unable to update match");
                        }

                        matchResponse.Add(JsonMatchResponse(matchModel.Match, true));
                    }

                    status = true;
                    message = "Matches are reset";
                    data = matchResponse;
                }
                else
                {
                    message = "You do not have permission to do this";
                }
            }
            else
            {
                message = "You must login to do this";
            }

            return Json(JsonConvert.SerializeObject(
                new
                {
                    status = status,
                    message = message,
                    data = data
                }
            ));
        }

        [HttpPost]
        [Route("Ajax/Bracket/Standings")]
        public JsonResult Standings(String jsonData)
        {
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            TournamentViewModel viewModel = new TournamentViewModel(json["tournamentId"]);
            viewModel.ProcessTournament();
            IBracket bracket = viewModel.Tourny.Brackets[json["bracketNum"]];

            return Json(JsonConvert.SerializeObject(
                new
                {
                    status = true,
                    data = new
                    {
                        ranks = bracket.Rankings
                    }
                }
            ));
        }
    }
}