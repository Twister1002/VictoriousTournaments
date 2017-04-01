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
            MatchViewModel viewModel = new MatchViewModel(ConvertToInt(tourny), ConvertToInt(bracket), ConvertToInt(match));

            String jsonResult = JsonConvert.SerializeObject(new {
                status = true,
                data = new 
                {
                    challenger = new
                    {
                        name = viewModel.matchModel.Challenger.Username,
                        id = viewModel.matchModel.Challenger.UserID,
                        score = viewModel.matchModel.ChallengerScore
                        //seed = viewModel.bracketModel.UserSeeds.First(x =>
                        //    x.TournamentID == ConvertToInt(tourny) &&
                        //    x.BracketID == viewModel.bracketModel.BracketID &&
                        //    x.UserID == viewModel.matchModel.Challenger.UserID
                        //)
                    },
                    defender = new
                    {
                        name = viewModel.matchModel.Defender.Username,
                        id = viewModel.matchModel.Defender.UserID,
                        score = viewModel.matchModel.DefenderScore
                    //    seed = viewModel.bracketModel.UserSeeds.First(x =>
                    //        x.TournamentID == ConvertToInt(tourny) &&
                    //        x.BracketID == viewModel.bracketModel.BracketID &&
                    //        x.UserID == viewModel.matchModel.Defender.UserID
                    //)
                    },
                    matchId = viewModel.matchModel.MatchID,
                    matchNum = viewModel.matchModel.MatchNumber
                }
                
            });

            return Json(jsonResult);
        }

        [HttpPost]
        [Route("Match/Ajax/Update")]
        public JsonResult MatchUpdate(String jsonData)
        {
            String jsonResult = "";
            Dictionary<string, dynamic> json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonData);

            if (Session["User.UserId"] != null)
            {
                TournamentViewModel tournyViewModel = new TournamentViewModel((int)json["tournyId"]);
                PlayerSlot winner = json["winner"] == "Defender" ? PlayerSlot.Defender : PlayerSlot.Challenger;
                Permission userPermission = UserPermission((int)Session["User.UserId"], tournyViewModel.Model);

                if (userPermission == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    tournyViewModel.ProcessTournament();
                    IBracket bracket = tournyViewModel.Tourny.Brackets.ElementAt((int)json["bracketNum"]);
                    List<MatchModel> matches = tournyViewModel.Model.Brackets.ElementAt((int)json["bracketNum"]).Matches.ToList();

                    bracket.AddWin((int)json["matchNum"], winner);
                    MatchModel matchModel = bracket.GetMatch((int)json["matchNum"]).GetModel((int)json["matchId"]);

                    DbError matchResult = db.UpdateMatch(matchModel);
                    if (matchResult == DbError.SUCCESS)
                    {
                        // Update the next matches.
                        if (matchModel.NextMatchNumber != -1)
                        {
                            int nextWinnerMatchId = matches.First(x => x.MatchNumber == matchModel.NextMatchNumber).MatchID;
                            MatchModel nextWinnerMatch = bracket.GetMatch((int)matchModel.NextMatchNumber).GetModel(nextWinnerMatchId);
                            DbError nextWinnerMatchResult = db.UpdateMatch(nextWinnerMatch);
                        }

                        if (matchModel.NextLoserMatchNumber != -1)
                        {
                            int nextLoserMatchId = matches.First(x => x.MatchNumber == matchModel.NextLoserMatchNumber).MatchID;
                            MatchModel nextLoserMatch = bracket.GetMatch((int)matchModel.NextLoserMatchNumber).GetModel(nextLoserMatchId);
                            DbError nextLoserMatchResult = db.UpdateMatch(nextLoserMatch);
                        }

                        jsonResult = JsonConvert.SerializeObject(new
                        {
                            status = true,
                            message = "Match was processed sucessfully",
                            data = new
                            {
                                loser = new
                                {
                                    round = matchModel.NextLoserMatchNumber
                                },
                                winner = new
                                {
                                    round = matchModel.NextMatchNumber
                                }
                            }
                        });
                    }
                    else
                    {
                        jsonResult = JsonConvert.SerializeObject(new
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
                        });
                    }
                }
                else
                {
                    jsonResult = JsonConvert.SerializeObject(new
                    {
                        status = false,
                        message = "You are not allowed to update this match.",
                        data = new { }
                    });
                }
            }
            else
            {
                jsonResult = JsonConvert.SerializeObject(new
                {
                    status = false,
                    message = "You must be logged in",
                    data = new { }
                });
            }

            return Json(jsonResult);
        }

        public Permission UserPermission(int userId, TournamentModel tournyModel)
        {
            return db.GetUserPermission(getUserModel(userId), tournyModel);
        }
    }
}