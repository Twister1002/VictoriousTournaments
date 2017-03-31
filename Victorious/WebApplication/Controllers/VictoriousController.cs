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
        //protected UserModel userModel;
        //protected bool isUserLoggedin { get; }

        //public VictoriousController()
        //{
        //    if (Session != null && Session["User.UserId"] != null)
        //    {
        //        userModel = db.GetUserById((int)Session["User.UserId"]);
        //        if (userModel.UserID > 0)
        //        {
        //            isUserLoggedin = true;
        //            userModel.Password = null;
        //        }
        //        else
        //        {
        //            userModel = null;
        //            isUserLoggedin = false;
        //        }
        //    }
        //    else
        //    {
        //        userModel = null;
        //        isUserLoggedin = false;
        //    }
        //}

        //public int getUserId()
        //{
        //    return userModel.UserID;
        //}

        //public UserModel getUserModel()
        //{
        //    return userModel;
        //}

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