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

            if (UserLoggedIn())
            {
                int.TryParse(Session["User.UserId"].ToString(), out userId);
            }

            return userId;
        }

        public UserModel getUserModel()
        {
            UserModel user = db.GetUserById(getUserId());

            return user;
        }
    }
}