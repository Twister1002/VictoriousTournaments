using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IAccountRepository 
    {
        IList<AccountModel> GetAllAccounts();

        DbError AddAccount(AccountModel account);

        DbError LogAccountIn(int accountId);

        DbError UpdateAccount(AccountModel account);

        DbError DeleteAccount(AccountModel account);

        AccountModel GetAccount(int accountId);

        AccountModel GetAccount(string username);

        DbError AccountUsernameExists(string username);

        DbError AccountEmailExists(string email);

        List<TournamentModel> GetTournamentsForAccount(int accountId);


    }
}
