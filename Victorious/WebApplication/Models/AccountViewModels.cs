using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseLib;
using DatabaseLib.Services;
using WebApplication.Utility;

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

        public AccountViewModel() : base()
        {
            Account = new AccountModel();
        }

        public AccountViewModel(int id) : base()
        {
            Account = accountService.GetAccount(id);
            LoadAccountTournaments();
            SetFields();
        }

        public AccountViewModel(AccountModel model) : base()
        {
            Account = model;
            LoadAccountTournaments();
            SetFields();
        }

        protected override void Init()
        {
            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();
        }

        private void LoadAccountTournaments()
        {
            List<TournamentModel> tournaments = accountService.GetTournamentsForAccount(Account.AccountID);

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

        public override void ApplyChanges()
        {
            // Non null fields
            Account.AccountID       = this.AccountId;
            Account.Username        = this.Username != String.Empty ? this.Username : String.Empty;
            Account.Email           = this.Email != String.Empty ? this.Email : String.Empty;
            Account.FirstName       = this.FirstName != String.Empty ? this.FirstName : String.Empty;
            Account.LastName        = this.LastName != String.Empty ? this.LastName : String.Empty;
            Account.Password        = this.Password != String.Empty ? this.Password : String.Empty;
        }
        
        public override void SetFields()
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

            bool usernameExists = accountService.AccountUsernameExists(Username) == DbError.EXISTS;
            bool emailExists = accountService.AccountEmailExists(Email) == DbError.SUCCESS;
            bool passwordsMatch = Password == PasswordVerify;


            if (!usernameExists && !emailExists && passwordsMatch)
            {
                accountService.AddAccount(Account);
                work.Save();
                return true;
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
                accountService.UpdateAccount(Account);
                return work.Save();
            }
            else
            {
                return false;
            }
        }

        public bool Login()
        {
            try
            {
                Account = accountService.GetAccount(Username);
                if (Account != null && Account.Password == Password)
                {
                    Account.LastLogin = DateTime.Now;
                    accountService.UpdateAccount(Account);
                    work.Save();
                    return true;
                }
            }
            catch (Exception e)
            {
                dbException = e;
                work.Refresh();
                return false;
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