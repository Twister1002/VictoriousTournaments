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
                if (viewModel.IsCreator((int)Session["User.UserId"]))
                {
                    viewModel.Bracket.ResetMatches();

                    status = true;
                    message = "Bracket was reset";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = viewModel.Model.Tournament.TournamentID });
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
            LoadAccount(Session);
            bool status = false;
            String message = "No action taken.";
            object data = new { };

            if (account != null)
            {
                BracketViewModel viewModel = new BracketViewModel(bracketId);

                if (viewModel.IsAdministrator(account.AccountId))
                {
                    List<int> matchesAffected = viewModel.MatchesAffectedList(matchNum);
                    List<object> matchResponse = new List<object>();

                    GameViewModel gameModel;
                    List<GameModel> games = viewModel.Bracket.ResetMatchScore(matchNum);

                    // Remove the games from the current match.
                    foreach (GameModel game in games)
                    {
                        // Delete the games
                        gameModel = new GameViewModel(game);
                        gameModel.Delete();
                    }

                    foreach (int match in matchesAffected)
                    {
                        matchResponse.Add(JsonMatchResponse(viewModel.Bracket.GetMatch(match), true));
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
            IBracket bracket = viewModel.Tourny.Brackets[json["bracketNum"]];

            return Json(JsonConvert.SerializeObject(
                new
                {
                    status = true,
                    usePoints = (bracket.BracketType == BracketType.DOUBLE || bracket.BracketType == BracketType.SINGLE ? false : true),
                    data = new
                    {
                        ranks = bracket.Rankings
                    }
                }
            ));
        }
    }
}