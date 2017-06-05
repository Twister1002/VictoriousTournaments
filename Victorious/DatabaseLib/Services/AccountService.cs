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


        public AccountService(IUnitOfWork unitOfWork)
        {
            //this.accountRepo = accountRepo;
            this.unitOfWork = unitOfWork;
        }

        #region Accounts

        public void AddAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Add(account);
        }

        public AccountModel GetAccount(int accountId)
        {
            try
            {
                return unitOfWork.AccountRepo.Get(accountId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public AccountModel GetAccount(string username)
        {
            try
            {
                return unitOfWork.AccountRepo.GetSingle(u => u.Username == username);
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public List<AccountModel> GetAllAccounts()
        {
            try
            {
                return unitOfWork.AccountRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void DeleteAccount(int accountId)
        {
            unitOfWork.AccountRepo.Delete(accountId);
        }

        public void UpdateAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Update(account);
        }

        public DbError AccountUsernameExists(string username)
        {
            try
            {
                AccountModel account = unitOfWork.AccountRepo.GetAll().Single(u => u.Username == username);
            }
            catch (Exception)
            {
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError AccountEmailExists(string email)
        {
            try
            {
                AccountModel account = unitOfWork.AccountRepo.GetSingle(e => e.Email == email);
            }
            catch (Exception)
            {
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

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
                return null;
                throw;
            }
            
            return tournaments;
        }


        #endregion


        #region AccountInvites 

        public void AddAccountInvite(AccountInviteModel accountInvite)
        {
            unitOfWork.AccountInviteRepo.Add(accountInvite);
          
        }

        public AccountInviteModel GetAccountInvite(string inviteCode)
        {
            try
            {
                return unitOfWork.AccountInviteRepo.GetSingle(x => x.AccountInviteCode == inviteCode);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateAccountInvite(AccountInviteModel accountInvite)
        {
            unitOfWork.AccountInviteRepo.Update(accountInvite);
        }

        public void DeleteAccountInvite(string inviteCode)
        {
            AccountInviteModel inviteToDelete = unitOfWork.AccountInviteRepo.GetSingle(x => x.AccountInviteCode == inviteCode);
            unitOfWork.AccountInviteRepo.DeleteEntity(inviteToDelete);
        }

        public List<AccountInviteModel> GetAllAccountInvites()
        {
            try
            {
                return unitOfWork.AccountInviteRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        #endregion
    }
}
