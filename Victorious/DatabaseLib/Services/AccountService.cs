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
       

        public AccountService(IRepository<AccountModel> accountRepo, IUnitOfWork unitOfWork)
        {
            this.accountRepo = accountRepo;
            this.unitOfWork = unitOfWork;
        }

        public void AddAccount(AccountModel account)
        {
            accountRepo.Add(account);
        }

        public AccountModel GetAccount(int accountId)
        {
            return accountRepo.Get(accountId);
        }

        public void DeleteAccount(int accountId)
        {
            accountRepo.Delete(accountId);
        }

        public void UpdateAccount(AccountModel account)
        {
            accountRepo.Update(account);
        }

        




    }
}
