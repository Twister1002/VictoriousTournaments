using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseLib;

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
        public AccountModel Model { get; private set; }
        public Dictionary<TournamentStatus, List<TournamentModel>> Tournaments { get; private set; }

        public AccountViewModel()
        {
            Model = new AccountModel();
            Init();
        }

        public AccountViewModel(int id)
        {
            Model = db.GetAccount(id);
            Init();
        }

        public AccountViewModel(AccountModel model)
        {
            Model = model;
            Init();
        }

        private void Init()
        {
            SetFields();

            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();

            // Filter the list down of tournaments
            //foreach (TournamentModel tourny in Model.Tournaments)
            //{
            //    // OWner of tournament
            //    if (tourny.UsersInTournament.Single(x=>x.UserID == Model.UserID).Permission == Permission.TOURNAMENT_ADMINISTRATOR)
            //    {
            //        Tournaments[TournamentStatus.ADMIN].Add(tourny);
            //    }
            //    else
            //    {
            //        // Active Tournament
            //        if (tourny.TournamentRules.TournamentStartDate <= DateTime.Now && 
            //            tourny.TournamentRules.TournamentEndDate > DateTime.Now)
            //        {
            //            Tournaments[TournamentStatus.ACTIVE].Add(tourny);
            //        }
            //        else if (tourny.TournamentRules.TournamentStartDate > DateTime.Now)
            //        {
            //            Tournaments[TournamentStatus.UPCOMING].Add(tourny);
            //        }
            //        else
            //        {
            //            Tournaments[TournamentStatus.PAST].Add(tourny);
            //        }
            //    }
            //}
        }

        public override void ApplyChanges()
        {
            // Non null fields
            Model.Username      = this.Username != String.Empty ? this.Username : String.Empty;
            Model.Email         = this.Email != String.Empty ? this.Email : String.Empty;
            Model.FirstName     = this.FirstName != String.Empty ? this.FirstName : String.Empty;
            Model.LastName      = this.LastName != String.Empty ? this.LastName : String.Empty;
            Model.Password      = this.Password != String.Empty ? this.Password : String.Empty;
        }

        public override void SetFields()
        {
            this.Username   = Model.Username;
            this.Email      = Model.Email;
            this.LastName   = Model.LastName;
            this.FirstName  = Model.FirstName;
        }

        public bool Create()
        {
            bool usernameExists = db.AccountUsernameExists(Username) == DbError.EXISTS;
            bool emailExists = db.AccountEmailExists(Email) == DbError.SUCCESS;
            bool passwordsMatch = Password == PasswordVerify;


            if (!usernameExists && !emailExists && passwordsMatch)
            {
                return db.AddAccount(Model) == DbError.SUCCESS;
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            return db.UpdateAccount(Model) == DbError.SUCCESS;
        }

        public bool Login()
        {
            AccountModel user = db.GetAccount(Username);
            if (user.Password == Password)
            {
                Model.LastLogin = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}