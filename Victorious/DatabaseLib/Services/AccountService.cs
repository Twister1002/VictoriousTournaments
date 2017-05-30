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

        public void AddAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Add(account);
        }

        public AccountModel GetAccount(int accountId)
        {
            return unitOfWork.AccountRepo.Get(accountId);
        }

        public void DeleteAccount(int accountId)
        {
            unitOfWork.AccountRepo.Delete(accountId);
        }

        public void UpdateAccount(AccountModel account)
        {
            unitOfWork.AccountRepo.Update(account);
        }

        public List<AccountModel> GetAllAccounts()
        {
            return unitOfWork.AccountRepo.GetAll().ToList();
        }


        public void AddAccountInvite(AccountInviteModel accountInvite)
        {
            unitOfWork.AccountInviteRepo.Add(accountInvite);
          
        }

        public AccountInviteModel GetAcountInvite(string inviteCode)
        {
            return unitOfWork.AccountInviteRepo.GetAll().Single(x => x.AccountInviteCode == inviteCode);
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

    }
}
