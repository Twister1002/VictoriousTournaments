using DatabaseLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tournament.Structure;
using WebApplication.Models;
using WebApplication.Models.Administrator;

namespace WebApplication.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AjaxController : VictoriousController
    {
        public JsonResult Index()
        {
            return Json("Invalid Request");
        }

        #region Administrator
        [HttpPost]
        [Route("Ajax/Administrator/Games")]
        public JsonResult Games(String jsonData)
        {
            object jsonReturn = new
            {
                status = false,
                message = "No action was taken"
            };

            if (account.IsAdministrator())
            {
                Dictionary<String, String> json = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonData);
                GameTypeViewModel gameType = new GameTypeViewModel();
                bool result = false;

                switch (json["function"])
                {
                    case "add":
                        gameType.Title = json["title"];
                        result = gameType.Create();
                        break;
                    case "delete":
                        result = gameType.Delete(int.Parse(json["gameid"]));
                        break;
                }

                jsonReturn = new
                {
                    status = result,
                    message = "Was able to " + json["function"] + " " + (result ? "successfully" : "unsuccessfully"),
                    data = gameType.GameTypes.Select(x => new { x.GameTypeID, x.Title }).ToList()
                };
            }

            return Json(JsonConvert.SerializeObject(jsonReturn));
        }

        [HttpPost]
        [Route("Ajax/Administrator/Platform")]
        public JsonResult Platform(String jsonData)
        {
            bool status = false;
            String message = "No action taken";

            Dictionary<String, String> json = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonData);
            PlatformTypeViewModel viewModel = new PlatformTypeViewModel();

            switch (json["action"])
            {
                case "add":
                    viewModel.Platform = json["Platform"];
                    status = viewModel.Create();
                    break;
                case "delete":
                    status = viewModel.Delete(int.Parse(json["PlatformId"]));
                    break;
            }

            message = "Was able to " + json["action"] + " " + (status ? "" : "un") + "successfully";


            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                platforms = viewModel.platforms.Select(x => new { x.PlatformID, x.PlatformName }).ToList()
            }));
        }

        [HttpPost]
        [Route("Ajax/Administrator/Bracket")]
        public JsonResult Bracket(int bracketTypeId)
        {
            bool status = false;
            String message = "No action taken";

            BracketTypeViewModel viewModel = new BracketTypeViewModel();
            status = viewModel.Update(bracketTypeId);
            message = "BracketType was updated " + (status ? "" : "un") + "successfully";


            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                brackets = viewModel.Brackets.Select(x => new { x.BracketTypeID, x.TypeName, x.IsActive })
            }));
        }
        #endregion

        #region Bracket
        [Route("Ajax/Bracket/Reset")]
        [HttpPost]
        public JsonResult Reset(int tournamentId, int bracketId)
        {
            bool status = false;
            String message = "No action taken";
            String redirect = Url.Action("Tournament", "Tournament");

            if (IsLoggedIn())
            {
                TournamentViewModel tournamentModel = new TournamentViewModel(tournamentId);
                //BracketViewModel viewModel = new BracketViewModel(bracketId);
                if (tournamentModel.IsCreator(account.AccountId))
                {
                    tournamentModel.Tourny.Brackets.Where(x => x.Id == bracketId).Single().ResetMatches();

                    status = true;
                    message = "Bracket was reset";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentModel.Model.TournamentID });
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentModel.Model.TournamentID });
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
        public JsonResult Reset(int tournamentId, int bracketId, int matchNum)
        {
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
        public JsonResult Standings(int tournamentId, int bracketNum)
        {
            bool status = true;
            bool usePoints = false;
            String message = "Standings are acquired.";
            object data = new { };

            TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
            IBracket bracket = viewModel.Tourny.Brackets.ElementAtOrDefault(bracketNum);

            if (bracket == null)
            {
                status = false;
                message = "Invalid data";

            }
            else
            {
                usePoints = (bracket.BracketType == BracketType.DOUBLE || bracket.BracketType == BracketType.SINGLE ? false : true);
                data = new { ranks = bracket.Rankings, usePoints = usePoints };
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
        #endregion

        #region Match
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
                if (account != null)
                {
                    Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonIds);
                    TournamentViewModel tournamentModel = new TournamentViewModel(json["tournamentId"]);

                    if (tournamentModel.IsAdministrator(account.AccountId))
                    {
                        BracketViewModel bracketModel = new BracketViewModel(tournamentModel.Tourny.Brackets.Where(x => x.Id == json["bracketId"]).Single());
                        Dictionary<int, bool> processed = new Dictionary<int, bool>();

                        // Verify these matches exists
                        foreach (GameViewModel gameModel in games)
                        {
                            // Tie game check
                            if (bracketModel.Bracket.GetMatch(json["matchNum"]).IsFinished || gameModel.ChallengerScore == gameModel.DefenderScore)
                            {
                                processed.Add(gameModel.GameNumber, false);
                                continue;
                            }

                            if (!bracketModel.Bracket.GetMatch(json["matchNum"]).Games.Any(x => x.GameNumber == gameModel.GameNumber))
                            {
                                // We need to add this game.
                                PlayerSlot winner = gameModel.DefenderScore > gameModel.ChallengerScore ? PlayerSlot.Defender : PlayerSlot.Challenger;
                                bracketModel.Bracket.AddGame(json["matchNum"], gameModel.DefenderScore, gameModel.ChallengerScore, winner);
                            }
                            else
                            {
                                processed.Add(gameModel.GameNumber, false);
                            }
                        }

                        // Update the matches in the database
                        MatchViewModel matchModel = new MatchViewModel(tournamentModel.Model.Brackets.Single(x => x.BracketID == json["bracketId"]).Matches.Single(x => x.MatchNumber == json["matchNum"]));
                        matchModel.ApplyChanges(bracketModel.Bracket.GetMatchModel(json["matchNum"]));
                        bool currentMatchUpdate = matchModel.Update();
                        List<object> matchUpdates = new List<object>();

                        if (currentMatchUpdate)
                        {
                            status = true;
                            message = "Current match was updated";

                            matchUpdates.Add(JsonMatchResponse(matchModel.Match, true));
                            if (matchModel.Match.NextMatchNumber != -1)
                                matchUpdates.Add(JsonMatchResponse(bracketModel.Bracket.GetMatch(matchModel.Match.NextMatchNumber), false));
                            if (matchModel.Match.NextLoserMatchNumber != -1)
                                matchUpdates.Add(JsonMatchResponse(bracketModel.Bracket.GetMatch(matchModel.Match.NextLoserMatchNumber), false));
                            if (bracketModel.Bracket.BracketType == BracketType.SWISS)
                            {
                                List<IMatch> roundMatches = bracketModel.Bracket.GetRound(matchModel.Match.RoundIndex);
                                // We need to verify and check if this round is finished
                                if (!roundMatches.Any(x => x.IsFinished == false))
                                {
                                    foreach (IMatch match in bracketModel.Bracket.GetRound(matchModel.Match.RoundIndex + 1))
                                    {
                                        matchUpdates.Add(JsonMatchResponse(match, false));
                                    }
                                }
                            }
                        }

                        // Prepare data
                        data = new
                        {
                            processed = processed,
                            matchUpdates = matchUpdates,
                            refresh = bracketModel.roundsModified
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
            bracketViewModel.Bracket.RemoveGameNumber(json["matchNum"], json["gameNum"]);

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

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = matchDataAffected
            }));
        }
        #endregion

        #region Tournament
        [HttpPost]
        [Route("Ajax/Tournament/Search")]
        public JsonResult AjaxSearch(String searchData)
        {
            Dictionary<String, String> searchBy = JsonConvert.DeserializeObject<Dictionary<String, String>>(searchData);
            TournamentViewModel viewModel = new TournamentViewModel();
            List<object> dataReturned = new List<object>();

            viewModel.Search(searchBy);
            foreach (TournamentModel tourny in viewModel.SearchedTournaments)
            {
                dataReturned.Add(new
                {
                    id = tourny.TournamentID,
                    title = tourny.Title,
                    game = tourny.GameType.Title,
                    platform = tourny.Platform != null ? tourny.Platform.PlatformName : "None",
                    startDate = tourny.TournamentStartDate.ToShortDateString(),
                    publicRegistration = tourny.PublicRegistration,
                    publicViewing = tourny.PublicViewing,
                    link = Url.Action("Tournament", "Tournament", new { guid = tourny.TournamentID })
                });
            }

            return Json(JsonConvert.SerializeObject(dataReturned));
        }

        [HttpPost]
        [Route("Ajax/Tournament/Register")]
        public JsonResult NoAccountRegister(int tournamentId, String name, int bracketId)
        {
            TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
            bool status = false;
            object data = new { };
            String message = "No action taken";

            // Is an Administrator registering a user?
            if (viewModel.IsAdministrator(account.AccountId))
            {
                TournamentUserModel model = viewModel.AddUser(name);
                data = new
                {
                    user = new
                    {
                        Name = model.Name,
                        Permission = model.PermissionLevel,
                        TournamentUserId = model.TournamentUserID,
                        Seed = viewModel.UserSeed(model.TournamentUserID, bracketId)
                    },
                    actions = viewModel.PermissionAction(account.AccountId, model.TournamentUserID, "default")
                };
                if (data != null) status = true;
                message = "User was " + (status ? "" : "not") + " removed successfully";
            }
            else
            {
                message = "You are not allowed to register a user.";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = data
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/CheckIn")]
        public JsonResult CheckIn(int tournamentId, int tournamentUserId = -1)
        {
            bool status = false;
            bool isCheckedIn = false;
            String message = "No action taken";


            if (tournamentUserId != -1)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.UserCheckIn(tournamentUserId))
                {
                    status = true;
                    message = "User is checkedin";
                }
                else
                {
                    message = "User was not checked in";
                }

                isCheckedIn = viewModel.isUserCheckedIn(tournamentUserId);
            }
            else if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.AccountCheckIn(account.AccountId))
                {
                    status = true;
                    message = "User is checkedin";
                }
                else
                {
                    message = "User was not checked in";
                }
            }
            else
            {
                message = "Account not logged in";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                isCheckedIn = isCheckedIn
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/Finalize")]
        public JsonResult Finalize(int tournamentId, int bracketId, Dictionary<String, Dictionary<String, int>> roundData)
        {
            bool status = false;
            String message = "No action was taken";
            String redirect = redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (account != null)
            {
                // Load the tournament
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.IsAdministrator(account.AccountId))
                {
                    if (viewModel.FinalizeTournament(bracketId, roundData))
                    {
                        status = true;
                        message = "Your tournament has been finalized. No changes can be made.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    else
                    {
                        message = "An error occurred while trying to create the matches.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
                    }
                }
                else
                {
                    message = "You are not permitted to do that.";

                    Session["Message"] = message;
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                message = "You must login to do that.";
                redirect = Url.Action("Login", "Account");

                Session["Message"] = message;
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
            }

            return Json(JsonConvert.SerializeObject(
                new
                {
                    status = status,
                    message = message,
                    redirect = redirect
                }
            ));
        }

        [HttpPost]
        [Route("Ajax/Tournament/Delete")]
        public JsonResult Delete(int tournamentId)
        {
            bool status = false;
            String message = "No action taken";
            String redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (IsLoggedIn())
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.IsCreator(account.AccountId))
                {
                    if (viewModel.Delete())
                    {
                        status = true;
                        message = "Tournament was deleted.";
                        redirect = Url.Action("Index", "Tournament");
                    }
                    else
                    {
                        status = false;
                        message = "Unable to delete the tournament due to an error.";
                    }
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this.";
                }
            }
            else
            {
                status = false;
                message = "Please login in order to modify a tournament.";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status,
                message,
                redirect
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/PermissionChange")]
        public JsonResult PermissionChange(int TournamentId, int targetUser, String action)
        {
            bool status = false;
            String message = "No action taken";
            object data = new { };

            if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(TournamentId);

                Dictionary<String, int> permissionChange = viewModel.PermissionAction(account.AccountId, targetUser, action);
                if (permissionChange == null)
                {
                    status = false;
                    message = "An unexpected error occured";
                }
                else
                {
                    data = new
                    {
                        permissions = permissionChange,
                        isCheckedIn = viewModel.isUserCheckedIn(targetUser)
                    };
                    message = "Permissions are updated";
                    status = true;
                }
            }
            else
            {
                message = "You must be logged in to do this action";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = data
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/SeedChange")]
        public JsonResult SeedChange(int tournamentId, int bracketId, Dictionary<String, int> players)
        {
            bool status = false;
            String message = "No action taken";

            if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);

                if (viewModel.IsAdministrator(account.AccountId))
                {
                    viewModel.UpdateSeeds(players, bracketId);
                    status = true;
                    message = "Seeds are updated";
                }
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message
            }));
        }
        #endregion
    }
}