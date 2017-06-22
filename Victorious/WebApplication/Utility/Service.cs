using DatabaseLib;
using DatabaseLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;

namespace WebApplication.Utility
{
    public class Service : IService
    {
        public Exception e { get; set; }
        private IUnitOfWork work;
        public AccountService Account { get; private set; }
        public TournamentService Tournament { get; private set; }
        public TypeService Type { get; private set; } 

        public Service(IUnitOfWork work)
        {
            this.work = work;
            Account = new AccountService(work);
            Tournament = new TournamentService(work);
            Type = new TypeService(work);
        }

        public bool Save()
        {
            if (work.Save())
            {
                return true;
            }
            else
            {
                //e = work.getException();
                work.Refresh();
                return false;
            }
        }

        public Exception GetException()
        {
            return e;
        }
    }
}