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
            Model = model;
            SetFields();
        }

        public void ApplyChanges(int SessionId)
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

        public void SetFields()
        {
            this.Title                  = Model.Title;
            this.Description            = Model.Description;
            this.IsPublic               = Model.TournamentRules.IsPublic == null ? true : (bool)Model.TournamentRules.IsPublic;
            this.RegistrationStartDate  = Model.TournamentRules.RegistrationStartDate;
            this.RegistrationEndDate    = Model.TournamentRules.RegistrationEndDate;
            this.TournamentStartDate    = Model.TournamentRules.TournamentStartDate;
            this.TournamentEndDate      = Model.TournamentRules.TournamentEndDate;
        }

        public void Search(String title)
        {
            List<TournamentModel> models = new List<TournamentModel>();
            models = db.GetAllTournaments();

            if (title != String.Empty && title != null)
            {
                models = models.Where(t => t.Title == title).ToList();   
            }

            SearchModels = models;
        }

        public void ProcessTournament()
        {
            Tourny = new Tournament.Structure.Tournament();
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 1; i <= 10; i++)
            {
                UserModel uModel = new UserModel()
                {
                    UserID = i,
                    FirstName = "FirstName " + i,
                    LastName = "LastName " + i,
                    Username = "Player " + i,
                    Email = "Email" + i
                };

                players.Add(new User(uModel));
            }

            Tourny.AddSingleElimBracket(players);
            Tourny.Brackets[0].AddWin(1, PlayerSlot.Challenger);
            Tourny.Brackets[0].AddWin(2, PlayerSlot.Challenger);
            Tourny.Brackets[0].AddWin(3, PlayerSlot.Challenger);
            //tourny.Brackets[0].AddWin(7, PlayerSlot.Challenger);
        }
    }
}