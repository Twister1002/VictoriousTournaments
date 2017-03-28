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

        public TournamentViewModel()
        {
            Model = new TournamentModel();
            Model.TournamentRules = new TournamentRuleModel();
            Init();
        }

        public TournamentViewModel(int id)
        {
            Model = db.GetTournamentById(id);
            Init();
            SetFields();
        }

        public TournamentViewModel(TournamentModel model)
        {
            Init();

            if (model != null)
            {
                Model = model;
                SetFields();
            }
        }

        public void Init()
        {
            this.BracketTypes = db.GetAllBracketTypes();
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
            if (Model.CreatedByID == 0 || Model.CreatedByID == null)
            {
                Model.CreatedByID = SessionId;
                Model.CreatedOn = DateTime.Now;
            }
        }

        public override void SetFields()
        {
            this.Title                  = Model.Title;
            this.Description            = Model.Description;

            this.IsPublic               = Model.TournamentRules.IsPublic == null ? true : (bool)Model.TournamentRules.IsPublic;
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

        public void CreateMatches()
        {
            ProcessTournament();

            BracketModel bracket = Model.Brackets.ToList()[0];

            if (bracket.Matches.Count == 0)
            {
                // Save the matches
                for (int i = 1; i <= Tourny.Brackets[0].NumberOfMatches; i++)
                {
                    IMatch match = Tourny.Brackets[0].GetMatch(i);
                    MatchModel matchModel = match.GetModel(-1);

                    bracket.Matches.Add(matchModel);
                }
            }
        }

        public DbError SaveMatches()
        {
            // This process somehow doubles the amount of records being saved. 
            // I believe its once for each match and all matches inside bracket.

            BracketModel bracket = Model.Brackets.ToList()[0];
            List<MatchModel> matches = bracket.Matches.ToList();
            DbError result = DbError.NONE;

            for (int i = 0; i < matches.Count; i++)
            {
                MatchModel matchModel = matches[i];
                result = db.AddMatch(ref matchModel, bracket);
                if (result != DbError.SUCCESS)
                {
                    // Loop backwards and remove all the matches that were created
                    for (int x = i; x > 0; x--)
                    {
                        db.DeleteMatch(matches[x]);
                    }

                    return result;
                }
            }

            return result;
        }

        public void ProcessTournament()
        {
            Tourny = new Tournament.Structure.Tournament();
            Tourny.Title = Model.Title;

            if (Model.Brackets.ToList()[0].Matches.Count > 0)
            {
                Tourny.AddBracket(BracketTournament());
            }
            else
            {
                Tourny.AddBracket(PlayerTournament());
            }
        }

        private IBracket BracketTournament()
        {
            IBracket bracket = null;

            switch ((int)Model.Brackets.ToList()[0].BracketTypeID)
            {
                case (int)BracketTypeModel.BracketType.SINGLE:
                    bracket = new SingleElimBracket(Model.Brackets.ToList()[0]);
                    break;
                case (int)BracketTypeModel.BracketType.DOUBLE:
                    bracket = new DoubleElimBracket(Model.Brackets.ToList()[0]);
                    break;
                case (int)BracketTypeModel.BracketType.ROUNDROBIN:
                    bracket = new RoundRobinBracket(Model.Brackets.ToList()[0]);
                    break;
            }

            return bracket;
        }

        private IBracket PlayerTournament()
        {
            IBracket bracket = null;
            List<IPlayer> players = new List<IPlayer>();

            foreach (UserModel userModel in Model.Users)
            {
                players.Add(new User(userModel));
            }

            switch ((int)Model.Brackets.ToList()[0].BracketTypeID)
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