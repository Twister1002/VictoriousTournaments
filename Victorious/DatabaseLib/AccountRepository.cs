using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class AccountRepository : IAccountRepository
    {
        private VictoriousEntities context;
        public Exception exception;

        public AccountRepository()
        {
            context = new VictoriousEntities();

        }

        public AccountRepository(VictoriousEntities context)
        {
            this.context = context;
        }

        public DbError AccountEmailExists(string email)
        {
            try
            {
                AccountModel user = context.AccountModels.Single(u => u.Email == email);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError AccountUsernameExists(string username)
        {
            try
            {
                AccountModel account = context.AccountModels.Single(u => u.Username == username);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError AddAccount(AccountModel account)
        {
            try
            {
                account.CreatedOn = DateTime.Now;
                context.AccountModels.Add(account);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteAccount(AccountModel account)
        {
            AccountModel _account = context.AccountModels.Find(account.AccountID);
            try
            {
                context.AccountModels.Remove(_account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public AccountModel GetAccount(string username)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Single(x => x.Username == username);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                account = null;
            }
            return account;
        }

        public AccountModel GetAccount(int accountId)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Find(accountId);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                account = null;
            }
            return account;
        }

        public IList<AccountModel> GetAllAccounts()
        {
            List<AccountModel> accountModels = new List<AccountModel>();
            try
            {
                foreach (var accont in context.AccountModels)
                {
                    accountModels.Add(accont);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                accountModels.Clear();
                WriteException(ex);
            }
            return accountModels;
        }

        public List<TournamentModel> GetTournamentsForAccount(int accountId)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                List<TournamentUserModel> users = new List<TournamentUserModel>();
                users = context.TournamentUserModels.Where(x => x.AccountID == accountId).ToList();
                foreach (var user in users)
                {
                    tournaments.Add(user.Tournament);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        public DbError LogAccountIn(int accountId)
        {
            try
            {

                AccountModel account = context.AccountModels.Find(accountId);
                account.LastLogin = DateTime.Now;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError UpdateAccount(AccountModel account)
        {
            try
            {
                AccountModel _account = context.AccountModels.Find(account.AccountID);
                context.Entry(_account).CurrentValues.SetValues(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AccountRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }
    }
}
