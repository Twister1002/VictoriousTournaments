using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseLib;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public enum TournamentStatus
    {
        ADMIN,
        ACTIVE,
        UPCOMING,
        PAST
    };

    public class AccountViewModel : ViewModel
    {
        public AccountModel Account { get; private set; }
        public Dictionary<TournamentStatus, List<TournamentModel>> Tournaments { get; private set; }

        public AccountViewModel(IUnitOfWork work) : base(work)
        {
            Account = new AccountModel();
            Init();
        }

        public AccountViewModel(IUnitOfWork work, int id) : base(work)
        {
            Account = services.AccountService.GetAccount(id);
            Init();
        }

        public AccountViewModel(IUnitOfWork work, AccountModel model) : base(work)
        {
            Account = model;
            Init();
        }

        public void Init()
        {
            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();

            SetFields();
            LoadAccountTournaments();
        }

        private void LoadAccountTournaments()
        {
            List<TournamentModel> tournaments = services.AccountService.GetTournamentsForAccount(Account.AccountID);

            // Sort the tournaments
            foreach (TournamentModel tournament in tournaments)
            {
                Permission userPermission = (Permission)tournament.TournamentUsers.Single(x => x.AccountID == Account.AccountID).PermissionLevel;
                // OWner of tournament
                if (userPermission == Permission.TOURNAMENT_CREATOR || userPermission == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    Tournaments[TournamentStatus.ADMIN].Add(tournament);
                }
                else
                {
                    // Active Tournament
                    if (tournament.TournamentStartDate <= DateTime.Now &&
                        tournament.TournamentEndDate > DateTime.Now)
                    {
                        Tournaments[TournamentStatus.ACTIVE].Add(tournament);
                    }
                    else if (tournament.TournamentStartDate > DateTime.Now)
                    {
                        Tournaments[TournamentStatus.UPCOMING].Add(tournament);
                    }
                    else
                    {
                        Tournaments[TournamentStatus.PAST].Add(tournament);
                    }
                }
            }
        }

        public void ApplyChanges()
        {
            // Non null fields
            Account.AccountID       = this.AccountId;
            Account.Username        = this.Username != String.Empty ? this.Username : String.Empty;
            Account.Email           = this.Email != String.Empty ? this.Email : String.Empty;
            Account.FirstName       = this.FirstName != String.Empty ? this.FirstName : String.Empty;
            Account.LastName        = this.LastName != String.Empty ? this.LastName : String.Empty;
            Account.Password        = this.Password != String.Empty ? this.Password : String.Empty;
        }
        
        public void SetFields()
        {
            this.AccountId  = Account.AccountID;
            this.Username   = Account.Username;
            this.Email      = Account.Email;
            this.LastName   = Account.LastName;
            this.FirstName  = Account.FirstName;
        }

        public void SetFields(AccountViewModel model)
        {
            this.AccountId  = model.AccountId;
            this.Username   = model.Username;
            this.Email      = model.Email;
            this.LastName   = model.LastName;
            this.FirstName  = model.FirstName;
            this.Password   = model.Password;
        }

        public bool Create()
        {
            ApplyChanges();
            Account.CreatedOn = DateTime.Now;
            Account.InviteCode = Codes.GenerateInviteCode();

            bool usernameExists = services.AccountService.AccountUsernameExists(Username) == DbError.EXISTS;
            bool emailExists = services.AccountService.AccountEmailExists(Email) == DbError.SUCCESS;
            bool passwordsMatch = Password == PasswordVerify;


            if (!usernameExists && !emailExists && passwordsMatch)
            {
                services.AccountService.AddAccount(Account);
                return services.Save();
            }
            else
            {
                return false;
            }
        }

        public bool Update(AccountViewModel updated)
        {
            if (Account.Password == updated.Password)
            {
                SetFields(updated);
                ApplyChanges();
                services.AccountService.UpdateAccount(Account);
                return services.Save();
            }
            else
            {
                return false;
            }
        }

        public bool Delete() { return false; }

        public bool Login()
        {
            Account = services.AccountService.GetAccount(Username);
            if (Account != null && Account.Password == Password)
            {
                Account.LastLogin = DateTime.Now;
                services.AccountService.UpdateAccount(Account);
                return services.Save();
            }
            
            return false;
        }

        public Permission SitePermission()
        {
            return (Permission)Account.PermissionLevel;
        }

        public bool IsAdministrator()
        {
            return SitePermission() == Permission.SITE_ADMINISTRATOR;
        }
    }
}