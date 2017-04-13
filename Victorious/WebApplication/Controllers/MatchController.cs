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
        public JsonResult MatchInfo(String match, String bracket, String tourny)
        {
            MatchViewModel viewModel = new MatchViewModel(ConvertToInt(match));

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
                    bracket.AddGame(ConvertToInt(json["matchNum"]), ConvertToInt(json["defenderScore"]), ConvertToInt(json["challengerScore"]));

                    IMatch currentMatch = bracket.GetMatch(ConvertToInt(json["matchNum"]));
                    IMatch nextWinnerMatch = null;
                    IMatch nextLoserMatch = null;
                    
                    UserModel matchDefender = currentMatch.Players[(int)PlayerSlot.Defender].GetModel();
                    UserModel matchChallenger = currentMatch.Players[(int)PlayerSlot.Challenger].GetModel();

                    dynamic nextWinnerMatchData = new { };
                    dynamic nextLoserMatchData = new { };

                    DbError matchResult = db.UpdateMatch(currentMatch.GetModel());
                    if (matchResult == DbError.SUCCESS)
                    {
                        // Update the next matches.
                        if (currentMatch.GetModel().NextMatchNumber != -1)
                        {
                            nextWinnerMatch = bracket.GetMatch((int)currentMatch.GetModel().NextMatchNumber);
                            DbError nextMatchResults = db.UpdateMatch(nextWinnerMatch.GetModel());

                            nextWinnerMatchData = new
                            {
                                matchNum = nextWinnerMatch.MatchNumber,
                                matchId = nextWinnerMatch.Id,
                                challenger = new
                                {
                                    id = nextWinnerMatch.Players[(int)PlayerSlot.Challenger].Id,
                                    name = nextWinnerMatch.Players[(int)PlayerSlot.Challenger].Name,
                                    score = nextWinnerMatch.Score[(int)PlayerSlot.Challenger]
                                },
                                defender = new
                                {
                                    id = nextWinnerMatch.Players[(int)PlayerSlot.Defender].Id,
                                    name = nextWinnerMatch.Players[(int)PlayerSlot.Defender].Name,
                                    score = nextWinnerMatch.Score[(int)PlayerSlot.Defender]
                                }
                            };
                        }

                        if (currentMatch.GetModel().NextLoserMatchNumber != -1)
                        {
                            nextLoserMatch = bracket.GetMatch((int)currentMatch.GetModel().NextLoserMatchNumber);
                            DbError nextLoserMatchResult = db.UpdateMatch(nextLoserMatch.GetModel());
                            nextLoserMatchData = new
                            {
                                matchNum = nextLoserMatch.MatchNumber,
                                matchId = nextLoserMatch.Id,
                                challenger = new
                                {
                                    id = nextLoserMatch.Players[(int)PlayerSlot.Challenger].Id,
                                    name = nextLoserMatch.Players[(int)PlayerSlot.Challenger].Name,
                                    score = nextLoserMatch.Score[(int)PlayerSlot.Challenger]
                                },
                                defender = new
                                {
                                    id = nextLoserMatch.Players[(int)PlayerSlot.Defender].Id,
                                    name = nextLoserMatch.Players[(int)PlayerSlot.Defender].Name,
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
                                    hasGames = true,
                                    isFinished = currentMatch.IsFinished
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
                            data = new {
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