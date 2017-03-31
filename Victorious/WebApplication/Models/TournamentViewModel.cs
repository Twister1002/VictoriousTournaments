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
            Administrators = new List<UserModel>();
            Participants = new List<UserModel>();
            GetUserPermissions();
        }

        public override void ApplyChanges(int SessionId)
        {
            // Tournament Stuff
            Model.Title                                 = this.Title;
            Model.Description                           = this.Description;

            // Tournament Rule Stuff
            Model.TournamentRules.IsPublic              = this.IsPublic;
            Model.TournamentRules.RegistrationStartDate = this.RegistrationStartDate;
            Model.TournamentRules.RegistrationEndDate   = this.RegistrationEndDate;
            Model.TournamentRules.TournamentStartDate   = this.TournamentStartDate;
            Model.TournamentRules.TournamentEndDate     = this.TournamentEndDate;
            Model.LastEditedOn = DateTime.Now;

            // Update bracket info
            if (Model.Brackets.Count > 0)
            {
                Model.Brackets.ToList()[0].BracketTypeID = this.BracketType;
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
            this.Title                  = Model.Title;
            this.Description            = Model.Description;

            this.IsPublic               = Model.TournamentRules.IsPublic;
            this.RegistrationStartDate  = Model.TournamentRules.RegistrationStartDate;
            this.RegistrationEndDate    = Model.TournamentRules.RegistrationEndDate;
            this.TournamentStartDate    = Model.TournamentRules.TournamentStartDate;
            this.TournamentEndDate      = Model.TournamentRules.TournamentEndDate;

            if (this.BracketType != Model.Brackets.ToList()[0].BracketTypeID)
            {
                this.BracketType = Model.Brackets.ToList()[0].BracketTypeID;
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

            if (db.interfaceException != null)
            {
                this.dbException = db.interfaceException;
                this.error = ViewError.EXCEPTION;
                this.message = "There was an error in acquiring the tournaments.";
            }

            if (title != String.Empty && title != null)
            {
                models = models.Where(t => t.Title.Contains(title)).ToList();   
            }

            SearchModels = models;
        }

        private void GetUserPermissions()
        {
            foreach (UserModel user in Model.Users)
            {
                Permission permission = db.GetUserPermission(user, Model);
                switch(permission)
                {
                    case Permission.TOURNAMENT_STANDARD:
                        Participants.Add(user);
                        break;
                    case Permission.TOURNAMENT_ADMINISTRATOR:
                        Administrators.Add(user);
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
            DbError result;
            BracketModel bracket = Model.Brackets.ElementAt(bracketNum);
            IBracket tourny = Tourny.Brackets[bracketNum];
            

            // Process
            result = CreateMatches(bracket, tourny);
            if (result == DbError.SUCCESS)
            {
                result = SaveSeedParticipants(bracket, tourny);
            }
            else
            {
                dbException = db.interfaceException;
            }

            return result;
        }

        private DbError CreateMatches(BracketModel bracket, IBracket tourny)
        {
            DbError result = DbError.NONE;
            List<MatchModel> matchModels = new List<MatchModel>();

            // Verify if the tournament has not need finalized.
            if (bracket.Matches.Count == 0)
            {
                // Add the matches to the database
                for (int i = 1; i <= tourny.NumberOfMatches; i++)
                {
                    MatchModel matchModel = tourny.GetMatch(i).GetModel(-1);

                    matchModels.Add(matchModel);
                }

                // Save the match models
                for (int i = 0; i < matchModels.Count; i++)
                {
                    DbError matchSave = db.AddMatch(matchModels[i], bracket);
                    
                    if (matchSave != DbError.SUCCESS)
                    {
                        result = matchSave;

                        // Reverse the list and remove the matches.
                        for (int x = i; x > 0; x--)
                        {
                            db.DeleteMatch(matchModels[x]);
                        }

                        break;
                    }
                }
            }

            return result;
        }

        public DbError SaveSeedParticipants(BracketModel bracket, IBracket tourny)
        {
            List<IPlayer> players = Tourny.Brackets[0].Players;
            DbError result = DbError.NONE;

            for (int i = 0; i < players.Count; i++)
            {
                UserModel userModel = db.GetUserById(players[i].Id);
                UserBracketSeedModel seedModel = new UserBracketSeedModel()
                {
                    UserID = players[i].Id,
                    TournamentID = Model.TournamentID,
                    BracketID = bracket.BracketID,
                    Seed = tourny.GetPlayerSeed(players[0].Id)
                };

                bracket.UserSeeds.Add(seedModel);
            }

            // Save the user seeds
            for (int i =0; i < bracket.UserSeeds.Count; i++)
            {
                DbError seedSave = db.SetUserBracketSeed(bracket.UserSeeds.ElementAt(i));

                if (seedSave != DbError.SUCCESS)
                {
                    for (int x = i; x > 0; x--)
                    {
                        db.DeleteUserBracketSeed(bracket.UserSeeds.ElementAt(x));
                    }

                    // Set the data
                    result = seedSave;
                    break;
                }
            }

            return result;
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
    }
}