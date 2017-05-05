using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;

namespace WebApplication.Models
{
    public class TournamentViewModel : TournamentFields
    {
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
            Model = db.GetTournament(id);
            if (Model.TournamentID != -1)
            {
                SetFields();
                Init();
            }
            else
            {
                Model = null;
            }
        }

        public TournamentViewModel(TournamentModel model)
        {
            if (model != null)
            {
                Model = model;
                SetFields();
            }
            Init();
        }

        public void Init()
        {
            this.BracketTypes = db.GetAllBracketTypes();
            this.GameTypes = db.GetAllGameTypes();
            Administrators = new List<TournamentUserModel>();
            Participants = new List<TournamentUserModel>();
            GetUserPermissions();

            Search(null);
        }

        public override void ApplyChanges()
        {
            // Tournament Stuff
            Model.Title = this.Title;
            Model.Description = this.Description;
            Model.GameTypeID = this.GameType;

            // Tournament Rule Stuff
            Model.IsPublic = this.IsPublic;
            Model.RegistrationStartDate = this.RegistrationStartDate;
            Model.RegistrationEndDate = this.RegistrationEndDate;
            Model.TournamentStartDate = this.TournamentStartDate;
            Model.TournamentEndDate = this.TournamentEndDate;
            Model.CheckInBegins = this.CheckinStart;
            Model.CheckInEnds = this.CheckinEnd;
        }

        public override void SetFields()
        {
            this.Title = Model.Title;
            this.Description = Model.Description;
            this.GameType = Model.GameTypeID;

            this.IsPublic = Model.IsPublic;
            this.RegistrationStartDate = Model.RegistrationStartDate;
            this.RegistrationEndDate = Model.RegistrationEndDate;
            this.TournamentStartDate = Model.TournamentStartDate;
            this.TournamentEndDate = Model.TournamentEndDate;
            this.CheckinStart = Model.CheckInBegins;
            this.CheckinEnd = Model.CheckInEnds;

            if (Model.Brackets.Count > 0 && this.BracketType != Model.Brackets.ElementAt(0).BracketTypeID)
            {
                this.BracketType = Model.Brackets.ElementAt(0).BracketTypeID;
            }
        }

        public void LoadData(int id)
        {
            Model = db.GetTournament(id);
        }

        public bool Update(int sessionId)
        {
            ApplyChanges();
            Model.LastEditedByID = sessionId;
            Model.LastEditedOn = DateTime.Now;

            DbError updateResult = db.UpdateTournament(Model);

            return updateResult == DbError.SUCCESS;
        }

        public bool Create(int sessionId)
        {
            ApplyChanges();

            Model.CreatedOn = DateTime.Now;
            Model.CreatedByID = sessionId;

            // Create the bracket
            BracketModel bracketModel = new BracketModel()
            {
                BracketTypeID = this.BracketType,
                Tournament = Model
            };

            Model.Brackets.Add(bracketModel);
            DbError createResult = db.AddTournament(Model);
            
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
                    Username = account.Username,
                    Name = account.Username,
                    PermissionLevel = (int)permission,
                    TournamentID = Model.TournamentID,
                    Tournament = Model
                };

                DbError addResult = db.AddTournamentUser(tournamentUserModel);

