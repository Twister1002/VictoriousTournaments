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

            String jsonResult = JsonConvert.SerializeObject(new {
                status = true,
                data = new 
                {
                    challenger = new
                    {
                        name = viewModel.Model.Challenger.Username,
                        id = viewModel.Model.Challenger.UserID,
                        score = viewModel.Model.ChallengerScore
                    },
                    defender = new
                    {
                        name = viewModel.Model.Defender.Username,
                        id = viewModel.Model.Defender.UserID,
                        score = viewModel.Model.DefenderScore
                    },
                    matchId = viewModel.Model.MatchID,
                    matchNum = viewModel.Model.MatchNumber
                }
                
            });

            return Json(jsonResult);
        }

        [HttpPost]
        [Route("Match/Ajax/Update")]
        public JsonResult MatchUpdate(String jsonData)
        {
            dynamic jsonResult = new { };
            Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            if (Session["User.UserId"] != null)
            {
                //MatchViewModel viewModel = new MatchViewModel(ConvertToInt(json["matchId"]));
                TournamentViewModel tournyViewModel = new TournamentViewModel(ConvertToInt(json["tournyId"]));

                Permission sessionPermission = tournyViewModel.UserPermission((int)Session["User.UserId"]);

                if (sessionPermission == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    tournyViewModel.ProcessTournament();

                    PlayerSlot winPlayerSlot = json["winner"] == "Defender" ? PlayerSlot.Defender : PlayerSlot.Challenger;
                    IBracket bracket = tournyViewModel.Tourny.Brackets.ElementAt(ConvertToInt(json["bracketNum"]));
                    GameModel gameModel = bracket.AddGame(ConvertToInt(json["matchNum"]), ConvertToInt(json["defenderScore"]), ConvertToInt(json["challengerScore"]));

                    IMatch currentMatch = bracket.GetMatch(ConvertToInt(json["matchNum"]));
                    IMatch nextWinnerMatch = null;
                    IMatch nextLoserMatch = null;

                    UserModel matchDefender = currentMatch.Players[(int)PlayerSlot.Defender].GetModel();
                    UserModel matchChallenger = currentMatch.Players[(int)PlayerSlot.Challenger].GetModel();

                    dynamic nextWinnerMatchData = null;
                    dynamic nextLoserMatchData = null;

                    DbError gameResult = db.AddGame(currentMatch.GetModel(), gameModel);
                    if (gameResult == DbError.SUCCESS)
                    {
                        DbError matchResult = db.UpdateMatch(currentMatch.GetModel());
                        if (matchResult == DbError.SUCCESS)
                        {
                            // Update the next matches.
                            if (currentMatch.GetModel().NextMatchNumber != -1)
                            {
                                nextWinnerMatch = bracket.GetMatch((int)currentMatch.GetModel().NextMatchNumber);
                                DbError nextMatchResults = db.UpdateMatch(nextWinnerMatch.GetModel());

                                IPlayer challenger = nextWinnerMatch.Players[(int)PlayerSlot.Challenger] != null ? 
                                    nextWinnerMatch.Players[(int)PlayerSlot.Challenger] : 
                                    new User(-1, "Match "+nextWinnerMatch.PreviousMatchNumbers[(int)PlayerSlot.Challenger], "", "", "");
                                IPlayer defender = nextWinnerMatch.Players[(int)PlayerSlot.Defender] != null ? 
                                    nextWinnerMatch.Players[(int)PlayerSlot.Defender] :
                                    new User(-1, "Match " + nextWinnerMatch.PreviousMatchNumbers[(int)PlayerSlot.Defender], "", "", "");

                                nextWinnerMatchData = new
                                {
                                    matchNum = nextWinnerMatch.MatchNumber,
                                    matchId = nextWinnerMatch.Id,
                                    isReady = nextWinnerMatch.IsReady,
                                    isFinished = nextWinnerMatch.IsFinished,
                                    challenger = new
                                    {
                                        id = challenger.Id,
                                        name = challenger.Name,
                                        score = nextWinnerMatch.Score[(int)PlayerSlot.Challenger]
                                    },
                                    defender = new
                                    {
                                        id = defender.Id,
                                        name = defender.Name,
                                        score = nextWinnerMatch.Score[(int)PlayerSlot.Defender]
                                    }
                                };
                            }

                            if (currentMatch.GetModel().NextLoserMatchNumber != -1)
                            {
                                nextLoserMatch = bracket.GetMatch((int)currentMatch.GetModel().NextLoserMatchNumber);
                                DbError nextLoserMatchResult = db.UpdateMatch(nextLoserMatch.GetModel());

                                IPlayer challenger = nextLoserMatch.Players[(int)PlayerSlot.Challenger] != null ?
                                    nextLoserMatch.Players[(int)PlayerSlot.Challenger] :
                                    new User(-1, "Match " + nextLoserMatch.PreviousMatchNumbers[(int)PlayerSlot.Challenger], "", "", "");
                                IPlayer defender = nextLoserMatch.Players[(int)PlayerSlot.Defender] != null ?
                                    nextLoserMatch.Players[(int)PlayerSlot.Defender] :
                                    new User(-1, "Match " + nextLoserMatch.PreviousMatchNumbers[(int)PlayerSlot.Defender], "", "", "");

                                nextLoserMatchData = new
                                {
                                    matchNum = nextLoserMatch.MatchNumber,
                                    matchId = nextLoserMatch.Id,
                                    isReady = nextLoserMatch.IsReady,
                                    isFinished = nextLoserMatch.IsFinished,
                                    challenger = new
                                    {
                                        id = challenger.Id,
                                        name = challenger.Name,
                                        score = nextLoserMatch.Score[(int)PlayerSlot.Challenger]
                                    },
                                    defender = new
                                    {
                                        id = defender.Id,
                                        name = defender.Name,
                                        score = nextLoserMatch.Score[(int)PlayerSlot.Defender]
                                    }
                                };
                            }

                            jsonResult = new
                            {
                                status = true,
                                message = "Match was processed sucessfully",
                                data = new
                                {
                                    currentMatch = new
                                    {
                                        matchNum = currentMatch.MatchNumber,
                                        matchId = currentMatch.Id,
                                        isReady = currentMatch.IsReady,
                                        isFinished = currentMatch.IsFinished,
                                        hasGames = currentMatch.Games.Count > 0,
                                        challenger = new
                                        {
                                            id = currentMatch.Players[(int)PlayerSlot.Challenger].Id,
                                            name = currentMatch.Players[(int)PlayerSlot.Challenger].Name,
                                            score = currentMatch.Score[(int)PlayerSlot.Challenger]
                                        },
                                        defender = new
                                        {
                                            id = currentMatch.Players[(int)PlayerSlot.Defender].Id,
                                            name = currentMatch.Players[(int)PlayerSlot.Defender].Name,
                                            score = currentMatch.Score[(int)PlayerSlot.Defender]
                                        },
                                        
                                    },
                                    nextWinnerMatch = nextWinnerMatchData,
                                    nextLoserMatch = nextLoserMatchData,
                                }
                            };
                        }
                        else
                        {
                            jsonResult = new
                            {
                                status = false,
                                message = "There was an unexpected error.",
                                data = new
                                {
                                    exception = new
                                    {
                                        message = db.interfaceException.Message,
                                        innerMessage = db.interfaceException.InnerException
                                    }
                                }
                            };
                        }
                    }
                    else
                    {
                        jsonResult = new
                        {
                            status = false,
                            message = "There was an error in creating the game.",
                            data = new
                            {
                                exception = new
                                {
                                    message = db.interfaceException.Message,
                                    innerMessage = db.interfaceException.InnerException
                                }
                            }
                        };
                    }
                }
                else
                {
                    jsonResult = new
                    {
                        status = false,
                        message = "You are not allowed to update this match.",
                        data = new { }
                    };
                }
            }
            else
            {
                jsonResult = new
                {
                    status = false,
                    message = "You must be logged in",
                    data = new { }
                };
            }

            return Json(JsonConvert.SerializeObject(jsonResult));
        }
    }
}