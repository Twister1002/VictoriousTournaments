using System;
using System.Collections.Generic;
using System.Linq;
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

        public DbError AddAccount(Account user)
        {
           
            try
            {
                user.CreatedOn = DateTime.Now;
                context.Accounts.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                //WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }


    }
}
