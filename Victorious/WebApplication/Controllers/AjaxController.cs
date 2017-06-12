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
                GameType gameType = new GameType(service);

                switch (function)
                {
                    case "add":
                        status = gameType.Create(game);
                        break;
                    case "delete":
                        status = gameType.Delete(game);
                        break;
                }
                gameType.Retrieve();
                message = "Was able to " + function + " " + (status ? "successfully" : "unsuccessfully");
                data = new
                {
                    games = gameType.Games.Select(x => new { x.GameTypeID, x.Title }).ToList()
                };
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Administrator/Platform")]
        public JsonResult Platform(String function, PlatformViewModel viewModel)
        {
            if (account.IsAdministrator())
            {
                Platform platform = new Platform(service);
                switch (function)
                {
                    case "add":
                        status = platform.Create(viewModel);
                        break;
                    case "delete":
                        status = platform.Delete(viewModel);
                        break;
                }

                platform.Retrieve();
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
                Models.BracketType bracketType = new Models.BracketType(service);
                
                status = bracketType.Update(bracketModel);
                message = "BracketType was updated " + (status ? "" : "un") + "successfully";

                bracketType.Retrieve();
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
            String redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                Models.Bracket bracket = tournament.GetBracket(bracketId);

                if (tournament.IsCreator(account.Model.AccountID))
                {
                    bracket.GetBracket().ResetMatches();

                    status = true;
                    message = "Bracket was reset";
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this";
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
            data = new { redirect = redirect };

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Bracket/MatchReset")]
        public JsonResult Reset(int tournamentId, int bracketId, int matchNum)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
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
            Models.Tournament tournament = new Models.Tournament(service, tournamentId);
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
            Models.Tournament tournament = new Models.Tournament(service, tournamentId);
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
                    Models.Tournament tournament = new Models.Tournament(service, tournamentId);
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
                                bracket.AddGame(matchNum, gameModel.DefenderScore, gameModel.ChallengerScore, winner);
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

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Match/RemoveGame")]
        public JsonResult RemoveGame(int tournamentId, int bracketId, int matchNum, int gameNum)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                Models.Bracket bracket = tournament.GetBracket(bracketId);
                //Models.Match match = bracket.GetMatchByNum(matchNum);
                List<int> matchesAffected = bracket.MatchesAffectedList(matchNum);
                List<object> matchesAffectedData = new List<object>();

                if (bracket.RemoveGame(matchNum, gameNum))
                {
                    foreach (int matchNumber in matchesAffected)
                    {
                        matchesAffectedData.Add(JsonMatchResponse(bracket.GetMatchByNum(matchNum), true));
                    }

                    status = true;
                    message = "Matches were updated";
                    data = new
                    {
                        matches = matchesAffectedData
                    };
                }
                else
                {
                    message = "There was an error in deleting the game";
                }
            }

            return BundleJson();
        }
        #endregion

        #region Tournament
        [HttpPost]
        [Route("Ajax/Tournament/Search")]
        public JsonResult AjaxSearch(String searchBy)
        {
            List<object> dataReturned = new List<object>();
            Models.Tournament tournament = new Models.Tournament(service, -1);
            tournament.Search(JsonConvert.DeserializeObject<Dictionary<String, String>>(searchBy));

            foreach (TournamentModel tourny in tournament.searched)
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

                data = new
                {
                    search = dataReturned
                };
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/Register")]
        public JsonResult NoAccountRegister(int tournamentId, String name, int bracketId)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                
                // Is an Administrator registering a user?
                if (tournament.IsAdmin(account.Model.AccountID))
                {
                    TournamentUserModel model = tournament.AddUser(name);
                    data = new
                    {
                        user = new
                        {
                            Name = model.Name,
                            Permission = model.PermissionLevel,
                            TournamentUserId = model.TournamentUserID,
                            Seed = tournament.GetUserSeed(bracketId, model.TournamentUserID)
                        },
                        actions = tournament.PermissionAction(account.Model.AccountID, model.TournamentUserID, "default")
                    };
                    if (data != null) status = true;
                    message = "User was " + (status ? "" : "not") + " removed successfully";
                }
                else
                {
                    message = "You are not allowed to register a user.";
                }
            }
            else
            {
                message = "You need to login first.";
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/CheckIn")]
        public JsonResult CheckIn(int tournamentId, int tournamentUserId = -1)
        {
            // Check if a userId was provided first before checking an account
            if (tournamentUserId != -1)
            {
                // An admin is checking in a user
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                status = tournament.CheckUserIn(tournamentUserId);
                message = "User is " + (status ? "" : "not") + " checked in";
            }
            else if (account.IsLoggedIn())
            {
                // A user with an account is checking in.
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                status = tournament.CheckAccountIn(account.Model.AccountID);
                message = "Account is " + (status ? "" : "not") + " checked in";
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/Finalize")]
        public JsonResult Finalize(int tournamentId, int bracketId, Dictionary<String, Dictionary<String, int>> roundData)
        {
            String redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (account.IsLoggedIn())
            {
                // Load the tournament
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                if (tournament.IsAdmin(account.Model.AccountID))
                {
                    if (tournament.FinalizeTournament(bracketId, roundData))
                    {
                        status = true;
                        message = "Your tournament has been finalized. No changes can be made.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewError.SUCCESS;
                    }
                    else
                    {
                        message = "An error occurred while trying to create the matches.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewError.ERROR;
                    }
                }
                else
                {
                    message = "You are not allowed to do that.";

                    Session["Message"] = message;
                    Session["Message.Class"] = ViewError.ERROR;
                }
            }
            else
            {
                message = "You must login to do that.";
                redirect = Url.Action("Login", "Account");

                Session["Message"] = message;
                Session["Message.Class"] = ViewError.ERROR;
            }

            data = new { redirect = redirect };


            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/Delete")]
        public JsonResult Delete(int tournamentId)
        {
            String redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);
                if (tournament.IsCreator(account.Model.AccountID))
                {
                    if (tournament.Delete())
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

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/PermissionChange")]
        public JsonResult PermissionChange(int tournamentId, int targetUser, String action)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);

                Dictionary<String, int> permissionChange = tournament.PermissionAction(account.Model.AccountID, targetUser, action);
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
                        isCheckedIn = tournament.isUserCheckedIn(targetUser)
                    };
                    message = "Permissions are updated";
                    status = true;
                }
            }
            else
            {
                message = "You must be logged in to do this action";
            }

            return BundleJson();
        }

        [HttpPost]
        [Route("Ajax/Tournament/SeedChange")]
        public JsonResult SeedChange(int tournamentId, int bracketId, Dictionary<String, int> players)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tournament = new Models.Tournament(service, tournamentId);

                if (tournament.IsAdmin(account.Model.AccountID))
                {
                    tournament.UpdateSeeds(players, bracketId);
                    status = true;
                    message = "Seeds are updated";
                }
                else
                {
                    message = "An error occured. Please try again later";
                }
            }
            else
            {
                message = "You must login first";
            }

            return BundleJson();
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