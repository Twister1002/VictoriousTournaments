using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class TournamentViewModel : TournamentFields
    {
        private bool matchPlayerFix = false;

        public ITournament Tourny { get; private set; }
        public TournamentModel Model { get; private set; }
        public List<TournamentUserModel> Administrators { get; private set; }
        public List<TournamentUserModel> Participants { get; private set; }
        public List<TournamentModel> SearchedTournaments { get; private set; }

        public TournamentViewModel()
        {
            Model = new TournamentModel();
            Init();
        }

        public TournamentViewModel(String id) : this(int.Parse(id))
        { }

        public TournamentViewModel(int id)
        {
            Init();

            Model = db.GetTournament(id);
            if (Model != null)
            {
                SetFields();
                GetUserPermissions();
            }
            else
            {
                Model = null;
            }
            ProcessTournament();
        }

        public TournamentViewModel(TournamentModel model)
        {
            Init();

            if (model != null)
            {
                Model = model;
                SetFields();
            }

            GetUserPermissions();
            ProcessTournament();
        }

        public void Init()
        {
            this.BracketTypes = db.GetAllBracketTypes();
            this.GameTypes = db.GetAllGameTypes();
            this.PlatformTypes = db.GetAllPlatforms();
            this.PublicViewing = true;
            Administrators = new List<TournamentUserModel>();
            Participants = new List<TournamentUserModel>();
            SearchedTournaments = new List<TournamentModel>();
        }

        public override void ApplyChanges()
        {
            // Tournament Stuff
            Model.Title = this.Title;
            Model.Description = this.Description;
            Model.GameTypeID = this.GameType;
            Model.PlatformID = this.PlatformType;
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

        public override void SetFields()
        {
            this.Title = Model.Title;
            this.Description = Model.Description;
            this.GameType = Model.GameTypeID;
            this.PlatformType = Model.PlatformID;
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

            if (Model.Brackets.Count > 0 && this.BracketType != Model.Brackets.ElementAt(0).BracketTypeID)
            {
                this.BracketType = Model.Brackets.ElementAt(0).BracketTypeID;
            }
        }

        private void MatchPlayerFix()
        {
            foreach (var bracket in Model.Brackets)
            {
                foreach (var match in bracket.Matches)
                {
                    Model.Brackets.First(x => x.BracketID == bracket.BracketID)
                        .Matches.First(x => x.MatchID == match.MatchID)
                        .Challenger = db.GetTournamentUser(match.ChallengerID);

                    Model.Brackets.First(x => x.BracketID == bracket.BracketID)
                        .Matches.First(x => x.MatchID == match.MatchID)
                        .Defender = db.GetTournamentUser(match.DefenderID);
                }
            }
        }

        public void LoadData(int id)
        {
            Model = db.GetTournament(id);
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

            BracketModel bracket = Model.Brackets.ElementAt(0);
            bracket.BracketTypeID = this.BracketType;

            DbError updateTournament = db.UpdateTournament(Model);
            DbError updateBracket = db.UpdateBracket(bracket);

            if (updateTournament == DbError.SUCCESS && this.Users != null)
            {
                // Lets update the new users that were created.
                foreach (TournamentUserModel user in Users)
                {
                    user.TournamentID = Model.TournamentID;
                    user.IsCheckedIn = false;
                    user.PermissionLevel = (int)Permission.TOURNAMENT_STANDARD;

                    db.AddTournamentUser(user);
                }

            }

            return updateTournament == DbError.SUCCESS;
        }

        public bool Create(int sessionId)
        {
            ApplyChanges();

            // Create the bracket
            BracketModel bracketModel = new BracketModel()
            {
                BracketTypeID = this.BracketType,
                Tournament = Model
            };

            Model.Brackets.Add(bracketModel);

            Model.CreatedOn = DateTime.Now;
            Model.CreatedByID = sessionId;

            DbError createResult = DbError.NONE;
            do
            {
                Model.InviteCode = Codes.GenerateInviteCode();
                createResult = db.AddTournament(Model);
            }
            while (createResult == DbError.INVITE_CODE_EXISTS);

            return createResult == DbError.SUCCESS;
        }

        public bool AddUser(int accountId, Permission permission)
        {
            // Verify this user doesn't exist in the tournament
            if (Model.TournamentUsers.Any(x => x.AccountID == accountId))
            {
                return false;
            }
            else
            {
                AccountModel account = db.GetAccount(accountId);
                TournamentUserModel tournamentUserModel = new TournamentUserModel()
                {
                    AccountID = account.AccountID,
                    Name = account.Username,
                    PermissionLevel = (int)permission,
                    TournamentID = Model.TournamentID,
                    Tournament = Model
                };

                DbError addResult = db.AddTournamentUser(tournamentUserModel);

                return addResult == DbError.SUCCESS;
            }
        }

        public bool RemoveUser(int accountId)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.AccountID == accountId);
            DbError removeResult = db.DeleteTournamentUser(user.TournamentUserID);

            return removeResult == DbError.SUCCESS;
        }

        public bool RemoveUser(String username)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.Name == username);
            DbError removeResult = db.DeleteTournamentUser(user.TournamentUserID);

            return removeResult == DbError.SUCCESS;
        }

        public bool Delete()
        {
            return db.DeleteTournament(Model.TournamentID) == DbError.SUCCESS;
        }

        public void Search(Dictionary<String, String> searchData)
        {
            if (searchData == null)
            {
                searchData = new Dictionary<String, String>();
            }

            searchData.Add("PublicViewing", "true");
            List<String> safeParamList = new List<String>() { "Title", "GameTypeID", "PlatformID", "TournamentStartDate", "PublicViewing", "PublicRegistration" };
            searchData = searchData.Where(k => safeParamList.Contains(k.Key) && k.Value != String.Empty).ToDictionary(k => k.Key, k => k.Value);
            SearchedTournaments = db.FindTournaments(searchData);
        }

        private void GetUserPermissions()
        {
            foreach (TournamentUserModel user in Model.TournamentUsers)
            {
                switch ((Permission)user.PermissionLevel)
                {
                    case Permission.TOURNAMENT_STANDARD:
                        Participants.Add(user);
                        break;
                    case Permission.TOURNAMENT_ADMINISTRATOR:
                    case Permission.TOURNAMENT_CREATOR:
                        Administrators.Add(user);
                        break;
                }
            }
        }

        public bool FinalizeTournament(Dictionary<String, Dictionary<String, int>> roundData)
        {
            // Set variables
            int bracketNum = 0;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);
            IBracket tourny = Tourny.Brackets[bracketNum];

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
                            tourny.GrandFinal.SetMaxGames(data.Value);
                            break;
                    }
                }
            }

            // Process
            try
            {
                SaveSeedParticipants(bracket, tourny);
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

            bool bracketUpdated = db.UpdateBracket(bracket) == DbError.SUCCESS;
            bool TournamentUpdated = db.UpdateTournament(Model) == DbError.SUCCESS;

            return bracketUpdated && TournamentUpdated;
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

                    bracketModel.Matches.Add(matchModel);
                }
            }
        }

        private void SaveSeedParticipants(BracketModel bracket, IBracket tourny)
        {
            List<IPlayer> players = Tourny.Brackets[0].Players;

            for (int i = 0; i < players.Count; i++)
            {
                TournamentUserModel userModel = Model.TournamentUsers.First(x => x.Name == players[i].Name);

                TournamentUsersBracketModel userSeed = new TournamentUsersBracketModel()
                {
                    TournamentUserID = userModel.TournamentUserID,
                    BracketID = bracket.BracketID,
                    Seed = tourny.GetPlayerSeed(players[i].Id)
                };

                db.AddTournamentUsersBracket(userSeed);
            }
        }

        private void ProcessTournament()
        {
            if (Model == null)
            {
                return;
            }

            if (matchPlayerFix) MatchPlayerFix();

            int bracketNum = 0;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);

            Tourny = new Tournament.Structure.Tournament();
            Tourny.Title = Model.Title;

            if (bracket.Finalized)
            {
                Tourny.AddBracket(BracketTournament(bracket));
            }
            else
            {
                Tourny.AddBracket(PlayerTournament(bracket));
            }
        }

        private IBracket BracketTournament(BracketModel bracketModel)
        {
            IBracket bracket = null;

            switch ((BracketType)bracketModel.BracketTypeID)
            {
                case DatabaseLib.BracketType.SINGLE:
                    bracket = new SingleElimBracket(bracketModel);
                    break;
                case DatabaseLib.BracketType.DOUBLE:
                    bracket = new DoubleElimBracket(bracketModel);
                    break;
                case DatabaseLib.BracketType.ROUNDROBIN:
                    bracket = new RoundRobinBracket(bracketModel);
                    break;
                case DatabaseLib.BracketType.SWISS:
                    bracket = new SwissBracket(bracketModel);
                    break;
            }

            return bracket;
        }

        private IBracket PlayerTournament(BracketModel bracketModel)
        {
            IBracket bracket = null;
            List<IPlayer> players = new List<IPlayer>();

            foreach (TournamentUserModel userModel in Participants)
            {
                players.Add(new User(userModel));
            }

            switch ((BracketType)bracketModel.BracketTypeID)
            {
                case DatabaseLib.BracketType.SINGLE:
                    bracket = new SingleElimBracket(players);
                    break;
                case DatabaseLib.BracketType.DOUBLE:
                    bracket = new DoubleElimBracket(players);
                    break;
                case DatabaseLib.BracketType.ROUNDROBIN:
                    bracket = new RoundRobinBracket(players);
                    break;
                case DatabaseLib.BracketType.SWISS:
                    bracket = new SwissBracket(players);
                    break;
            }

            return bracket;
        }

        public bool isAccountCheckedIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.FirstOrDefault(x => x.AccountID == accountId);
            bool checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            return checkedIn;
        }

        public bool isUserCheckedIn(int tournamentUserId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.FirstOrDefault(x => x.TournamentUserID == tournamentUserId);
            bool checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
            return checkedIn;
        }

        public bool AccountCheckIn(int accountId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.First(x => x.AccountID == accountId);

            return db.CheckUserIn(userModel.TournamentUserID) == DbError.SUCCESS;
        }

        public bool UserCheckIn(int tournamentUserId)
        {
            TournamentUserModel userModel = Model.TournamentUsers.First(x => x.TournamentUserID == tournamentUserId);

            return db.CheckUserIn(userModel.TournamentUserID) == DbError.SUCCESS;
        }

        public bool isRegistered(int accountId)
        {
            return Model.TournamentUsers.Any(x => x.AccountID == accountId);
        }

        public bool CanEdit()
        {
            return !Model.InProgress ? true : false;
        }

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
            DbError dbResult = DbError.NONE;
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
                                dbResult = db.UpdateTournamentUser(targetAccount);
                                permissionActions["Demote"] = 1;
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            break;
                        case Permission.TOURNAMENT_CREATOR:
                            break;
                    }
                    break;
                case "demote":
                    switch ((Permission)targetAccount.PermissionLevel)
                    {
                        case Permission.TOURNAMENT_STANDARD:
                            if (accountIsAdmin)
                            {
                                dbResult = db.DeleteTournamentUser(targetAccount.TournamentUserID);
                            }
                            break;
                        case Permission.TOURNAMENT_ADMINISTRATOR:
                            if (accountIsCreator)
                            {
                                targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_STANDARD;
                                dbResult = db.UpdateTournamentUser(targetAccount);
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
                            if (accountIsCreator)
                            {
                                permissionActions["Remove"] = 1;
                                permissionActions["Promote"] = 1;
                            }
                            else if (accountIsAdmin)
                            {
                                permissionActions["Remove"] = 1;
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

            if (dbResult == DbError.SUCCESS || dbResult == DbError.NONE)
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
        #endregion
    }
}