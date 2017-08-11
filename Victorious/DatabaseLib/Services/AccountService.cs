using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Services
{
    public class AccountService
    {
        IUnitOfWork unitOfWork;
        //EmailService emailService;

        public AccountService(IUnitOfWork unitOfWork)
        {
            //this.accountRepo = accountRepo;
            this.unitOfWork = unitOfWork;
        }

        #region Accounts

        /// <summary>
        /// Adds a single Account to the database.
        /// </summary>
        /// <param name="account"> The model of the Account to add. </param>
        public void AddAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Add(account);
        }

        /// <summary>
        /// Retreives a single Account from the database.
        /// </summary>
        /// <param name="accountId"> The Id of the Account to retreive. </param>
        /// <returns> Returns an AccountModel, or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, an ObjectNotFoundException will be thrown. </remarks>
        public AccountModel GetAccount(int accountId)
        {
            try
            {
                return unitOfWork.AccountRepo.Get(accountId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a single Account from the database. 
        /// This function is designed to be used when the user is logging in to check their account info.
        /// </summary>
        /// <param name="userOrEmail">The username or the email associated with the account</param>
        /// <returns> Returns an AccountModel, or null if an exception is thrown. </returns>
        /// <remarks> If an Account with a matching username is not found, an ObjectNotFoundException will be thrown. </remarks>
        public AccountModel GetAccount(string userOrEmail)
        {
            try
            {
                return unitOfWork.AccountRepo.GetSingle(u => u.Username == userOrEmail || u.Email == userOrEmail);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all Accounts in the database.
        /// </summary>
        /// <returns> Returns a List of AccountModels. </returns>
        public List<AccountModel> GetAllAccounts()
        {
            try
            {
                return unitOfWork.AccountRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<AccountModel>();
            }
        }

        /// <summary>
        /// Deletes an account from the database. 
        /// </summary>
        /// <param name="accountId"> The Id of the Account to delete. </param>
        public void DeleteAccount(int accountId)
        {
            unitOfWork.AccountRepo.Delete(accountId);
        }

        /// <summary>
        /// Updates a single Account in the database.
        /// </summary>
        /// <param name="account"> The model of the Account to update. </param>
        public void UpdateAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Update(account);
        }

        /// <summary>
        /// Checks to see if the a username already exists.
        /// </summary>
        /// <param name="username"> The username to check. </param>
        /// <returns> Returns true if it exists, else returns false. </returns>
        public bool AccountUsernameExists(string username)
        {
            try
            {
                AccountModel account = unitOfWork.AccountRepo.GetAll().Single(u => u.Username == username);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks to see if an email has already been used for an account.
        /// </summary>
        /// <param name="email"> The email to checked. </param>
        /// <returns> Returns true if the email has been used, else returns false. </returns>
        public bool AccountEmailExists(string email)
        {
            try
            {
                AccountModel account = unitOfWork.AccountRepo.GetSingle(e => e.Email == email);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets all Tournaments that a user has entered.
        /// </summary>
        /// <param name="accountId"> Id of the Account to delete. </param>
        /// <returns></returns>
        public List<TournamentModel> GetTournamentsForAccount(int accountId)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            List<TournamentUserModel> users = new List<TournamentUserModel>();
            try
            {
                users = unitOfWork.TournamentUserRepo.GetAll().Where(x => x.AccountID == accountId).ToList();
                foreach (var user in users)
                {
                    tournaments.Add(user.Tournament);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentModel>();
            }
            
            return tournaments;
        }


        #endregion

        #region AccountInvites 

        /// <summary>
        /// Adds a single AccountInvite to the database.
        /// </summary>
        /// <param name="accountInvite"> The model of the AccountInvite to add. </param>
        public void AddAccountInvite(AccountInviteModel accountInvite)
        {
            unitOfWork.AccountInviteRepo.Add(accountInvite);
          
        }

        /// <summary>
        /// Retreives a single AccountInvite from the database.
        /// </summary>
        /// <param name="inviteCode"> The invite code of the AccountInvite. </param>
        /// <returns> Returns an AccountInviteModel, or null if an exception is thrown. </returns>
        /// <remarks> If an AccountInvite with a matching invite code could not be found, an ObjectNotFoundException will be thrown. </remarks>
        public AccountInviteModel GetAccountInvite(string inviteCode)
        {
            try
            {
                return unitOfWork.AccountInviteRepo.GetSingle(x => x.AccountInviteCode == inviteCode);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all AccountInvites from the database.
        /// </summary>
        /// <returns> Returns a List of AccountInviteModels. </returns>
        public List<AccountInviteModel> GetAllAccountInvites()
        {
            try
            {
                return unitOfWork.AccountInviteRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<AccountInviteModel>();
            }
        }

        /// <summary>
        /// Updates a single AccountInvite in the database.
        /// </summary>
        /// <param name="accountInvite"> The model of the AccountInvite to update. </param>
        public void UpdateAccountInvite(AccountInviteModel accountInvite)
        {
            unitOfWork.AccountInviteRepo.Update(accountInvite);
        }

        /// <summary>
        /// Deletes a single AccountInvite from the database. 
        /// </summary>
        /// <param name="inviteCode"> The inviteCode of the AcccountInvite to delete. </param>
        public void DeleteAccountInvite(string inviteCode)
        {
            AccountInviteModel inviteToDelete = unitOfWork.AccountInviteRepo.GetSingle(x => x.AccountInviteCode == inviteCode);
            unitOfWork.AccountInviteRepo.DeleteEntity(inviteToDelete);
        }

        #endregion

        #region AccountForget
        public void AddAccountForget(AccountForgetModel model)
        {
            unitOfWork.AccountForgetRepo.Add(model);
        }
        
        public AccountForgetModel Get(String token)
        {
            try
            {
                return unitOfWork.AccountForgetRepo.GetSingle(x => x.Token == token);
            }
            catch(Exception e)
            {
                unitOfWork.SetException(e);
                return null;
            }
        }

        public void UpdateAccountForget(AccountForgetModel model)
        {
            unitOfWork.AccountForgetRepo.Update(model);
        }

        public void DeleteAccountForget(int accountForgetID)
        {
            AccountForgetModel accountForget = unitOfWork.AccountForgetRepo.GetSingle(x => x.AccountForgetID == accountForgetID);
            unitOfWork.AccountForgetRepo.DeleteEntity(accountForget);
        }
        #endregion
    }
}
