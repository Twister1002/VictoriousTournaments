using DatabaseLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tournaments = Tournament.Structure;
using Tournament.Structure;
using WebApplication.Models;
using WebApplication.Models.ViewModels;

namespace WebApplication.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AjaxController : VictoriousController
    {
        bool status;
        String message;
        object data;

        public AjaxController()
        {
            status = false;
            message = "No action taken";
            data = new { };
        }

        public JsonResult Index()
        {
            return Json("Invalid Request");
        }

        public JsonResult BundleJson()
        {
            return Json(new
            {
                status = status,
                message = message,
                data = data
            });
        }

        #region Administrator
        [HttpPost]
        [Route("Ajax/Administrator/Games")]
        public JsonResult Games(String function, GameTypeViewModel game)
        {
            if (account.IsAdministrator())
            {
                GameType gameType = new GameType(work);

                switch (function)
                {
                    case "add":
                        status = gameType.Create(game);
                        break;
                    case "delete":
                        status = gameType.Delete(game);
                        break;
                }

                message = "Was able to " + function + " " + (status ? "successfully" : "unsuccessfully");
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Administrator/Platform")]
        public JsonResult Platform(String function, PlatformViewModel viewModel)
        {
            if (account.IsAdministrator())
            {
                Platform platform = new Platform(work);
                switch (function)
                {
                    case "add":
                        status = platform.Create(viewModel);
                        break;
                    case "delete":
                        status = platform.Delete(viewModel);
                        break;
                }

                message = "Was able to " + function + " " + (status ? "" : "un") + "successfully";
                data = new
                {
                    platforms = platform.Platforms.Select(x => new { x.PlatformID, x.PlatformName }).ToList()
                };
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Administrator/Bracket")]
        public JsonResult Bracket(BracketTypeViewModel bracketModel)
        {
            if (account.IsAdministrator())
            {
                Models.BracketType bracketType = new Models.BracketType(work);

                status = bracketType.Update(bracketModel);
                message = "BracketType was updated " + (status ? "" : "un") + "successfully";
                data = new
                {
                    brackets = bracketType.Brackets.Select(x => new { x.BracketTypeID, x.TypeName, x.IsActive })
                };
            }

            return BundleJson();
        }
        #endregion

        #region Bracket
        [Route("Ajax/Bracket/Reset")]
        [HttpPost]
        public JsonResult Reset(int tournamentId, int bracketId)
        {
            String redirect = Url.Action("Tournament", "Tournament");

            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(work, tournamentId);
                Models.Bracket bracket = tournament.GetBracket(bracketId);

                if (tournament.IsCreator(account.Model.AccountID))
                {
                    bracket.GetBracket().ResetMatches();

                    status = true;
                    message = "Bracket was reset";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this";
                    redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });
                }
            }
            else
            {
                status = false;
                message = "You must login to do this";
                redirect = Url.Action("Login", "Account");
            }

            Session["Message"] = message;
            Session["Message.Class"] = status ? ViewError.SUCCESS : ViewError.WARNING;

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
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(work, tournamentId);
                Models.Bracket bracket = tournament.GetBracket(bracketId);

                if (tournament.IsAdmin(account.Model.AccountID))
                {
                    //List<int> matchesAffected = viewModel.MatchesAffectedList(matchNum);
                    //List<object> matchResponse = new List<object>();

                    //GameViewModel gameModel;
                    //List<GameModel> games = viewModel.Bracket.ResetMatchScore(matchNum);

                    //// Remove the games from the current match.
                    //foreach (GameModel game in games)
                    //{
                    //    // Delete the games
                    //    gameModel = new GameViewModel(game);
                    //    gameModel.Delete();
                    //}

                    List<int> matchNumsAffected = bracket.MatchesAffectedList(matchNum);
                    List<object> matchResponse = new List<object>();

                    bracket.GetBracket().ResetMatchScore(matchNum);

                    foreach (int match in matchNumsAffected)
                    {
                        matchResponse.Add(JsonMatchResponse(bracket.GetMatchByNum(match), true));
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

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Bracket/Standings")]
        public JsonResult Standings(int tournamentId, int bracketId)
        {
            Models.Tournament tournament = new Models.Tournament(work, tournamentId);
            Models.Bracket bracket = tournament.GetBracket(bracketId);
            
            if (bracket == null)
            {
                status = false;
                message = "Invalid data";
            }
            else
            {
                data = new
                {
                    ranks = bracket.GetBracket().Rankings,
                    usePoints = bracket.UsePoints()
                };
            }

            return BundleJson();
        }
        #endregion

        #region Match
        [HttpPost]
        [Route("Ajax/Match")]
        public JsonResult MatchInfo(int tournamentId, int bracketId, int matchId)
        {
            Models.Tournament tournament = new Models.Tournament(work, tournamentId);
            Models.Bracket bracket = tournament.GetBracket(bracketId);
            Models.Match match = bracket.GetMatchById(matchId);

            if (match != null)
            {
                status = true;
                message = "Match was loaded.";
                data = new
                {
                    match = JsonMatchResponse(match, true)
                };
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Match/Update")]
        public JsonResult MatchUpdate(int tournamentId, int bracketId, int matchNum, List<GameViewModel> games)
        {
            if (games != null)
            {
                if (account.IsLoggedIn())
                {
                    Models.Tournament tournament = new Models.Tournament(work, tournamentId);
                    Models.Bracket bracket = tournament.GetBracket(bracketId);
                    Models.Match match = bracket.GetMatchByNum(matchNum);

                    if (tournament.IsAdmin(account.Model.AccountID))
                    {
                        Dictionary<int, bool> processed = new Dictionary<int, bool>();

                        // Verify these matches exists
                        foreach (GameViewModel gameModel in games)
                        {
                            // Tie game check
                            if (match.match.IsFinished || gameModel.ChallengerScore == gameModel.DefenderScore)
                            {
                                processed.Add(gameModel.GameNumber, false);
                                continue;
                            }

                            if (!match.match.Games.Any(x => x.GameNumber == gameModel.GameNumber))
                            {
                                // We need to add this game.
                                PlayerSlot winner = gameModel.DefenderScore > gameModel.ChallengerScore ? PlayerSlot.Defender : PlayerSlot.Challenger;
                                bracket.GetBracket().AddGame(matchNum, gameModel.DefenderScore, gameModel.ChallengerScore, winner);
                            }
                            else
                            {
                                processed.Add(gameModel.GameNumber, false);
                            }
                        }

                        // Update the matches in the database
                        status = bracket.UpdateMatch(bracket.GetBracket().GetMatchModel(matchNum));
                        match = bracket.GetMatchByNum(matchNum);
                        List<object> matchUpdates = new List<object>();

                        if (status)
                        {
                            message = "Current match was updated";

                            matchUpdates.Add(JsonMatchResponse(match, true));
                            if (match.match.NextMatchNumber != -1)
                                matchUpdates.Add(JsonMatchResponse(bracket.GetMatchByNum(match.match.NextMatchNumber), false));
                            if (match.match.NextLoserMatchNumber != -1)
                                matchUpdates.Add(JsonMatchResponse(bracket.GetMatchByNum(match.match.NextLoserMatchNumber), false));
                            if (bracket.GetBracket().BracketType ==  DatabaseLib.BracketType.SWISS)
                            {
                                List<IMatch> roundMatches = bracket.GetBracket().GetRound(match.match.RoundIndex);
                                // We need to verify and check if this round is finished
                                if (!roundMatches.Any(x => x.IsFinished == false))
                                {
                                    foreach (Models.Match iMatch in bracket.GetBracket().GetRound(match.match.RoundIndex + 1))
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
                            refresh = bracket.roundsModified
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
        public JsonResult RemoveGame(int tournamentId, int bracketId, int matchNum, int gameNum)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(work, tournamentId);
                Models.Bracket bracket = tournament.GetBracket(bracketId);
                //Models.Match match = bracket.GetMatchByNum(matchNum);
                List<int> matchesAffected = bracket.MatchesAffectedList(matchNum);
                List<object> matchesAffectedData = new List<object>();

                GameModel gameModel = bracket.GetBracket().RemoveGameNumber(matchNum, gameNum);

                foreach (int matchNumber in matchesAffected)
                {
                    matchesAffectedData.Add(JsonMatchResponse(bracket.GetMatchByNum(matchNum), true));
                }

                data = new
                {
                    matches = matchesAffectedData
                };
            }
            //Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            //BracketViewModel bracketViewModel = new BracketViewModel(json["bracketId"]);
            //List<int> matchesAffected = bracketViewModel.MatchesAffectedList(json["matchNum"]);
            //List<object> matchDataAffected = new List<object>();
            //bracketViewModel.Bracket.RemoveGameNumber(json["matchNum"], json["gameNum"]);

            //status = true;
            //message = "Matches are affected.";

            //foreach (int matchNum in matchesAffected)
            //{
            //    // Load the original and load one from the bracket
            //    MatchViewModel modified = new MatchViewModel(bracketViewModel.Bracket.GetMatchModel(matchNum));
            //    MatchViewModel original = new MatchViewModel(modified.Match.Id);

            //    List<IGame> games = original.Match.Games.Where(x => !modified.Match.Games.Any(y => y.Id == x.Id)).ToList();
            //    foreach (IGame game in games)
            //    {
            //        GameViewModel gameViewModel = new GameViewModel(game);
            //        gameViewModel.Delete();
            //    }

            //    modified.Update();
            //    matchDataAffected.Add(JsonMatchResponse(modified.Match, true));
            //}

            return BundleJson();
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

        #region Helpers
        protected object JsonMatchResponse(Models.Match match, bool includeGames)
        {
            List<object> gameData = new List<object>();

            IPlayer Challenger = match.Challenger;
            IPlayer Defender = match.Defender;

            if (includeGames)
            {
                foreach (IGame game in match.GetGames())
                {
                    gameData.Add(JsonGameResponse(game));
                }
            }

            return new
            {
                matchId = match.match.Id,
                matchNum = match.match.MatchNumber,
                ready = match.match.IsReady,
                finished = match.match.IsFinished,
                challenger = JsonPlayerDataResponse(Challenger, match.ChallengerScore()),
                defender = JsonPlayerDataResponse(Defender, match.DefenderScore()),
                games = gameData
            };
        }

        protected object JsonGameResponse(IGame game)
        {
            return new
            {
                id = game.Id,
                gameNum = game.GameNumber,
                matchId = game.MatchId,
                challenger = new
                {
                    id = game.PlayerIDs[(int)PlayerSlot.Challenger],
                    score = game.Score[(int)PlayerSlot.Challenger]
                },
                defender = new
                {
                    id = game.PlayerIDs[(int)PlayerSlot.Defender],
                    score = game.Score[(int)PlayerSlot.Defender]
                }
            };
        }
        #endregion
    }
}