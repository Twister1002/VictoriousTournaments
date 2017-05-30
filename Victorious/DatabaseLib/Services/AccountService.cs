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
            return unitOfWork.AccountRepo.Get(accountId);
        }

        public AccountModel GetAccount(string username)
        {
            return unitOfWork.AccountRepo.GetSingle(u => u.Username == username);
        }

        public List<AccountModel> GetAllAccounts()
        {
            return unitOfWork.AccountRepo.GetAll().ToList();
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
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
            catch (Exception ex)
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
            AccountInviteModel inviteToDelete = unitOfWork.AccountInviteRepo.GetAll().Single(x => x.AccountInviteCode == inviteCode);
            unitOfWork.AccountInviteRepo.Delete(inviteToDelete.AccountInviteID);
        }

        public List<AccountInviteModel> GetAllAccountInvites()
        {
            return unitOfWork.AccountInviteRepo.GetAll().ToList();
        }

        #endregion
    }
}
