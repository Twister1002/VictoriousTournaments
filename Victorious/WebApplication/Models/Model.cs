using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public enum TournamentStatus
    {
        ADMIN,
        ACTIVE,
        UPCOMING,
        PAST
    }

    public abstract class Model
    {
        protected Service services;
        public String message;
        public Exception ex;

        public Model(IUnitOfWork work)
        {
            services = new Service(work);
        }
    }
}