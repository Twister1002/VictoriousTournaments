using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tournament.Structure;
using WebApplication.Models;
using DatabaseLib;

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
        [Route("Ajax/Match")]
        public JsonResult MatchInfo(String jsonData)
        {
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            MatchViewModel viewModel = new MatchViewModel(json["matchId"]);
            
            String jsonResult = JsonConvert.SerializeObject(new {
                status = true,
                data = JsonMatchResponse(viewModel.Match, true)
            });

            return Json(jsonResult);
        }

        [HttpPost]
        [Route("Ajax/Match/Update")]
        public JsonResult MatchUpdate(String jsonIds, List<GameViewModel> games)
        {
            bool status = false;
            String message = "No action taken";
            object data = new { };

            if (Session["User.UserId"] != null)
            {
                Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonIds);
                TournamentViewModel tournamentModel = new TournamentViewModel(json["tournamentId"]);

                if (tournamentModel.IsAdministrator((int)Session["User.UserId"]))
                {
                    tournamentModel.ProcessTournament();
                    IBracket bracket = tournamentModel.Tourny.Brackets.ElementAt(json["bracketNum"]);
                    IMatch match = bracket.GetMatch(json["matchNum"]);
                    BracketViewModel bracketModel = new BracketViewModel(bracket);
                    //MatchViewModel matchModel = new MatchViewModel(match);
                    Dictionary<int, bool> processed = new Dictionary<int, bool>();

                    // Verify these matches exists
                    foreach (GameViewModel gameModel in games)
                    {
                        if (match.IsFinished || gameModel.ChallengerScore == gameModel.DefenderScore)
                        {
                            processed.Add(gameModel.GameNumber, false);
                            continue;
                        }

                        if (!match.Games.Any(x=>x.GameNumber == gameModel.GameNumber))
                        {
                            // We need to add this game.
                            PlayerSlot winner = gameModel.DefenderScore > gameModel.ChallengerScore ? PlayerSlot.Defender : PlayerSlot.Challenger;
                            GameModel addedGameModel = bracket.AddGame(match.MatchNumber, gameModel.DefenderScore, gameModel.ChallengerScore, winner);
                        }
                        else
                        {
                            processed.Add(gameModel.GameNumber, false);
                        }
                    }
                    
                    //  Load the Models
                    MatchViewModel matchModel = new MatchViewModel(bracket.GetMatchModel(match.MatchNumber));
                    MatchViewModel winnerMatchModel = matchModel.Match.NextMatchNumber != -1 ? new MatchViewModel(bracket.GetMatchModel(matchModel.Match.NextMatchNumber)) : null;
                    MatchViewModel loserMatchModel = matchModel.Match.NextLoserMatchNumber != -1 ? new MatchViewModel(bracket.GetMatchModel(matchModel.Match.NextLoserMatchNumber)) : null;

                    // Update the bracket in the database
                    //DbError bracketUpdate = db.UpdateBracket(bracketModel.Model);

                    // Update the matches in the database
                    object currentMatchData = null;
                    object winnerMatchData = null;
                    object loserMatchData = null;
                    bool currentMatchUpdate = matchModel.Update();
                    bool winnerMatchUpdate = winnerMatchModel != null ? winnerMatchModel.Update() : false;
                    bool loserMatchUpdate = loserMatchModel != null ? loserMatchModel.Update() : false;

                    if (currentMatchUpdate)
                    {
                        status = true;
                        message = "Current match was updated";

                        currentMatchData = JsonMatchResponse(matchModel.Match, false);
                    }
                    if (winnerMatchUpdate)
                    {
                        winnerMatchData = JsonMatchResponse(winnerMatchModel.Match, false);
                    }
                    if (loserMatchUpdate)
                    {
                        loserMatchData = JsonMatchResponse(loserMatchModel.Match, false);
                    }

                    // Prepare data
                    data = new
                    {
                        processed = processed,
                        currentMatch = currentMatchData,
                        winnerMatch = winnerMatchData,
                        loserMatch = loserMatchData
                    };
                }
                else
                {
                    message = "You are not authorized to do this.";
                }
            }
            else
            {
                message = "You must login to do this action."; 
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = data
            }));
        }
    }
}