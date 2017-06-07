﻿using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Tournaments = Tournament.Structure;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class Tournament : Model
    {
        private bool TempFixMatchObjects = true;
        private Tournaments.ITournament Tourny;
        private TournamentViewModel viewModel;
        private TournamentModel Model;
        private List<TournamentModel> searched;

        public Tournament(IUnitOfWork work, int id) : base(work)
        {
            Retrieve(id);
            Init();
        }

        private void Init()
        {
            // Create the tournament
            Tourny = new Tournaments.Tournament(Model);
            searched = new List<TournamentModel>();

            // Set the field's original data
            viewModel.BracketTypes = services.Type.GetAllBracketTypes().Where(x => x.IsActive == true).ToList();
            viewModel.GameTypes = services.Type.GetAllGameTypes();
            viewModel.PlatformTypes = services.Type.GetAllPlatforms();
            viewModel.PublicViewing = true;
            viewModel.BracketData = new List<BracketInfo>();
            viewModel.NumberOfRounds = Enumerable.Range(0, 100).ToList();
        }

        public void Search(Dictionary<String, String> searchData)
        {
            if (searchData == null)
            {
                searchData = new Dictionary<String, String>();
            }

            searchData.Add("PublicViewing", "true");
            if (searchData.Keys.Contains("startDate"))
            {
                searchData.Add("TournamentStartDate", "true");
                searchData.Add("RegistrationStartDate", "true");

                if (searchData.Keys.Contains("endDate"))
                {
                    searchData.Add("TournamentEndDate", DateTime.Now.ToShortDateString());
                    searchData.Add("RegistrationEndDate", DateTime.Now.ToShortDateString());
                }
                else
                {
                    searchData.Add("TournamentEndDate", DateTime.Now.ToShortDateString());
                    searchData.Add("RegistrationEndDate", DateTime.Now.ToShortDateString());
                }
            }

            List<String> safeParamList = new List<String>() {
                "Title",
                "GameTypeID",
                "PlatformTypeID",
                "PublicViewing",
                "PublicRegistration",
                "TournamentStartDate",
                "TournamentEndDate",
                "RegistrationStartDate",
                "RegistrationEndDate"
            };

            searchData = searchData.Where(k => safeParamList.Contains(k.Key) && k.Value != String.Empty).ToDictionary(k => k.Key, k => k.Value);
            searched = services.Tournament.FindTournaments(searchData);
        }

        private void ProcessTournament()
        {
            if (Model == null)
            {
                return;
            }

            if (TempFixMatchObjects) MatchObjectFix();

            Tourny = new Tournaments.Tournament();
            Tourny.Title = Model.Title;
            foreach (BracketModel bracket in Model.Brackets)
            {
                Tourny.AddBracket(Tourny.RestoreBracket(bracket));
            }
        }

        #region Bracket
        public Bracket GetBracket(int bracketId)
        {
            return new Bracket(services, Tourny.Brackets.Single(x => x.Id == bracketId));
        }
        #endregion

        #region Match
        public Match GetMatch(int bracketId, int matchId)
        {
            int matchNum = Model.Brackets.Single(x => x.BracketID == bracketId).Matches.Single(x => x.MatchID == matchId).MatchNumber;
            return new Match(services, Tourny.Brackets.Single(x => x.Id == bracketId).GetMatch(matchNum));
        }
        #endregion

        #region Finalize
        public bool FinalizeTournament(int bracketId, Dictionary<String, Dictionary<String, int>> roundData)
        {
            // Set variables
            BracketModel bracket = Model.Brackets.Single(x => x.BracketID == bracketId);
            Tournaments.IBracket tourny = Tourny.Brackets.Single(x => x.Id == bracketId);

            // Set max games for every round
            foreach (KeyValuePair<String, Dictionary<String, int>> roundInfo in roundData)
            {
                foreach (KeyValuePair<String, int> data in roundInfo.Value)
                {
                    switch (roundInfo.Key)
                    {
                        case "upper":
                            tourny.SetMaxGamesForWholeRound(int.Parse(data.Key), data.Value);
                            break;
                        case "lower":
                            tourny.SetMaxGamesForWholeLowerRound(int.Parse(data.Key), data.Value);
                            break;
                        case "final":
                            tourny.SetMaxGamesForWholeRound(int.Parse(data.Key), data.Value);
                            break;
                    }
                }
            }

            // Update the necesarry information
            bracket.Finalized = true;
            Model.InProgress = true;

            // Update the database
            CreateMatches(bracket, tourny);
            services.Tournament.UpdateBracket(bracket);
            services.Tournament.UpdateTournament(Model);
            return services.Save();
        }

        private void CreateMatches(BracketModel bracketModel, Tournaments.IBracket bracket)
        {
            // Verify if the tournament has not need finalized.
            if (!bracketModel.Finalized)
            {
                // Add the matches to the database
                for (int i = 1; i <= bracket.NumberOfMatches; i++)
                {
                    MatchModel matchModel = bracket.GetMatchModel(i);
                    services.Tournament.AddMatch(matchModel);
                }
            }
        }
        #endregion

        #region CRUD
        //TODO: Fix issue where tournamentCodes can collide and be repeatable.
        public bool Create(int accountId)
        {
            ApplyChanges();
            Model.CreatedOn = DateTime.Now;
            Model.CreatedByID = accountId;

            // Generate the Tournament Invite Codes
            Model.InviteCode = Codes.GenerateInviteCode();

            // Create InviteModel
            TournamentInviteModel inviteModel = new TournamentInviteModel()
            {
                TournamentInviteCode = Model.InviteCode,
                DateCreated = DateTime.Now,
                TournamentID = Model.TournamentID,
                NumberOfUses = 0
            };

            UpdateBrackets();
            services.Tournament.AddTournament(Model);
            services.Tournament.AddTournamentInvite(inviteModel);

            return services.Save();
        }

        public void Retrieve(int id)
        {
            Model = services.Tournament.GetTournament(id);
        }

        public bool Update(int accountId)
        {
            ApplyChanges();
            Model.LastEditedByID = accountId;
            Model.LastEditedOn = DateTime.Now;

            services.Tournament.UpdateTournament(Model);
            UpdateBrackets();

            if (services.Save())
            {
                UpdatePlayers();
                return services.Save();
            }
            else
            {
                return false;
            }
        }

        public bool Delete()
        {
            List<BracketModel> brackets = Model.Brackets.ToList();
            List<TournamentUserModel> tournamentUsers = Model.TournamentUsers.ToList();

            // Delete all the brackets
            foreach (BracketModel bracket in brackets)
            {
                List<TournamentUsersBracketModel> users = bracket.TournamentUsersBrackets.ToList();

                // Delete all the seeded users
                foreach (TournamentUsersBracketModel user in users)
                {
                    services.Tournament.DeleteTournamentUsersBracket(user.TournamentUserID, bracket.BracketID);
                }

                services.Tournament.DeleteBracket(bracket.BracketID);
            }

            // Delete all the users
            foreach (TournamentUserModel user in tournamentUsers)
            {
                services.Tournament.DeleteTournamentUser(user.TournamentUserID);
            }

            // Delete the Invite code
            services.Tournament.DeleteTournamentInvite(Model.InviteCode);

            // Delete the tournament
            services.Tournament.DeleteTournament(Model.TournamentID);

            return services.Save();
        }
        
        public void UpdateBrackets()
        {
            int updates = Math.Max(viewModel.BracketData.Count, Model.Brackets.Count);
            List<BracketModel> updatedBrackets = new List<BracketModel>();

            for (int i = 0; i < updates; i++)
            {
                BracketInfo? newBracket = viewModel.BracketData.ElementAtOrDefault(i);
                BracketModel bracketModel = Model.Brackets.ElementAtOrDefault(i);
                //List<TournamentUsersBracketModel> users = new List<TournamentUsersBracketModel>();

                if (newBracket != null)
                {
                    if (bracketModel != null)
                    {
                        // We just need to update the data
                        bracketModel.BracketTypeID = newBracket.Value.BracketType;
                        bracketModel.MaxRounds = newBracket.Value.NumberOfRounds;

                        updatedBrackets.Add(bracketModel);
                        //tournamentService.UpdateBracket(bracketModel);
                    }
                    else if (bracketModel == null)
                    {
                        // We need to add this bracket
                        bracketModel = new BracketModel()
                        {
                            MaxRounds = newBracket.Value.NumberOfRounds,
                            BracketTypeID = newBracket.Value.BracketType,
                            Finalized = false,
                            TournamentID = Model.TournamentID
                        };

                        updatedBrackets.Add(bracketModel);
                        //tournamentService.AddBracket(bracketModel);
                    }
                }
            }

            Model.Brackets = updatedBrackets;
        }

        private void UpdatePlayers()
        {
            foreach (BracketModel bracketModel in Model.Brackets)
            {
                // Update the Players
                if (bracketModel.TournamentUsersBrackets.Count != Model.TournamentUsers.Where(x => IsUserParticipant(x.TournamentUserID)).Count())
                {
                    for (int j = 0; j < Model.TournamentUsers.Count; j++)
                    {
                        TournamentUserModel user = Model.TournamentUsers.ElementAt(j);
                        TournamentUsersBracketModel model = new TournamentUsersBracketModel()
                        {
                            TournamentUserID = user.TournamentUserID,
                            BracketID = bracketModel.BracketID,
                            TournamentID = Model.TournamentID,
                            Seed = j + 1
                        };
                        //bracketModel.TournamentUsersBrackets.Add(model);

                        if (!IsUserAdministrator(user.TournamentUserID))
                        {
                            services.Tournament.AddTournamentUsersBracket(model);
                        }
                    }
                }
            }
        }

        public void UpdateSeeds(Dictionary<String, int> players, int bracketId)
        {
            foreach (KeyValuePair<String, int> player in players)
            {
                TournamentUsersBracketModel user = Model.Brackets.Where(x => x.BracketID == bracketId).Single()
                    .TournamentUsersBrackets.Where(z => z.TournamentUserID == int.Parse(player.Key)).SingleOrDefault();
                if (user != null)
                {
                    user.Seed = player.Value;
                    services.Tournament.UpdateTournamentUsersBracket(user);
                }
                else
                {
                    user = new TournamentUsersBracketModel()
                    {
                        Seed = player.Value,
                        TournamentUserID = int.Parse(player.Key),
                        TournamentID = Model.TournamentID,
                        BracketID = bracketId
                    };

                    services.Tournament.AddTournamentUsersBracket(user);
                }
            }

            services.Save();
        }
        #endregion

        #region AddUsers
        public TournamentUserModel AddUser(String name)
        {
            bool isEmail = false;
            TournamentUserModel userModel = null;

            if (!isEmail)
            {
                userModel = new TournamentUserModel()
                {
                    Name = name,
                    PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
                    TournamentID = Model.TournamentID
                };
            }

            return AddUserToTournament(userModel) ? userModel : null;
        }

        public bool AddUser(AccountViewModel account, Permission permission)
        {
            // Verify this user doesn't exist in the tournament
            if (!Model.TournamentUsers.Any(x => x.AccountID == account.AccountId))
            {
                TournamentUserModel tournamentUserModel = new TournamentUserModel()
                {
                    AccountID = account.AccountId,
                    Name = account.Username,
                    PermissionLevel = (int)permission,
                    TournamentID = Model.TournamentID
                };

                return AddUserToTournament(tournamentUserModel);
            }
            else
            {
                return false;
            }
        }

        private bool AddUserToTournament(TournamentUserModel model)
        {
            // Add the user to the tournament
            services.Tournament.AddTournamentUser(model);

            if (model.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD)
            {
                // Add user to every bracket
                foreach (BracketModel bracket in Model.Brackets)
                {
                    int? seedData = bracket.TournamentUsersBrackets.Max(x => x.Seed);
                    int seed = seedData != null ? seedData.Value + 1 : 1;

                    TournamentUsersBracketModel bracketUser = new TournamentUsersBracketModel()
                    {
                        TournamentID = Model.TournamentID,
                        TournamentUserID = model.TournamentUserID,
                        Seed = seed,
                        BracketID = bracket.BracketID
                    };

                    services.Tournament.AddTournamentUsersBracket(bracketUser);
                }
            }

            return services.Save();
        }
        #endregion

        #region RemoveUsers
        public bool RemoveUser(int accountId)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.AccountID == accountId);

            services.Tournament.DeleteTournamentUser(user.TournamentUserID);
            return services.Save();
        }

        public bool RemoveUser(String username)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.Name == username);

            services.Tournament.DeleteTournamentUser(user.TournamentUserID);
            return services.Save();
        }
        #endregion

        #region Permissions
        #region Account
        public Permission GetAccountPermission(int accountId)
        {
            TournamentUserModel user = Model.TournamentUsers.Single(x => x.AccountID == accountId);
            if (user != null)
            {
                return (Permission)user.PermissionLevel;
            }

            return Permission.NONE;
        }

        public bool IsParticipent(int accountId)
        {
            Permission permission = GetAccountPermission(accountId);
            if (permission == Permission.TOURNAMENT_STANDARD ||
                permission == Permission.TOURNAMENT_ADMINISTRATOR ||
                permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAdmin(int accountId)
        {
            Permission permission = GetAccountPermission(accountId);
            if (permission == Permission.TOURNAMENT_ADMINISTRATOR ||
                permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCreator(int accountId)
        {
            return GetAccountPermission(accountId) == Permission.TOURNAMENT_CREATOR;
        }
        #endregion

            #region User
        public Permission GetUserPermission(int tournamentUserId)
        {
            TournamentUserModel user = Model.TournamentUsers.Single(x => x.TournamentUserID == tournamentUserId);
            if (user != null)
            {
                return (Permission)user.PermissionLevel;
            }

            return Permission.NONE;
        }

        public bool IsUserParticipant(int tournamentUserId)
        {
            Permission permission = GetUserPermission(tournamentUserId);
            if (permission == Permission.TOURNAMENT_STANDARD) //||
            //    permission == Permission.TOURNAMENT_ADMINISTRATOR ||
            //    permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUserAdministrator(int tournamentUserId)
        {
            Permission permission = GetUserPermission(tournamentUserId);
            if (permission == Permission.TOURNAMENT_ADMINISTRATOR ||
                permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUserCreator(int tournamentUserId)
        {
            Permission permission = GetUserPermission(tournamentUserId);
            if (permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public Dictionary<String, int> PermissionAction(int accountId, int tournamentUserId, String action)
        {
            TournamentUserModel targetAccount = Model.TournamentUsers.First(x => x.TournamentUserID == tournamentUserId);
            bool accountIsAdmin = IsAdmin(accountId);
            bool accountIsCreator = IsCreator(accountId);

            Dictionary<String, int> permissionActions = new Dictionary<string, int>();
            permissionActions.Add("Demote", 0);
            permissionActions.Add("Promote", 0);
            permissionActions.Add("Remove", 0);
            permissionActions.Add("Permission", -1);

            switch (action)
            {
                case "promote":
                    switch ((Permission)targetAccount.PermissionLevel)
                    {
                        case Permission.TOURNAMENT_STANDARD:
                            if (accountIsCreator)
                            {
                                targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_ADMINISTRATOR;
                                services.Tournament.UpdateTournamentUser(targetAccount);
                                permissionActions["Demote"] = 1;
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            break;
                        case Permission.TOURNAMENT_CREATOR:
                            break;
                    }
                    break;
                case "remove":
                case "demote":
                    switch ((Permission)targetAccount.PermissionLevel)
                    {
                        case Permission.TOURNAMENT_STANDARD:
                            if (accountIsAdmin)
                            {
                                targetAccount.PermissionLevel = (int)Permission.NONE;
                                services.Tournament.DeleteTournamentUser(targetAccount.TournamentUserID);
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            if (accountIsCreator)
                            {
                                targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_STANDARD;
                                services.Tournament.UpdateTournamentUser(targetAccount);
                                permissionActions["Remove"] = 1;
                                permissionActions["Promote"] = 1;
                            }
                            break;
                        case Permission.TOURNAMENT_CREATOR:
                            break;
                    }
                    break;
                default:
                    switch ((Permission)targetAccount.PermissionLevel)
                    {
                        case Permission.TOURNAMENT_STANDARD:
                            if (targetAccount.AccountID != null)
                            {
                                if (accountIsCreator)
                                {
                                    permissionActions["Remove"] = 1;
                                    permissionActions["Promote"] = 1;
                                }
                                else if (accountIsAdmin)
                                {
                                    permissionActions["Remove"] = 1;
                                }
                            }
                            else
                            {
                                if (accountIsAdmin || accountIsCreator)
                                {
                                    permissionActions["Remove"] = 1;
                                }
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            if (accountIsCreator)
                            {
                                permissionActions["Demote"] = 1;
                            }
                            break;
                        case Permission.TOURNAMENT_CREATOR:
                            break;
                    }
                    break;
            }

            permissionActions["Permission"] = targetAccount.PermissionLevel != null ? (int)targetAccount.PermissionLevel : -1;

            if (services.Save())
            {
                return permissionActions;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Check-ins
        public bool isAccountCheckedIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.AccountID == accountId);
            bool checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            return checkedIn;
        }

        public bool isUserCheckedIn(int tournamentUserId)
        {
            bool checkedIn = false;
            TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.TournamentUserID == tournamentUserId);

            if (userModel != null)
            {
                checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            }
            return checkedIn;
        }

        public bool CheckAccountIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.AccountID == accountId);

            services.Tournament.CheckUserIn(userModel.TournamentUserID);
            return services.Save();
        }

        public bool CheckUserIn(int tournamentUserId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.TournamentUserID == tournamentUserId);

            services.Tournament.CheckUserIn(userModel.TournamentUserID);
            return services.Save();
        }
        #endregion

        #region Helper
        public int GetUserSeed(int bracketId, int userId)
        {
            int? seed = -1;

            seed = Model.Brackets.Single(x => x.BracketID == bracketId)?
                .TournamentUsersBrackets.Single(x => x.TournamentUserID == userId)?.Seed;

            if (seed != null)
            {
                return seed.Value;
            }
            else
            {
                return -1;
            }
        }

        public List<TournamentUserModel> GetParticipants()
        {
            return Model.TournamentUsers.Where(x => x.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD).ToList();
        }

        public bool CanEdit()
        {
            return !Model.InProgress ? true : false;
        }

        public bool isRegistered(int accountId)
        {
            return Model.TournamentUsers.Any(x => x.AccountID == accountId);
        }

        public bool CanRegister()
        {
            if (Model.RegistrationStartDate < DateTime.Now && Model.RegistrationEndDate > DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MatchObjectFix()
        {
            foreach (BracketModel bracket in Model.Brackets)
            {
                foreach (MatchModel match in bracket.Matches)
                {
                    if (match.Defender == null)
                    {
                        match.Defender = services.Tournament.GetTournamentUser(match.DefenderID);
                    }
                    if (match.Challenger == null)
                    {
                        match.Challenger = services.Tournament.GetTournamentUser(match.ChallengerID);
                    }
                }
            }
        }
        #endregion

        #region ViewModel
        public void ApplyChanges()
        {
            // Tournament Stuff
            Model.Title = viewModel.Title;
            Model.Description = viewModel.Description;
            Model.GameTypeID = viewModel.GameTypeID;
            Model.PlatformID = viewModel.PlatformTypeID;
            Model.PublicViewing = viewModel.PublicViewing;
            Model.PublicRegistration = viewModel.PublicRegistration;

            // Adding Dates and Times
            Model.RegistrationStartDate = viewModel.RegistrationStartDate + viewModel.RegistrationStartTime.TimeOfDay;
            Model.RegistrationEndDate = viewModel.RegistrationEndDate + viewModel.RegistrationEndTime.TimeOfDay;
            Model.TournamentStartDate = viewModel.TournamentStartDate + viewModel.TournamentStartTime.TimeOfDay;
            Model.TournamentEndDate = viewModel.TournamentEndDate + viewModel.TournamentEndTime.TimeOfDay;
            Model.CheckInBegins = viewModel.CheckinStartDate + viewModel.CheckinStartTime.TimeOfDay;
            Model.CheckInEnds = viewModel.CheckinEndDate + viewModel.CheckinEndTime.TimeOfDay;
        }

        public void SetFields()
        {
            viewModel.Title = Model.Title;
            viewModel.Description = Model.Description;
            viewModel.GameTypeID = Model.GameTypeID;
            viewModel.PlatformTypeID = Model.PlatformID;
            viewModel.PublicViewing = Model.PublicViewing;
            viewModel.PublicRegistration = Model.PublicRegistration;

            // Dates
            viewModel.RegistrationStartDate = Model.RegistrationStartDate;
            viewModel.RegistrationEndDate = Model.RegistrationEndDate;
            viewModel.TournamentStartDate = Model.TournamentStartDate;
            viewModel.TournamentEndDate = Model.TournamentEndDate;
            viewModel.CheckinStartDate = Model.CheckInBegins;
            viewModel.CheckinEndDate = Model.CheckInEnds;

            // Times
            viewModel.RegistrationStartTime = Model.RegistrationStartDate;
            viewModel.RegistrationEndTime = Model.RegistrationEndDate;
            viewModel.TournamentStartTime = Model.TournamentStartDate;
            viewModel.TournamentEndTime = Model.TournamentEndDate;
            viewModel.CheckinStartTime = Model.CheckInBegins;
            viewModel.CheckinEndTime = Model.CheckInEnds;

            // Bracket data
            foreach (BracketModel bracket in Model.Brackets)
            {
                viewModel.BracketData.Add(new BracketInfo()
                {
                    BracketType = bracket.BracketTypeID,
                    NumberOfRounds = bracket.MaxRounds
                });
            }
        }
        #endregion
    }
}