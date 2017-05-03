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
        public AccountModel Account { get; private set; }
        public Dictionary<TournamentStatus, List<TournamentModel>> Tournaments { get; private set; }

        public AccountViewModel()
        {
            Account = new AccountModel();
            Init();
        }

        public AccountViewModel(int id)
        {
            Account = db.GetAccount(id);
            Init();
        }

        public AccountViewModel(AccountModel model)
        {
            Account = model;
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
            Account.Username      = this.Username != String.Empty ? this.Username : String.Empty;
            Account.Email         = this.Email != String.Empty ? this.Email : String.Empty;
            Account.FirstName     = this.FirstName != String.Empty ? this.FirstName : String.Empty;
            Account.LastName      = this.LastName != String.Empty ? this.LastName : String.Empty;
            Account.Password      = this.Password != String.Empty ? this.Password : String.Empty;
        }

        public override void SetFields()
        {
            this.Username   = Account.Username;
            this.Email      = Account.Email;
            this.LastName   = Account.LastName;
            this.FirstName  = Account.FirstName;
        }

        public bool Create()
        {
            bool usernameExists = db.AccountUsernameExists(Username) == DbError.EXISTS;
            bool emailExists = db.AccountEmailExists(Email) == DbError.SUCCESS;
            bool passwordsMatch = Password == PasswordVerify;


            if (!usernameExists && !emailExists && passwordsMatch)
            {
                return db.AddAccount(Account) == DbError.SUCCESS;
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            return db.UpdateAccount(Account) == DbError.SUCCESS;
        }

        public bool Login()
        {
            Account = db.GetAccount(Username);
            if (Account.AccountID != -1 && Account.Password == Password)
            {
                Account.LastLogin = DateTime.Now;
                db.UpdateAccount(Account);
                return true;
            }

            return false;
        }
    }
}