using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class VictoriousController : Controller
    {
        public bool UserLoggedIn()
        {
            if (Session["User.UserId"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getUserId()
        {
            int userId = -1;
            int.TryParse(Session["User.UserId"].ToString(), out userId);

            return userId;
        }
    }
}