                return addResult == DbError.SUCCESS;
            }
        }

        public bool AddUser(String username)
        {
            TournamentUserModel tournamentUserModel = new TournamentUserModel()
            {
                Username = username,
                PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
            };

            DbError addResult = db.AddTournamentUser(tournamentUserModel);

            return addResult == DbError.SUCCESS;
        }

        public bool RemoveUser(int accountId)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.AccountID == accountId);
            DbError removeResult = db.DeleteTournamentUser(user.TournamentUserID);

            return removeResult == DbError.SUCCESS;
        }

        public bool RemoveUser(String username)
        {
            TournamentUserModel user = Model.TournamentUsers.First(x => x.Username == username);
            DbError removeResult = db.DeleteTournamentUser(user.TournamentUserID);

            return removeResult == DbError.SUCCESS;
        }

        public bool Delete()
        {
            return db.DeleteTournament(Model.TournamentID) == DbError.SUCCESS;
        }

        public void Search(Dictionary<String, String> searchData)
        {
            if (searchData != null)
            {
                List<String> safeParamList = new List<string>() { "title", "startDate", "gameType", "gameTypeId" };
                searchData = searchData.Where(k => safeParamList.Contains(k.Key)).ToDictionary(k => k.Key, k => k.Value);
                SearchedTournaments = db.FindTournaments(searchData);
            }
            else
            {
                SearchedTournaments = new List<TournamentModel>();
            }
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

        public bool FinalizeTournament(Dictionary<String, int> roundData)
        {
            // Load the tournament first
            ProcessTournament();

            // Set variables
            int bracketNum = 0;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);
            IBracket tourny = Tourny.Brackets[bracketNum];

            // Set max games for every round
            foreach (KeyValuePair<String, int> data in roundData)
            {
                tourny.SetMaxGamesForWholeRound(int.Parse(data.Key), data.Value);
                tourny.SetMaxGamesForWholeLowerRound(int.Parse(data.Key), data.Value);
            }

            // Verify the grand final round
            if (tourny.GrandFinal != null)
            {
                tourny.GrandFinal.SetMaxGames(roundData.Last().Value);
            }

            // Process
            try
            {
                SaveSeedParticipants(bracket, tourny);
                CreateMatches(bracket, tourny);

                // Recall the bracket
                bracket.Finalized = true;
            }
            catch (Exception e)
            {
                this.dbException = e;
                return false;
            }

            return db.UpdateBracket(bracket) == DbError.SUCCESS;
        }

        private void CreateMatches(BracketModel bracket, IBracket tourny)
        {
            // Verify if the tournament has not need finalized.
            if (bracket.Matches.Count == 0)
            {
                // Add the matches to the database
                for (int i = 1; i <= tourny.NumberOfMatches; i++)
                {
                    MatchModel matchModel = tourny.GetMatch(i).GetModel();

                    bracket.Matches.Add(matchModel);
                }
            }
        }

        private void SaveSeedParticipants(BracketModel bracket, IBracket tourny)
        {
            List<IPlayer> players = Tourny.Brackets[0].Players;

            for (int i = 0; i < players.Count; i++)
            {
                TournamentUserModel userModel = Model.TournamentUsers.First(x => x.Username == players[i].Name);

                TournamentUsersBracketModel userSeed = new TournamentUsersBracketModel()
                {
                    TournamentUserID = userModel.TournamentUserID,
                    BracketID = bracket.BracketID,
                    Seed = tourny.GetPlayerSeed(players[i].Id)
                };

                //db.AddTournamentUserToBracket(userSeed);
                db.AddTournamentUserToBracket(userModel.TournamentUserID, bracket.BracketID, tourny.GetPlayerSeed(players[i].Id));
            }
        }

        public void ProcessTournament()
        {
            if (Model == null)
            {
                return;
            }

            int bracketNum = 0;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);

            Tourny = new Tournament.Structure.Tournament();
            Tourny.Title = Model.Title;

            if (bracket.Matches.Count > 0)
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

        public Dictionary<String, object> ChangePermission(int sessionId, int accountId, String action)
        {
            Dictionary<String, object> result = new Dictionary<String, object>();

            TournamentUserModel sessionAccount = Model.TournamentUsers.First(x => x.AccountID == sessionId);
            TournamentUserModel targetAccount = Model.TournamentUsers.First(x => x.AccountID == accountId);
            Dictionary<String, bool> permissionActions = new Dictionary<string, bool>();
            permissionActions.Add("Demote", false);
            permissionActions.Add("Promote", false);
            permissionActions.Add("Remove", false);

            Permission permission = Permission.NONE;
            DbError dbResult = DbError.NONE;

            if (TournamentPermission(sessionId) == Permission.TOURNAMENT_ADMINISTRATOR)
            {
                switch (action)
                {
                    case "promote":
                        // Only the creator can do this
                        if (sessionAccount.PermissionLevel == (int)Permission.TOURNAMENT_CREATOR)
                        {
                            // Process the request
                            if (targetAccount.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD)
                            {
                                targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_ADMINISTRATOR;

                                dbResult = db.UpdateTournamentUser(targetAccount);
                                if (dbResult == DbError.SUCCESS)
                                {
                                    permissionActions["Demote"] = true;
                                }
                            }
                        }
                        break;
                    case "demote":
                        if (targetAccount.PermissionLevel == (int)Permission.TOURNAMENT_ADMINISTRATOR &&
                            sessionAccount.PermissionLevel == (int)Permission.TOURNAMENT_CREATOR)
                        {
                            // Only the creator can do this
                            // Demote to a regular participant
                            dbResult = db.UpdateTournamentUser(targetAccount);

                            if (dbResult == DbError.SUCCESS)
                            {
                                permission = Permission.TOURNAMENT_STANDARD;
                                permissionActions.Add("Promote", true);
                                permissionActions.Add("Remove", true);
                            }
                        }
                        else if (targetAccount.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD)
                        {
                            // Remove this user
                            dbResult = db.DeleteTournamentUser(targetAccount.TournamentUserID);

                            permission = Permission.NONE;
                        }

                        break;
                }

                switch (dbResult)
                {
                    case DbError.SUCCESS:
                        result["status"] = true;
                        result["message"] = "The action to " + action + " was successful";
                        result["permissionChange"] = permission;
                        result["actions"] = permissionActions;
                        break;
                    case DbError.NONE:
                        result["status"] = false;
                        result["message"] = "No action was taken.";
                        break;
                    default:
                        result["status"] = false;
                        result["message"] = "Failed to " + action + " the selected user.";
                        break;
                }
            }
            else
            {
                result["status"] = false;
                result["message"] = "You do not have permission to do this.";
            }

            return result;
        }
    }
}