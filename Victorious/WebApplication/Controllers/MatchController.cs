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
                    },
                    defender = new
                    {
                        name = viewModel.matchModel.Defender.Username,
                        id = viewModel.matchModel.Defender.UserID,
                        score = viewModel.matchModel.DefenderScore
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
                PlayerSlot winPlayerSlot = json["winner"] == "Defender" ? PlayerSlot.Defender : PlayerSlot.Challenger;
                Permission userPermission = UserPermission((int)Session["User.UserId"], tournyViewModel.Model);

                if (userPermission == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    tournyViewModel.ProcessTournament();

                    IBracket bracket = tournyViewModel.Tourny.Brackets.ElementAt((int)json["bracketNum"]);
                    bracket.AddWin((int)json["matchNum"], winPlayerSlot);

                    MatchModel matchModel = bracket.GetMatch((int)json["matchNum"]).Model;

                    DbError matchResult = db.UpdateMatch(matchModel);
                    if (matchResult == DbError.SUCCESS)
                    {
                        PlayerSlot forDefender = PlayerSlot.unspecified;
                        PlayerSlot forChallenger = PlayerSlot.unspecified;

                        // Update the next matches.
                        if (matchModel.NextMatchNumber != -1)
                        {
                            MatchModel nextWinnerMatch = bracket.GetMatch((int)matchModel.NextMatchNumber).Model;
                            DbError nextWinnerMatchResult = db.UpdateMatch(nextWinnerMatch);

                            switch (winPlayerSlot)
                            {
                                case PlayerSlot.Challenger:
                                    // IF (NEXTMATCH.ID == CURRENTMATCH.ID) 
                                    if (nextWinnerMatch.ChallengerID == matchModel.ChallengerID)
                                    {
                                        forChallenger = PlayerSlot.Challenger;
                                    }
                                    else if (nextWinnerMatch.DefenderID == matchModel.ChallengerID)
                                    {
                                        forChallenger = PlayerSlot.Defender;
                                    }
                                    else
                                    {
                                        forChallenger = PlayerSlot.unspecified;
                                    }
                                    break;
                                case PlayerSlot.Defender:
                                    if (nextWinnerMatch.ChallengerID == matchModel.DefenderID)
                                    {
                                        forDefender = PlayerSlot.Challenger;
                                    }
                                    else if (nextWinnerMatch.DefenderID == matchModel.DefenderID)
                                    {
                                        forDefender = PlayerSlot.Defender;
                                    }
                                    else
                                    {
                                        forDefender = PlayerSlot.unspecified;
                                    }
                                    break;
                            }
                        }

                        if (matchModel.NextLoserMatchNumber != -1)
                        {
                            MatchModel nextLoserMatch = bracket.GetMatch((int)matchModel.NextLoserMatchNumber).Model;
                            DbError nextLoserMatchResult = db.UpdateMatch(nextLoserMatch);

                            // INVERSE LOGIC -- We want to modify the loser, not the winner, but we're based on the winner.
                            switch (winPlayerSlot)
                            {
                                // In this case, set the defender information.
                                case PlayerSlot.Challenger:
                                    // IF (NEXTMATCH.ID == CURRENTMATCH.ID) 
                                    if (nextLoserMatch.ChallengerID == matchModel.DefenderID)
                                    {
                                        forDefender = PlayerSlot.Challenger;
                                    }
                                    else if (nextLoserMatch.DefenderID == matchModel.DefenderID)
                                    {
                                        forDefender = PlayerSlot.Defender;
                                    }
                                    else
                                    {
                                        forDefender = PlayerSlot.unspecified;
                                    }
                                    break;
                                // In this case, set the challenger info.
                                case PlayerSlot.Defender:
                                    if (nextLoserMatch.ChallengerID == matchModel.ChallengerID)
                                    {
                                        forChallenger = PlayerSlot.Challenger;
                                    }
                                    else if (nextLoserMatch.DefenderID == matchModel.ChallengerID)
                                    {
                                        forChallenger = PlayerSlot.Defender;
                                    }
                                    else
                                    {
                                        forChallenger = PlayerSlot.unspecified;
                                    }
                                    break;
                            }
                        }

                        jsonResult = JsonConvert.SerializeObject(new
                        {
                            status = true,
                            message = "Match was processed sucessfully",
                            data = new
                            {
                                matchNum = matchModel.MatchNumber,
                                defender = new
                                {
                                    nextRound = winPlayerSlot == PlayerSlot.Defender ? matchModel.NextMatchNumber : matchModel.NextLoserMatchNumber,
                                    name = matchModel.Defender.Username,
                                    id = matchModel.Defender.UserID,
                                    score = matchModel.DefenderScore,
                                    slot = forDefender
                                },
                                challenger = new
                                {
                                    nextRound = winPlayerSlot == PlayerSlot.Challenger ? matchModel.NextMatchNumber : matchModel.NextLoserMatchNumber,
                                    name = matchModel.Challenger.Username,
                                    id = matchModel.Challenger.UserID,
                                    score = matchModel.ChallengerScore,
                                    slot = forChallenger
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