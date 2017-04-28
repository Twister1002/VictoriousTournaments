using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public enum Permission
    {
        // 0 = None
        NONE = 0,
        // 1 = Site Permissions
        SITE_ADMINISTRATOR = 1, SITE_STANDARD,
        // 100 = Tournament Permissions
        TOURNAMENT_ADMINISTRATOR = 100, TOURNAMENT_STANDARD,
        // 200 = Team Permissions
        TEAM_CAPTAIN = 200, TEAM_STANDARD
    }

    public enum DbError
    {
        ERROR = -1, NONE = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_REMOVE, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS, CONCURRENCY_ERROR
    };

    public class DbInterface
    {
        VictoriousEntities context = new VictoriousEntities();

        public Exception interfaceException;

        public DbInterface()
        {
           
        }

        #region Accounts

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
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError AccountExists(AccountModel account)
        {
            AccountModel _account;
            try
            {
                _account = context.AccountModels.Find(account.AccountID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                throw;
            }
            if (account == null)
                return DbError.DOES_NOT_EXIST;
            else
                return DbError.EXISTS;
        }

        // Updates LastLogin of passed-in account.
        public DbError LogAccountIn(AccountModel account)
        {
            try
            {
                account.LastLogin = DateTime.Now;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
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
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
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
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public AccountModel GetAccountById(int id)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Find(id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                account.AccountID = -1;
            }
            return account;
        }

        public AccountModel GetAccountByUsername(string username)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Single(x => x.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                account.AccountID = -1;
                return account;
            }
            return account;
        }

        public List<AccountModel> GetAllAccountModels()
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
                interfaceException = ex;
                accountModels.Clear();
                WriteException(ex);
                accountModels.Add(new AccountModel() { AccountID = -1 });
            }
            return accountModels;
        }

        public DbError AccountUsernameExists(string username)
        {
            try
            {
                AccountModel account = context.AccountModels.Single(u => u.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        #endregion


        #region Tournaments

        public DbError TournamentExists(TournamentModel tournament)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(tournament.TournamentID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public List<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                tournaments = context.TournamentModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
            }
            return tournaments;
        }

        public DbError AddTournament(TournamentModel tournament)
        {
           
            TournamentModel _tournament = new TournamentModel();
            try
            {
                _tournament = tournament;
             
                _tournament.CreatedOn = DateTime.Now;
                _tournament.LastEditedOn = DateTime.Now;
                context.SaveChanges();
                context.TournamentModels.Add(_tournament);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

       

        public TournamentModel GetTournamentById(int id)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.TournamentModels.Single(t => t.TournamentID == id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournament.TournamentID = -1;
            }
            return tournament;
        }

        public List<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            List<TournamentUserModel> list = new List<TournamentUserModel>();

            try
            {
                list = context.TournamentModels.Find(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception)
            {
                list.Clear();
                list.Add(new TournamentUserModel() { TournamentUserID = 0 });
                return list;
            }

            return list;
        }

        public DbError UpdateTournament(TournamentModel tournament)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(tournament.TournamentID);
                context.Entry(_tournament).CurrentValues.SetValues(tournament);
            
                //foreach (BracketModel bracket in tournament.Brackets)
                //{
                //    UpdateBracket(bracket);
                //}
                //context.Entry(_tournament.BracketModels).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ce)
            {
                interfaceException = ce;
                WriteException(ce);
                return DbError.CONCURRENCY_ERROR;

            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(int tournamentId)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(tournamentId);
                
                //foreach (var bracket in _tournament.Brackets.ToList())
                //{
                //    DeleteBracket(bracket);
                //}
                //foreach (var user in _tournament.TournamentUsers.ToList())
                //{
                //    RemoveUserFromTournament(_tournament, user);
                //}
                context.TournamentModels.Remove(_tournament);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

      

        /// <summary>
        /// Takes in a Dictionary of strings in which the key is the name of the parameter being searched for
        /// and the value is the value of that parameter.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();

            try
            {
                List<SqlParameter> sqlparams = new List<SqlParameter>();
                string query = string.Empty;
                //string tournamentIdQuery = "SELECT TournamentID FROM dbo.TournamentModels AS Tournament  ";
                //string rulesIdQuery = "SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";

                //string[] queries = new string[] { "Tournament.Title = @Title", "Tournament.StartDate = @StartDate" };  
                if (searchParams.ContainsKey("Title"))
                {
                    sqlparams.Add(new SqlParameter("@Title", searchParams["Title"]));
                    query = "SELECT TournamentID FROM TournamentModels WHERE Title = @Title";
                }
                if (searchParams.ContainsKey("StartDate"))
                {
                    sqlparams.Add(new SqlParameter("@StartDate", DateTime.Parse(searchParams["TournamentStartDate"])));
                    if (searchParams.ContainsKey("Title"))
                        query += "UNION SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";
                    else
                        query = "SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";
                }
                tournaments = context.TournamentModels.SqlQuery(query, sqlparams).ToList();
                //if (searchParams.ContainsKey("GameTypeID"))
                //{
                //    sqlparams.Add(new SqlParameter("@Game", searchParams["Game"]));
                //    foreach (var tournament in tournaments)
                //    {
                //        if (tournament)
                //    }
                //}




                //List < TournamentModel > _tournaments = context.TournamentModels.SqlQuery("SELECT * FROM dbo.TournamentModels WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();

            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
            }
            return tournaments;

        }

        public List<TournamentModel> FindTournaments(string title, DateTime startDate)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            //TournamentRulesModel rules = tournament.TournamentRule;
            try
            {
                List<TournamentModel> _tournaments = context.TournamentModels.SqlQuery("SELECT * FROM dbo.TournamentModels WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();
                foreach (var _tournament in _tournaments)
                {
                    if (_tournament.TournamentStartDate == startDate)
                        tournaments.Add(_tournament);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
                return tournaments;
            }
            return tournaments;
        }


        #endregion


        #region TournamentUsers

        public DbError AddTournamentUser(TournamentUserModel user)
        {
            try
            {
                context.TournamentUserModels.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
                throw;
            }
            return DbError.SUCCESS;
        }

        public TournamentUserModel GetTournamentUserById(int id)
        {
            TournamentUserModel user = new TournamentUserModel();
            try
            {
                user = context.TournamentUserModels.Find(id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                user.TournamentUserID = -1;
            }
            return user;
        }

        public DbError DeleteTournamentUser(int id)
        {
            try
            {
                TournamentUserModel _user = context.TournamentUserModels.Find(id);
                context.TournamentUserModels.Remove(_user);
                //context.TournamentModels.Include(x => x.).Single(x => x.TournamentID == tournament.TournamentID).Users.Remove(_user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_REMOVE;
            }
            return DbError.SUCCESS;
        }

        #endregion



        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }
    }
}
