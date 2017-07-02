using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class Home : Model
    {
        public Home(IService service) : base(service)
        {

        }
    }
}