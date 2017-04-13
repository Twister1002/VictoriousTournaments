using DataLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class TournamentViewModel : TournamentFields
    {
        public ITournament Tourny { get; private set; }
        public TournamentModel Model { get; private set; }
        public List<TournamentModel> SearchModels { get; private set; }
        public List<UserModel> Administrators { get; private set; }
        public List<UserModel> Participants { get; private set; }
        public String titleSearch = "";

        public TournamentViewModel()
        {
            Model = new TournamentModel();
            Model.TournamentRules = new TournamentRuleModel();
            Init();
        }

        public TournamentViewModel(int id)
        {
            Model = db.GetTournamentById(id);
            SetFields();
            Init();
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
            Administrators = new List<UserModel>();
            Participants = new List<UserModel>();
            GetUserPermissions();
        }

        public override void ApplyChanges(int SessionId)
        {
            // Tournament Stuff
            Model.Title = this.Title;
            Model.Description = this.Description;
            Model.GameTypeID = this.GameType;

            // Tournament Rule Stuff
            Model.TournamentRules.IsPublic = this.IsPublic;
            Model.TournamentRules.RegistrationStartDate = this.RegistrationStartDate;
            Model.TournamentRules.RegistrationEndDate = this.RegistrationEndDate;
            Model.TournamentRules.TournamentStartDate = this.TournamentStartDate;
            Model.TournamentRules.TournamentEndDate = this.TournamentEndDate;
            Model.LastEditedByID = SessionId;
            Model.LastEditedOn = DateTime.Now;

            // Update bracket info
            if (Model.Brackets.Count > 0)
            {
                Model.Brackets.ElementAt(0).BracketTypeID = this.BracketType;
            }
            else
            {
                Model.Brackets.Add(new BracketModel() { BracketTypeID = this.BracketType, Tournament = Model });
            }

            // Tournament Creator stuff
            if (Model.CreatedByID == 0)
            {
                Model.CreatedByID = SessionId;
                Model.CreatedOn = DateTime.Now;
            }
        }

        public override void SetFields()
        {
            this.Title = Model.Title;
            this.Description = Model.Description;
            this.GameType = Model.GameTypeID;

            this.IsPublic = Model.TournamentRules.IsPublic;
            this.RegistrationStartDate = Model.TournamentRules.RegistrationStartDate;
            this.RegistrationEndDate = Model.TournamentRules.RegistrationEndDate;
            this.TournamentStartDate = Model.TournamentRules.TournamentStartDate;
            this.TournamentEndDate = Model.TournamentRules.TournamentEndDate;

            if (this.BracketType != Model.Brackets.ElementAt(0).BracketTypeID)
            {
                this.BracketType = Model.Brackets.ElementAt(0).BracketTypeID;
            }
        }

        public void SetModel(int id)
        {
            this.Model = db.GetTournamentById(id);
        }

        public void SetModel(TournamentModel model)
        {
            this.Model = model;
        }

        public void Search(String title)
        {
            List<TournamentModel> models = new List<TournamentModel>();
            models = db.GetAllTournaments();

            if (title != String.Empty && title != null)
            {
                titleSearch = title;

                models = models.Where(t => t.Title.Contains(title)).ToList();
            }

            SearchModels = models;
        }

        private void GetUserPermissions()
        {
            foreach (UserInTournamentModel user in Model.UsersInTournament)
            {
                switch (user.Permission)
                {
                    case Permission.TOURNAMENT_STANDARD:
                        Participants.Add(user.User);
                        break;
                    case Permission.TOURNAMENT_ADMINISTRATOR:
                        Administrators.Add(user.User);
                        break;
                }
            }
        }

        public DbError FinalizeTournament()
        {
            // Load the tournament first
            ProcessTournament();

            // Set variables
            int bracketNum = 0;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);
            IBracket tourny = Tourny.Brackets[bracketNum];

            // Process
            try
            {
                CreateMatches(bracket, tourny);
                SaveSeedParticipants(bracket, tourny);
                bracket.Finalized = true;
            }
            catch (Exception e)
            {
                this.dbException = e;
                return DbError.ERROR;
            }

            return db.UpdateBracket(bracket);
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

        public void SaveSeedParticipants(BracketModel bracket, IBracket tourny)
        {
            List<IPlayer> players = Tourny.Brackets[0].Players;

            for (int i = 0; i < players.Count; i++)
            {
                UserModel userModel = db.GetUserById(players[i].Id);
                UserBracketSeedModel seedModel = new UserBracketSeedModel()
                {
                    UserID = players[i].Id,
                    TournamentID = Model.TournamentID,
                    BracketID = bracket.BracketID,
                    Seed = tourny.GetPlayerSeed(players[i].Id)
                };

                bracket.UserSeeds.Add(seedModel);
            }
        }

        public void ProcessTournament()
        {
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

            // Progress a few matches
            //Tourny.Brackets[0].AddWin(1, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(2, PlayerSlot.Defender);
            //Tourny.Brackets[0].AddWin(3, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(4, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(5, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(6, PlayerSlot.Defender);
            //Tourny.Brackets[0].AddWin(7, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(8, PlayerSlot.Defender);
        }

        private IBracket BracketTournament(BracketModel bracketModel)
        {
            IBracket bracket = null;

            switch ((int)bracketModel.BracketTypeID)
            {
                case (int)BracketTypeModel.BracketType.SINGLE:
                    bracket = new SingleElimBracket(bracketModel);
                    break;
                case (int)BracketTypeModel.BracketType.DOUBLE:
                    bracket = new DoubleElimBracket(bracketModel);
                    break;
                case (int)BracketTypeModel.BracketType.ROUNDROBIN:
                    bracket = new RoundRobinBracket(bracketModel);
                    break;
            }

            return bracket;
        }

        private IBracket PlayerTournament(BracketModel bracketModel)
        {
            IBracket bracket = null;
            List<IPlayer> players = new List<IPlayer>();

            foreach (UserModel userModel in Participants)
            {
                players.Add(new User(userModel));
            }

            switch (bracketModel.BracketTypeID)
            {
                case (int)BracketTypeModel.BracketType.SINGLE:
                    bracket = new SingleElimBracket(players);
                    break;
                case (int)BracketTypeModel.BracketType.DOUBLE:
                    bracket = new DoubleElimBracket(players);
                    break;
                case (int)BracketTypeModel.BracketType.ROUNDROBIN:
                    bracket = new RoundRobinBracket(players);
                    break;
            }

            return bracket;
        }

        public Permission UserPermission(int userId)
        {
            UserInTournamentModel model =
                Model.UsersInTournament.FirstOrDefault(x =>
                x.UserID == userId);

            if (model != null)
            {
                return model.Permission;
            }
            else
            {
                return Permission.NONE;
            }
        }

        public Dictionary<String, dynamic> ChangePermission(int actionUserId, int userId, String action)
        {
            Dictionary<String, dynamic> result = new Dictionary<String, dynamic>();
            UserInTournamentModel userInTournamentModel = Model.UsersInTournament.First(x => x.UserID == userId);
            int permissionString = -1;
            dynamic permissionActions = new { Demote = false, Promote = false, Remove = false };

            if (UserPermission(actionUserId) == Permission.TOURNAMENT_ADMINISTRATOR)
            {
                DbError dbResult = DbError.NONE;

                switch (action)
                {
                    case "promote":
                        // Only the creator can do this
                        if (Model.CreatedByID == actionUserId)
                        {
                            // Process the request
                            if (userInTournamentModel.Permission == Permission.TOURNAMENT_STANDARD)
                            {
                                dbResult = db.UpdateUserTournamentPermission(userInTournamentModel.User, Model, Permission.TOURNAMENT_ADMINISTRATOR);
                                permissionString = 2;
                                permissionActions = new { Demote = true };
                            }
                        }
                        break;
                    case "demote":
                        if (userInTournamentModel.Permission == Permission.TOURNAMENT_ADMINISTRATOR &&
                            Model.CreatedByID == actionUserId)
                        {
                            // Only the creator can do this
                            // Demote to a regular participant
                            dbResult = db.UpdateUserTournamentPermission(userInTournamentModel.User, Model, Permission.TOURNAMENT_STANDARD);
                            permissionString = 1;
                            permissionActions = new { Promote = true, Remove = true };
                        }
                        else if (userInTournamentModel.Permission == Permission.TOURNAMENT_STANDARD)
                        {
                            // Remove this user
                            dbResult = db.RemoveUserFromTournament(Model, userInTournamentModel.User);
                            permissionString = 0;
                        }

                        break;
                }

                switch (dbResult)
                {
                    case DbError.SUCCESS:
                        result["status"] = true;
                        result["message"] = "The action to " + action + " was successful";
                        result["permissionChange"] = permissionString;
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