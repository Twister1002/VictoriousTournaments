using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;

namespace WebApplication.Models
{
    public class Home : Model
    {
        public Home(IUnitOfWork work) : base(work)
        {
        }
    }
}