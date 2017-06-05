using System;
using DatabaseLib;
using DatabaseLib.Services;

namespace WebApplication.Models
{
    public abstract class ViewModel
    {
        public enum ViewError
        {
            NONE,
            SUCCESS,
            WARNING,
            EXCEPTION,
            CRITICAL
        }

        public String[] errorClassNames = new String[] {
            "none",
            "success",
            "warning",
            "exception",
            "critical"
        };
        
        public ViewError error = ViewError.NONE;
        protected AccountService accountService;
        protected TournamentService tournamentService;
        protected TypeService typeService;

#if DEBUG
        protected IUnitOfWork work = new UnitOfWork();
        //protected DatabaseRepository db = new DatabaseRepository("Debug");
#elif !DEBUG
        protected DatabaseRepository db = new DatabaseRepository("Production");
#endif
        protected ViewModel()
        {
            accountService = new AccountService(work);
            tournamentService = new TournamentService(work);
            typeService = new TypeService(work);

            Init();
        }

        protected abstract void Init();
        public String message { get; set; }
        public Exception dbException { get; set; }

        protected bool Save()
        {
            if (work.Save())
            {
                return true;
            }
            else
            {
                work.Refresh();
                return false;
            }
        }
    }
}