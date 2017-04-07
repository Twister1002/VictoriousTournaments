using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLib;

namespace WebApplication.Controllers
{
    public class VictoriousController : Controller
    {
        protected DatabaseInterface db = new DatabaseInterface();

        public UserModel getUserModel(int id)
        {
            return db.GetUserById(id);
        }

        public int ConvertToInt(String x)
        {
            int i = -1;
            int.TryParse(x, out i);
            return i;
        }
    }
}