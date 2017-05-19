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
            bool status = false;
            String message = "Match doesn't exist or failed to load";

            if (viewModel.Model != null)
            {
                status = true;
                message = "Match was loaded.";
            }

            String jsonResult = JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
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

            if (games != null)
            {

                LoadAccount(Session);
                if (account != null)
                {
                    Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonIds);
                    TournamentViewModel tournamentModel = new TournamentViewModel(json["tournamentId"]);

                    if (tournamentModel.IsAdministrator(account.AccountId))
                    {
                        BracketViewModel bracketModel = new BracketViewModel(tournamentModel.Tourny.Brackets.ElementAt(json["bracketNum"]));
                        MatchViewModel matchModel = new MatchViewModel(bracketModel.Bracket.GetMatch(json["matchNum"]));
                        Dictionary<int, bool> processed = new Dictionary<int, bool>();

                        // Verify these matches exists
                        foreach (GameViewModel gameModel in games)
                        {
                            // Tie game check
                            if (matchModel.Match.IsFinished || gameModel.ChallengerScore == gameModel.DefenderScore)
                            {
                                processed.Add(gameModel.GameNumber, false);
                                continue;
                            }

                            if (!matchModel.Match.Games.Any(x => x.GameNumber == gameModel.GameNumber))
                            {
                                // We need to add this game.
                                PlayerSlot winner = gameModel.DefenderScore > gameModel.ChallengerScore ? PlayerSlot.Defender : PlayerSlot.Challenger;
                                GameModel addedGameModel = bracketModel.Bracket.AddGame(matchModel.Match.MatchNumber, gameModel.DefenderScore, gameModel.ChallengerScore, winner);
                            }
                            else
                            {
                                processed.Add(gameModel.GameNumber, false);
                            }
                        }
                        
                        //  Load the Models
                        matchModel.ReloadModel(bracketModel.Bracket.GetMatchModel(matchModel.Match.MatchNumber));

                        // Update the matches in the database
                        object currentMatchData = null;
                        object winnerMatchData = null;
                        object loserMatchData = null;
                        bool currentMatchUpdate = matchModel.Update();

                        if (currentMatchUpdate)
                        {
                            status = true;
                            message = "Current match was updated";

                            currentMatchData = JsonMatchResponse(matchModel.Match, false);
                            if (matchModel.Match.NextMatchNumber != -1)
                                winnerMatchData = JsonMatchResponse(bracketModel.Bracket.GetMatch(matchModel.Match.NextMatchNumber), false);
                            if (matchModel.Match.NextLoserMatchNumber != -1) 
                                loserMatchData = JsonMatchResponse(bracketModel.Bracket.GetMatch(matchModel.Match.NextLoserMatchNumber), false);
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
            }
            else
            {
                message = "No games were received.";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = data
            }));
        }

        [HttpPost]
        [Route("Ajax/Match/RemoveGame")]
        public JsonResult RemoveGame(String jsonData)
        {
            bool status = false;
            String message = "No action taken";
            object data = new { };

            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            BracketViewModel bracketViewModel = new BracketViewModel(json["bracketId"]);
            List<int> matchesAffected = bracketViewModel.MatchesAffectedList(json["matchNum"]);
            List<object> matchDataAffected = new List<object>();

            GameModel gameRemoved = bracketViewModel.Bracket.RemoveGameNumber(json["matchNum"], json["gameNum"]);
            GameViewModel removeGameViewModel = new GameViewModel(gameRemoved);
            if (removeGameViewModel.Delete())
            {
                status = true;
                message = "Matches are affected.";

                foreach (int matchNum in matchesAffected)
                {
                    // Load the original and load one from the bracket
                    MatchViewModel modified = new MatchViewModel(bracketViewModel.Bracket.GetMatchModel(matchNum));
                    MatchViewModel original = new MatchViewModel(modified.Match.Id);

                    List<IGame> games = original.Match.Games.Where(x => !modified.Match.Games.Any(y => y.Id == x.Id)).ToList();
                    foreach (IGame game in games)
                    {
                        GameViewModel gameViewModel = new GameViewModel(game);
                        gameViewModel.Delete();
                    }

                    modified.Update();
                    matchDataAffected.Add(JsonMatchResponse(modified.Match, true));
                }
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = matchDataAffected
            }));
        }
    }
}