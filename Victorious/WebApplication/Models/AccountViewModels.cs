using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{

    public enum TournamentStatus
    {
        ADMIN,
        ACTIVE,
        UPCOMING,
        PAST
    };

    public class AccountViewModel : AccountFields
    {
        public UserModel Model { get; private set; }
        public Dictionary<TournamentStatus, List<TournamentModel>> Tournaments { get; private set; }

        public AccountViewModel()
        {
            Model = new UserModel();
            Init();
        }

        public AccountViewModel(int id)
        {
            Model = db.GetUserById(id);
            Init();
        }

        public AccountViewModel(UserModel model)
        {
            Model = model;
            Init();
        }

        private void Init()
        {
            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();

            // Filter the list down of tournaments
            foreach (TournamentModel tourny in Model.Tournaments)
            {
                // OWner of tournament
                if (tourny.UsersInTournament.Single(x=>x.UserID == Model.UserID).Permission == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    Tournaments[TournamentStatus.ADMIN].Add(tourny);
                }
                else
                {
                    // Active Tournament
                    if (tourny.TournamentRules.TournamentStartDate <= DateTime.Now && 
                        tourny.TournamentRules.TournamentEndDate > DateTime.Now)
                    {
                        Tournaments[TournamentStatus.ACTIVE].Add(tourny);
                    }
                    else if (tourny.TournamentRules.TournamentStartDate > DateTime.Now)
                    {
                        Tournaments[TournamentStatus.UPCOMING].Add(tourny);
                    }
                    else
                    {
                        Tournaments[TournamentStatus.PAST].Add(tourny);
                    }
                }
            }
        }

        public override void ApplyChanges()
        {
            // Non null fields
            Model.Username      = this.Username != String.Empty ? this.Username : String.Empty;
            Model.Email         = this.Email != String.Empty ? this.Email : String.Empty;

            // Null fields
            Model.FirstName     = this.FirstName;
            Model.LastName      = this.LastName;
            Model.Password      = this.Password;
        }

        public override void SetFields()
        {
            this.Username   = Model.Username;
            this.Email      = Model.Email;
            this.LastName   = Model.LastName;
            this.FirstName  = Model.FirstName;
        }

        public void setUserModel()
        {
            if (Model != null)
            {
                Model = db.GetUserById(Model.UserID);
            }
        }

        public void setUserModel(int id)
        {
            if (id > 0)
            {
                Model = db.GetUserById(id);
            }
        }

        public void setUserModel(String name)
        {
            if (name != String.Empty)
            {
                Model = db.GetUserByUsername(name);
            }
        }
    }
}