using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Interfaces;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class Account : Model, IViewModel<AccountViewModel>
    {
        private HashManager hasher;
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
            hasher = new HashManager();

            Tournaments = new Dictionary<TournamentStatus, List<TournamentModel>>();
            Tournaments[TournamentStatus.ADMIN] = new List<TournamentModel>();
            Tournaments[TournamentStatus.ACTIVE] = new List<TournamentModel>();
            Tournaments[TournamentStatus.UPCOMING] = new List<TournamentModel>();
            Tournaments[TournamentStatus.PAST] = new List<TournamentModel>();

            // ViewModel
            viewModel = new AccountViewModel();
            SetupViewModel();
            SetFields();

            LoadAccountTournaments();
        }

        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="viewModel">The model that was submitted by a form</param>
        /// <returns>True if user is authenticated</returns>
        public bool Login(AccountViewModel viewModel)
        {
            if (viewModel != null)
            {
                // Acquire the account in question
                if (!String.IsNullOrEmpty(viewModel.Username))
                {
                    Model = services.Account.GetAccount(viewModel.Username);
                }
                else
                {
                    Model = null;
                }

                if (Model != null)
                {
                    if (HashManager.ValidatePassword(viewModel.Password, Model.Password))
                    {
                        Model.LastLogin = DateTime.Now;
                        services.Account.UpdateAccount(Model);

                        viewModel.errorType = ViewError.NONE;
                        viewModel.message = String.Empty;

                        return services.Save();
                    }
                    else
                    {
                        viewModel.message = "Username or password is incorrect.";
                        viewModel.errorType = ViewError.ERROR;
                    }
                }
                else
                {
                    viewModel.message = "Username or password is incorrect.";
                    viewModel.errorType = ViewError.ERROR;
                }

                if (Model == null && viewModel.ProviderID != 0 && !String.IsNullOrEmpty(viewModel.SocialID))
                {
                    // This must be a social login. Lets find it.
                    AccountSocialModel socialModel = services.Account.GetAccountSocialProvider(viewModel.SocialID, viewModel.ProviderID);
                    if (socialModel != null)
                    {
                        // Grab the accountID referenced and load the model and return true. 
                        // No need to validate the password as they were verified through social media

                        Model = socialModel.Account;

                        viewModel.errorType = ViewError.NONE;
                        viewModel.message = String.Empty;

                        Model.LastLogin = DateTime.Now;
                        services.Account.UpdateAccount(Model);

                        return services.Save();
                    }
                    else
                    {
                        // We could register the person...
                        viewModel.message = "There is no account linked with this social media profile.";
                        viewModel.errorType = ViewError.ERROR;
                    }
                }
                else
                {
                    viewModel.message = "There has been an unexpected error. Please try again.";
                    viewModel.errorType = ViewError.ERROR;
                }
            }

            return false;
        }

        /// <summary>
        /// Determins if user is logged in
        /// </summary>
        /// <returns>True if user is logged in</returns>
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

        public void ForgotPassword(AccountViewModel viewModel)
        {

        }

        public void ForgotUsername(AccountViewModel viewModel)
        {

        }

        public bool ModifySocialAccount(bool addSocial, int provider, Dictionary<String, String> socialInfo)
        {
            if (addSocial)
            {
                // Verify that the account hasn't been added to another account.
                AccountSocialModel socialModel = services.Account.GetAccountSocialProvider(socialInfo["id"], provider);

                if (socialModel == null)
                {
                    socialModel = new AccountSocialModel()
                    {
                        AccountID = Model.AccountID,
                        ProviderID = provider,
                        SocialID = socialInfo["id"],
                        Email = Model.Email
                    };
                    services.Account.AddAccountSocialProvider(socialModel);
                }
                else
                {
                    viewModel.message = "The current social account is all ready linked to another account.";
                    viewModel.errorType = ViewError.WARNING;
                    return false;
                }
            }
            else
            {
                if (Model.AccountSocials.Count == 1 && String.IsNullOrEmpty(Model.Password))
                {
                    viewModel.message = "To disconnect your social account, you must first create a password.";
                    viewModel.errorType = ViewError.ERROR;
                    return false;
                }
                else
                {
                    services.Account.DeleteAccountSocialProider(Model.AccountSocials.Single(x => x.AccountID == Model.AccountID && x.ProviderID == provider));
                }
                
            }

            if (services.Save())
            {
                if (addSocial)
                {
                    viewModel.message = "Your social account has been linked.";
                }
                else
                {
                    viewModel.message = "Your social account has been unlinked.";
                }

                viewModel.errorType = ViewError.SUCCESS;
                return true;
            }
            else
            {
                viewModel.message = "There was an error in unlinking your account.";
                viewModel.errorType = ViewError.ERROR;
                return false;
            }
        }

        public Dictionary<String, String> GetSocialAcountInfo(String token, AccountSocialModel.SocialProviders provider)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            switch (provider)
            {
                case AccountSocialModel.SocialProviders.FACEBOOK:
                    services.FBClient.AccessToken = token;
                    data = services.FBClient.Get<Dictionary<String, String>>("/me", new { fields = "id,email,first_name,last_name" });
                    break;
            }

            return data;
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
                bool usernameExists = false;
                bool emailExists = false;
                bool passwordsMatch = false;
                Dictionary<String, String> socialData = null;

                if (!String.IsNullOrEmpty(viewModel.AccessToken))
                {
                    switch (viewModel.ProviderID)
                    {
                        case (int)AccountSocialModel.SocialProviders.FACEBOOK:
                            // We need to grab the email
                            socialData = GetSocialAcountInfo(viewModel.AccessToken, AccountSocialModel.SocialProviders.FACEBOOK);

                            viewModel.Email = socialData["email"];
                            Model.FirstName = socialData["first_name"];
                            Model.LastName = socialData["last_name"];
                            passwordsMatch = true;
                            break;
                    }
                }
                else
                {
                    usernameExists = services.Account.AccountUsernameExists(viewModel.Username);
                    passwordsMatch = viewModel.Password == viewModel.PasswordVerify && IsPasswordValid(viewModel.Password);
                }
        
                // We need to check for email address regardless of any process.
                emailExists = services.Account.AccountEmailExists(viewModel.Email);
                
                // Verify we can create the user
                if (!usernameExists && !emailExists && passwordsMatch)
                {
                    ApplyChanges(viewModel);
                    Model.CreatedOn = DateTime.Now;
                    Model.InviteCode = Codes.GenerateInviteCode();

                    services.Account.AddAccount(Model);
                    if (services.Save())
                    {
                        if (socialData != null && viewModel.ProviderID != 0 && !String.IsNullOrEmpty(viewModel.AccessToken))
                        {
                            ModifySocialAccount(true, viewModel.ProviderID, socialData);
                        }
                        return true;
                    }
                    else
                    {
                        viewModel.message = "We were unable to register your account due to an error. Please try again.";
                    }
                }
                else
                {
                    if (emailExists)
                    {
                        viewModel.message = "An account with this email all ready exists. Please login instead.";
                    }
                    else if (!passwordsMatch)
                    {
                        viewModel.message = "The password you provided was not valid or the same";
                    }
                    else
                    {
                        viewModel.message = "We were unable to register your account. Please try again.";
                    }
                }
            }

            viewModel.errorType = ViewError.ERROR;
            viewModel.Providers = this.viewModel.Providers;
            viewModel.ProviderID = 0;
            viewModel.Email = null;
            viewModel.AccessToken = null;
            viewModel.SocialID = null;

            return false;
        }
        
        /// <summary>
        /// Acquires the Model of the user in question
        /// </summary>
        /// <param name="id">ID of the account</param>
        private void Retreive(int id)
        {
            Model = services.Account.GetAccount(id);

            if (Model == null)
            {
                Model = new AccountModel();
            }
        }

        /// <summary>
        /// Updates the user with the model provided by a form
        /// </summary>
        /// <param name="viewModel">The model of the form</param>
        /// <returns>True if updated; false if not.</returns>
        public bool Update(AccountViewModel viewModel)
        {
            if (!String.IsNullOrEmpty(Model.Password))
            {
                if (!String.IsNullOrEmpty(viewModel.CurrentPassword))
                {
                    if (!HashManager.ValidatePassword(viewModel.CurrentPassword, Model.Password) || viewModel.Password != viewModel.PasswordVerify)
                    {
                        viewModel.message = "Your new password did not correctly validate.";
                        viewModel.errorType = ViewError.WARNING;
                        return false;
                    }
                    else
                    {
                        ApplyChanges(viewModel);
                        services.Account.UpdateAccount(Model);
                    }
                }
                else
                {
                    viewModel.message = "To update your account, you need to provide your current password.";
                    viewModel.errorType = ViewError.WARNING;
                    return false;
                }
            }
            else
            {
                // This is to assume the user created the account via Social media.
                ApplyChanges(viewModel);
                services.Account.UpdateAccount(Model);
            }

            if (services.Save())
            {
                if (String.IsNullOrEmpty(viewModel.message))
                {
                    viewModel.message = "Your account was updated successfully.";
                    viewModel.errorType = ViewError.SUCCESS;
                }

                return true;
            }
            else
            {
                viewModel.message = "Your account was no updated due to an error. Please try again.";
                viewModel.errorType = ViewError.ERROR;
                return false;
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

        /// <summary>
        /// Gets the string of the username for the user.
        /// </summary>
        /// <returns></returns>
        public String GetUsername()
        {
            return Model.Username != null ? Model.Username : "Guest";
        }

        /// <summary>
        /// Creates rules that the password must possess in order to change passwords.
        /// </summary>
        /// <param name="pass">The password to validate</param>
        /// <returns>True or false if it meets the requirements</returns>
        public bool IsPasswordValid(String pass)
        {
            if (String.IsNullOrEmpty(pass) || pass.Length < 6)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region ViewModel
        public void SetupViewModel()
        {
            viewModel.Providers = services.Type.SocialProviders();
            viewModel.LinkedProviders = Model.AccountSocials.ToList();
        }
        
        public void ApplyChanges(AccountViewModel viewModel)
        {
            Model.Email = viewModel.Email != String.Empty ? viewModel.Email : String.Empty;
            Model.FirstName = viewModel.FirstName != String.Empty ? viewModel.FirstName : String.Empty;
            Model.LastName = viewModel.LastName != String.Empty ? viewModel.LastName : String.Empty;

            if (!String.IsNullOrEmpty(viewModel.Password))
            {
                // Check to see if we can cange the password
                if (IsPasswordValid(viewModel.Password) && viewModel.Password == viewModel.PasswordVerify)
                {
                    Model.Salt = HashManager.GetSalt();
                    Model.Password = HashManager.HashPassword(viewModel.Password, Model.Salt);
                }
                else
                {
                    viewModel.message = "Your password doesn't meet the minimum requirements.";
                    viewModel.errorType = ViewError.WARNING;
                }
            }
        }

        public void SetFields()
        {
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