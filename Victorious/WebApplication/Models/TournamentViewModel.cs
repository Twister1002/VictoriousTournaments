using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class TournamentViewModel : ViewModel
    {
        private bool TempFixMatchObjects = true;
        public ITournament Tourny { get; private set; }
        public TournamentModel Model { get; private set; }
        public List<TournamentModel> SearchedTournaments { get; private set; }

        public TournamentViewModel(IUnitOfWork work) : base(work)
        {
            Model = new TournamentModel();
            Init();
        }

        public TournamentViewModel(IUnitOfWork work, String id) : this(work, int.Parse(id))
        { }

        public TournamentViewModel(IUnitOfWork work, int id) : base(work)
        {
            Model = tournamentService.GetTournament(id);
            Init();

            if (Model != null)
            {
                SetFields();
            }
            else
            {
                Model = null;
            }
            ProcessTournament();
        }

        public TournamentViewModel(IUnitOfWork work, TournamentModel model) : base(work)
        {
            Init();

            if (model != null)
            {
                Model = model;
                SetFields();
            }
            
            ProcessTournament();
        }

        public void Init()
        {
            this.BracketTypes = typeService.GetAllBracketTypes().Where(x=>x.IsActive == true).ToList();
            this.GameTypes = typeService.GetAllGameTypes();
            this.PlatformTypes = typeService.GetAllPlatforms();
            this.PublicViewing = true;
            this.SearchedTournaments = new List<TournamentModel>();
            this.BracketData = new List<BracketInfo>();
            this.NumberOfRounds = Enumerable.Range(0, 100).ToList();
        }

        private void MatchObjectFix()
        {
            foreach (BracketModel bracket in Model.Brackets)
            {
                foreach (MatchModel match in bracket.Matches)
                {
                    if (match.Defender == null)
                    {
                        match.Defender = tournamentService.GetTournamentUser(match.DefenderID);
                    }
                    if (match.Challenger == null)
                    {
                        match.Challenger = tournamentService.GetTournamentUser(match.ChallengerID);
                    }
                }
            }
        }

        public void ApplyChanges()
        {
            // Tournament Stuff
            Model.Title = this.Title;
            Model.Description = this.Description;
            Model.GameTypeID = this.GameTypeID;
            Model.PlatformID = this.PlatformTypeID;
            Model.PublicViewing = this.PublicViewing;
            Model.PublicRegistration = this.PublicRegistration;

            // Adding Dates and Times
            Model.RegistrationStartDate = this.RegistrationStartDate + this.RegistrationStartTime.TimeOfDay;
            Model.RegistrationEndDate = this.RegistrationEndDate + this.RegistrationEndTime.TimeOfDay;
            Model.TournamentStartDate = this.TournamentStartDate + this.TournamentStartTime.TimeOfDay;
            Model.TournamentEndDate = this.TournamentEndDate + this.TournamentEndTime.TimeOfDay;
            Model.CheckInBegins = this.CheckinStartDate + this.CheckinStartTime.TimeOfDay;
            Model.CheckInEnds = this.CheckinEndDate + this.CheckinEndTime.TimeOfDay;
        }

        public void SetFields()
        {
            this.Title = Model.Title;
            this.Description = Model.Description;
            this.GameTypeID = Model.GameTypeID;
            this.PlatformTypeID = Model.PlatformID;
            this.PublicViewing = Model.PublicViewing;
            this.PublicRegistration = Model.PublicRegistration;

            // Dates
            this.RegistrationStartDate = Model.RegistrationStartDate;
            this.RegistrationEndDate = Model.RegistrationEndDate;
            this.TournamentStartDate = Model.TournamentStartDate;
            this.TournamentEndDate = Model.TournamentEndDate;
            this.CheckinStartDate = Model.CheckInBegins;
            this.CheckinEndDate = Model.CheckInEnds;

            // Times
            this.RegistrationStartTime = Model.RegistrationStartDate;
            this.RegistrationEndTime = Model.RegistrationEndDate;
            this.TournamentStartTime = Model.TournamentStartDate;
            this.TournamentEndTime = Model.TournamentEndDate;
            this.CheckinStartTime = Model.CheckInBegins;
            this.CheckinEndTime = Model.CheckInEnds;

            // Bracket data
            foreach (BracketModel bracket in Model.Brackets)
            {
                this.BracketData.Add(new BracketInfo()
                {
                    BracketType = bracket.BracketTypeID,
                    NumberOfRounds = bracket.MaxRounds
                });
            }
        }

        public void LoadData(int id)
        {
            Model = tournamentService.GetTournament(id);
        }

        public void LoadModel(TournamentModel model)
        {
            Model = model;
        }

        public bool Update(int sessionId)
        {
            ApplyChanges();
            Model.LastEditedByID = sessionId;
            Model.LastEditedOn = DateTime.Now;

            tournamentService.UpdateTournament(Model);
            UpdateBrackets();
            if (Save())
            {
                UpdatePlayers();
                return Save();
            }

            return Save();
        }

        //TODO: Fix issue where tournamentCodes can collide and be repeatable.
        public bool Create(int sessionId)
        {
            ApplyChanges();

            // Create the brackets
            foreach (BracketInfo brackets in this.BracketData)
            {
                BracketModel bracketModel = new BracketModel()
                {
                    BracketTypeID = brackets.BracketType,
                    MaxRounds = brackets.NumberOfRounds,
                    TournamentID = Model.TournamentID,
                    Finalized = false,
                };

                //tournamentService.AddBracket(bracketModel);
                Model.Brackets.Add(bracketModel);
            }

            Model.CreatedOn = DateTime.Now;
            Model.CreatedByID = sessionId;
            Model.InviteCode = Codes.GenerateInviteCode();

            // Save the tournament
            tournamentService.AddTournament(Model);

            // Create InviteModel
            TournamentInviteModel inviteModel = new TournamentInviteModel()
            {
                TournamentInviteCode = Model.InviteCode,
                DateCreated = DateTime.Now,
                TournamentID = Model.TournamentID,
                NumberOfUses = 0
            };

            tournamentService.AddTournamentInvite(inviteModel);
            return Save();
        }

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
            tournamentService.AddTournamentUser(model);

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

                    tournamentService.AddTournamentUsersBracket(bracketUser);
                }
            }

            return Save();
        }

        public bool RemoveUser(int accountId)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.AccountID == accountId);

            tournamentService.DeleteTournamentUser(user.TournamentUserID);
            return Save();
        }

        public bool RemoveUser(String username)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.Name == username);

            tournamentService.DeleteTournamentUser(user.TournamentUserID);
            return Save();
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
                    tournamentService.DeleteTournamentUsersBracket(user.TournamentUserID, bracket.BracketID);
                }

                tournamentService.DeleteBracket(bracket.BracketID);
            }
            
            // Delete all the users
            foreach(TournamentUserModel user in tournamentUsers)
            {
                tournamentService.DeleteTournamentUser(user.TournamentUserID);
            }

            // Delete the Invite code
            tournamentService.DeleteTournamentInvite(Model.InviteCode);
            
            // Delete the tournament
            tournamentService.DeleteTournament(Model.TournamentID);

            return Save();
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
            SearchedTournaments = tournamentService.FindTournaments(searchData);
        }

        public bool FinalizeTournament(int bracketId, Dictionary<String, Dictionary<String, int>> roundData)
        {
            // Set variables
            BracketModel bracket = Model.Brackets.Where(x => x.BracketID == bracketId).Single();
            IBracket tourny = Tourny.Brackets.Where(x => x.Id == bracketId).Single();

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

            // Process
            try
            {
                CreateMatches(bracket, tourny);

                // Recall the bracket
                bracket.Finalized = true;
                Model.InProgress = true;
            }
            catch (Exception e)
            {
                this.dbException = e;
                return false;
            }

            tournamentService.UpdateBracket(bracket);
            tournamentService.UpdateTournament(Model);
            return Save();
        }

        private void CreateMatches(BracketModel bracketModel, IBracket bracket)
        {
            // Verify if the tournament has not need finalized.
            if (!bracketModel.Finalized)
            {
                // Add the matches to the database
                for (int i = 1; i <= bracket.NumberOfMatches; i++)
                {
                    MatchModel matchModel = bracket.GetMatchModel(i);
                    tournamentService.AddMatch(matchModel);
                }
            }
        }

        private void ProcessTournament()
        {
            if (Model == null)
            {
                return;
            }

            if (TempFixMatchObjects) MatchObjectFix();
            Tourny = new Tournament.Structure.Tournament();
            Tourny.Title = Model.Title;
            foreach (BracketModel bracket in Model.Brackets)
            {
                Tourny.AddBracket(Tourny.RestoreBracket(bracket));
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
                    tournamentService.UpdateTournamentUsersBracket(user);
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

                    tournamentService.AddTournamentUsersBracket(user);
                }
            }

            Save();
        }

        #region CheckIn
        public bool isAccountCheckedIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.FirstOrDefault(x => x.AccountID == accountId);
            bool checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            return checkedIn;
        }

        public bool isUserCheckedIn(int tournamentUserId)
        {
            bool checkedIn = false;
            TournamentUserModel userModel = Model.TournamentUsers.FirstOrDefault(x => x.TournamentUserID == tournamentUserId);

            if (userModel != null)
            {
                checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            }
            return checkedIn;
        }

        public bool AccountCheckIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.First(x => x.AccountID == accountId);

            tournamentService.CheckUserIn(userModel.TournamentUserID);
            return Save();
        }

        public bool UserCheckIn(int tournamentUserId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.First(x => x.TournamentUserID == tournamentUserId);

            tournamentService.CheckUserIn(userModel.TournamentUserID);
            return Save();
        }
        #endregion
        #region Permissions 
        public Permission TournamentPermission(int accountId)
        {
            TournamentUserModel model =
                Model.TournamentUsers.FirstOrDefault(x =>
                x.AccountID == accountId);

            if (model != null)
            {
                return (Permission)model.PermissionLevel;
            }
            else
            {
                return Permission.NONE;
            }
        }

        public bool IsParticipant(int accountId)
        {
            Permission permission = TournamentPermission(accountId);
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

        public bool IsAdministrator(int accountId)
        {
            Permission permission = TournamentPermission(accountId);
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
            return TournamentPermission(accountId) == Permission.TOURNAMENT_CREATOR;
        }

        public Permission UserPermission(int tournamentUserId)
        {
            TournamentUserModel model =
                Model.TournamentUsers.FirstOrDefault(x =>
                x.TournamentUserID == tournamentUserId);

            if (model != null)
            {
                return (Permission)model.PermissionLevel;
            }
            else
            {
                return Permission.NONE;
            }
        }

        public bool IsUserParticipant(int tournamentUserId)
        {
            Permission permission = UserPermission(tournamentUserId);
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
            Permission permission = UserPermission(tournamentUserId);
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
            Permission permission = UserPermission(tournamentUserId);
            if (permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<String, int> PermissionAction(int accountId, int tournamentUserId, String action)
        {
            TournamentUserModel targetAccount = Model.TournamentUsers.First(x => x.TournamentUserID == tournamentUserId);
            bool accountIsAdmin = IsAdministrator(accountId);
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
                                tournamentService.UpdateTournamentUser(targetAccount);
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
                                tournamentService.DeleteTournamentUser(targetAccount.TournamentUserID);
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            if (accountIsCreator)
                            {
                                targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_STANDARD;
                                tournamentService.UpdateTournamentUser(targetAccount);
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

            if (Save())
            {
                return permissionActions;
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Updates the amount of brackets in the tournament
        /// </summary>
        public void UpdateBrackets()
        {
            int updates = Math.Max(this.BracketData.Count, Model.Brackets.Count);
            List<BracketModel> updatedBrackets = new List<BracketModel>();

            for (int i = 0; i < updates; i++)
            {
                BracketInfo? newBracket = this.BracketData.ElementAtOrDefault(i);
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
                            tournamentService.AddTournamentUsersBracket(model);
                        }
                    }
                }
            }
        }

        public bool CanRegister()
        {
            if (RegistrationStartDate < DateTime.Now && RegistrationEndDate > DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isRegistered(int accountId)
        {
            return Model.TournamentUsers.Any(x => x.AccountID == accountId);
        }

        public bool CanEdit()
        {
            return !Model.InProgress ? true : false;
        }

        public List<TournamentUserModel> GetParticipants()
        {
            return Model.TournamentUsers.Where(x => x.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD).ToList();
        }

        public int UserSeed(int tournamentUserId, int bracketId)
        {
            int? seedNum;
            if (Model != null)
            {
                seedNum = Model.Brackets.Where(x => x.BracketID == bracketId).Single()?
                    .TournamentUsersBrackets.Where(x => x.TournamentUserID == tournamentUserId)?.SingleOrDefault()?
                    .Seed;
            }
            else
            {
                seedNum = tournamentService.GetTournamentUsersBracket(tournamentUserId, bracketId)?.Seed;
            }

            if (seedNum != null)
            {
                return seedNum.Value;
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }
}