using DataLib;
using Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tournament.Structure;
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
        public JsonResult MatchInfo(String jsonData)
        {
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            MatchViewModel viewModel = new MatchViewModel(json["matchId"]);

            Dictionary<int, object> matchData = new Dictionary<int, object>();
            foreach (IGame game in viewModel.Match.Games)
            {
                matchData.Add(game.GameNumber,
                    new {
                        gameNum = game.GameNumber,
                        scores = game.Score,
                    }
                );
            }

            String jsonResult = JsonConvert.SerializeObject(new {
                status = true,
                data = new 
                {
                    matchData = matchData,
                    matchId = viewModel.Match.Id,
                    matchNum = viewModel.Match.MatchNumber,
                    maxGames = viewModel.Match.MaxGames,
                    finished = viewModel.Match.IsFinished
                }
                
            });

            return Json(jsonResult);
        }

        [HttpPost]
        [Route("Match/Ajax/Update")]
        public JsonResult MatchUpdate(String jsonIds, List<GameViewModel> games)
        {
            bool status = false;
            String message = "No action taken";
            object data = new { };

            if (Session["User.UserId"] != null)
            {
                Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonIds);
                TournamentViewModel tournamentModel = new TournamentViewModel(json["tournamentId"]);

                if (tournamentModel.UserPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    tournamentModel.ProcessTournament();
                    IBracket bracket = tournamentModel.Tourny.Brackets.ElementAt(json["bracketNum"]);
                    IMatch match = bracket.GetMatch(json["matchNum"]);
                    BracketViewModel bracketModel = new BracketViewModel(bracket);
                    MatchViewModel matchModel = new MatchViewModel(match);
                    Dictionary<int, bool> processed = new Dictionary<int, bool>();

                    // Verify these matches exists
                    foreach (GameViewModel gameModel in games)
                    {
                        if (match.IsFinished || gameModel.ChallengerScore == gameModel.DefenderScore)
                        {
                            processed.Add(gameModel.GameNumber, false);
                            continue;
                        }

                        if (!match.Games.Contains(gameModel.Game))
                        {
                            // We need to add this game.
                            PlayerSlot winner = gameModel.DefenderScore > gameModel.ChallengerScore ? PlayerSlot.Defender : PlayerSlot.Challenger;
                            GameModel addedGameModel = bracket.AddGame(match.MatchNumber, gameModel.DefenderScore, gameModel.ChallengerScore, winner);

                            // Update the games in the database
                            DbError gameUpdate = db.AddGame(matchModel.Model, addedGameModel);
                            if (gameUpdate != DbError.SUCCESS)
                            {
                                processed.Add(gameModel.GameNumber, false);
                                message = "Failed to update a game.";
                                return Json(JsonConvert.SerializeObject(new
                                {
                                    status = false,
                                    message = message
                                }));
                            }
                            else
                            {
                                processed.Add(gameModel.GameNumber, true);
                            }
                        }
                    }

                    // Load the next models
                    MatchViewModel winnerMatchModel = matchModel.Match.NextMatchNumber != -1 ? new MatchViewModel(bracket.GetMatch(matchModel.Match.NextMatchNumber)) : null;
                    MatchViewModel loserMatchModel = matchModel.Match.NextLoserMatchNumber != -1 ? new MatchViewModel(bracket.GetMatch(matchModel.Match.NextLoserMatchNumber)) : null;

                    // Update the bracket in the database
                    DbError bracketUpdate = db.UpdateBracket(bracketModel.Model);

                    // Update the matches in the database
                    object currentMatchData = null;
                    object winnerMatchData = null;
                    object loserMatchData = null;
                    DbError currentMatchUpdate = db.UpdateMatch(matchModel.Model);
                    DbError winnerMatchUpdate = winnerMatchModel != null ? db.UpdateMatch(winnerMatchModel.Model) : DbError.NONE;
                    DbError loserMatchUpdate = loserMatchModel != null ? db.UpdateMatch(loserMatchModel.Model) : DbError.NONE;

                    if (currentMatchUpdate == DbError.SUCCESS)
                    {
                        status = true;
                        message = "Current match was updated";

                        currentMatchData = new
                        {
                            matchId = matchModel.Match.Id,
                            ready = match.IsReady,
                            finished = match.IsFinished,
                            defender = new
                            {
                                id = matchModel.Defender().Id,
                                name = matchModel.Defender().Name,
                                score = matchModel.Match.Score[(int)PlayerSlot.Defender]
                            },
                            challenger = new
                            {
                                id = matchModel.Challenger().Id,
                                name = matchModel.Challenger().Name,
                                score = matchModel.Match.Score[(int)PlayerSlot.Challenger]
                            }
                        };
                    }
                    if (winnerMatchUpdate == DbError.SUCCESS)
                    {
                        winnerMatchData = new
                        {
                            matchId = winnerMatchModel.Match.Id,
                            ready = winnerMatchModel.Match.IsReady,
                            finished = winnerMatchModel.Match.IsFinished,
                            defender = new
                            {
                                id = winnerMatchModel.Defender().Id,
                                name = winnerMatchModel.Defender().Name,
                                score = winnerMatchModel.Match.Score[(int)PlayerSlot.Defender]
                            },
                            challenger = new
                            {
                                id = winnerMatchModel.Challenger().Id,
                                name = winnerMatchModel.Challenger().Name,
                                score = winnerMatchModel.Match.Score[(int)PlayerSlot.Challenger]
                            }
                        };
                    }
                    if (loserMatchUpdate == DbError.SUCCESS)
                    {
                        loserMatchData = new
                        {
                            matchId = loserMatchModel.Match.Id,
                            ready = loserMatchModel.Match.IsReady,
                            finished = loserMatchModel.Match.IsFinished,
                            defender = new
                            {
                                id = loserMatchModel.Defender().Id,
                                name = loserMatchModel.Defender().Name,
                                score = loserMatchModel.Match.Score[(int)PlayerSlot.Defender]
                            },
                            challenger = new
                            {
                                id = loserMatchModel.Challenger().Id,
                                name = loserMatchModel.Challenger().Name,
                                score = loserMatchModel.Match.Score[(int)PlayerSlot.Challenger]
                            }
                        };
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