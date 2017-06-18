using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Interfaces;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class Account : Model
    {
        public AccountViewModel viewModel { get; private set; }
        public AccountModel Model { get; private set; }
        public Dictionary<TournamentStatus, List<TournamentModel>> Tournaments { get; private set; }
        
        public Account(IService service, int id) : base(service)
        {
            Retreive(id);
            Init();
        }

        private void Init()
        {
            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();

            // ViewModel
            viewModel = new AccountViewModel();
            SetFields();

            LoadAccountTournaments();
        }

        public bool Login(AccountViewModel viewModel)
        {
            if (viewModel != null)
            {
                // Acquire the account in question
                Model = services.Account.GetAccount(viewModel.Username);
                if (Model != null)
                {
                    if (viewModel.Password == Model.Password)
                    {
                        Model.LastLogin = DateTime.Now;
                        services.Account.UpdateAccount(Model);
                        return services.Save();
                    }
                }
            }

            return false;
        }

        public bool IsLoggedIn()
        {
            if (Model.AccountID != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region CRUD
        /// <summary>
        /// Creates the user to the database
        /// </summary>
        /// <param name="viewModel">The form of the model that was used</param>
        /// <returns>True if successful save, false if not.</returns>
        public bool Create(AccountViewModel viewModel)
        {
            if (viewModel != null)
            {
                this.viewModel = viewModel;
                ApplyChanges();

                // Verify we can create the user
                Model.Salt =
                Model.CreatedOn = DateTime.Now;
                Model.InviteCode = Codes.GenerateInviteCode();

                bool usernameExists = services.Account.AccountEmailExists(viewModel.Username);
                bool emailExists = services.Account.AccountEmailExists(viewModel.Email);
                bool passwordsMatch = viewModel.Password == viewModel.PasswordVerify;


                if (!usernameExists && !emailExists && passwordsMatch)
                {
                    services.Account.AddAccount(Model);
                    return services.Save();
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        
        private void Retreive(int id)
        {
            Model = services.Account.GetAccount(id);

            if (Model == null)
            {
                Model = new AccountModel();
            }
        }

        public bool Update(AccountViewModel viewModel)
        {
            if (viewModel.Password == Model.Password)
            {
                this.viewModel = viewModel;
                ApplyChanges();
                services.Account.UpdateAccount(Model);
                return services.Save();
            }

            return false;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException("Delete is not allowed");
        }
        #endregion

        #region Helpers
        private void LoadAccountTournaments()
        {
            List<TournamentModel> tournaments = services.Account.GetTournamentsForAccount(Model.AccountID);

            // Sort the tournaments
            foreach (TournamentModel tournament in tournaments)
            {
                Permission userPermission = (Permission)tournament.TournamentUsers.Single(x => x.AccountID == Model.AccountID).PermissionLevel;
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

        public String GetUsername()
        {
            return Model.Username != null ? Model.Username : "Guest";
        }
        #endregion

        #region ViewModel
        public void ApplyChanges()
        {
            Model.AccountID   = viewModel.AccountId;
            Model.Username    = viewModel.Username != String.Empty ? viewModel.Username : String.Empty;
            Model.Email       = viewModel.Email != String.Empty ? viewModel.Email : String.Empty;
            Model.FirstName   = viewModel.FirstName != String.Empty ? viewModel.FirstName : String.Empty;
            Model.LastName    = viewModel.LastName != String.Empty ? viewModel.LastName : String.Empty;
            Model.Password    = viewModel.Password != String.Empty ? viewModel.Password : String.Empty;
        }

        public void SetFields()
        {
            viewModel.AccountId     = Model.AccountID;
            viewModel.Username      = Model.Username;
            viewModel.Email         = Model.Email;
            viewModel.LastName      = Model.LastName;
            viewModel.FirstName     = Model.FirstName;
        }
        #endregion

        #region Permissions
        public Permission AccountPermission()
        {
            return (Permission)Model.PermissionLevel;
        }

        public bool IsAdministrator()
        {
            return AccountPermission() == Permission.SITE_ADMINISTRATOR && IsLoggedIn();
        }
        #endregion
    }
}