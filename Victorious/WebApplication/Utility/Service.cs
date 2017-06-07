using DatabaseLib;
using DatabaseLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Utility
{
    public class Service
    {
        private IUnitOfWork work;
        public AccountService AccountService { get; private set; }
        public TournamentService TournamentService { get; private set; }
        public TypeService TypeService { get; private set; } 

        public Service(IUnitOfWork work)
        {
            this.work = work;
            AccountService = new AccountService(work);
            TournamentService = new TournamentService(work);
            TypeService = new TypeService(work);
        }

        public bool Save()
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