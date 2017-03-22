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
        }

        public TournamentViewModel(int id)
        {
            Model = db.GetTournamentById(id);
            SetFields();
        }

        public TournamentViewModel(TournamentModel model)
        {
            if (model != null)
            {
                Model = model;
                SetFields();
            }
        }

        public override void ApplyChanges(int SessionId)
        {
            Model.Title                                 = this.Title;
            Model.Description                           = this.Description;
            Model.TournamentRules.IsPublic              = this.IsPublic;
            Model.TournamentRules.RegistrationStartDate = this.RegistrationStartDate;
            Model.TournamentRules.RegistrationEndDate   = this.RegistrationEndDate;
            Model.TournamentRules.TournamentStartDate   = this.TournamentStartDate;
            Model.TournamentRules.TournamentEndDate     = this.TournamentEndDate;
            Model.LastEditedOn = DateTime.Now;

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

        public void ProcessTournament()
        {
            Tourny = new Tournament.Structure.Tournament();
            List<IPlayer> players = new List<IPlayer>();

            //for (int i = 1; i <= 8; i++)
            //{
            //    UserModel uModel = new UserModel()
            //    {
            //        UserID = i,
            //        FirstName = "FirstName " + i,
            //        LastName = "LastName " + i,
            //        Username = "Player " + i,
            //        Email = "Email" + i
            //    };

            //    players.Add(new User(uModel));
            //}

            foreach (UserModel userModel in Model.Users)
            {
                players.Add(new User(userModel));
            }

            Tourny.AddSingleElimBracket(players);
            Tourny.AddDoubleElimBracket(players);
            Tourny.AddRoundRobinBracket(players);
            //Tourny.Brackets[0].AddWin(1, PlayerSlot.Challenger);
            //Tourny.Brackets[1].AddWin(1, PlayerSlot.Challenger);
            //Tourny.Brackets[2].AddWin(1, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(2, PlayerSlot.Challenger);
            //Tourny.Brackets[1].AddWin(2, PlayerSlot.Challenger);
            //Tourny.Brackets[2].AddWin(2, PlayerSlot.Challenger);
            //Tourny.Brackets[0].AddWin(3, PlayerSlot.Challenger);
            //tourny.Brackets[0].AddWin(7, PlayerSlot.Challenger);
        }
    }
}