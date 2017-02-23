using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using DataLib;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        DatabaseInterface db = new DatabaseInterface();

        // GET: Account
        [Route("Account")]
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [Route("Account/Login")]
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
            
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(AccountLoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check the username and password
                //UserModel user = db.GetUserByUsername(viewModel.Username);
                //if (user.Password == viewModel.Password)
                //{
                //    db.LogUserInById(user.UserID);
                //}
            }
            else
            {
                ModelState.AddModelError("", "This is an invalid model");
            }

            return View(viewModel);
        }
        
        [Route("Account/Register")]
        public ActionResult Register()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
           
        }

        [HttpPost]
        [Route("Account/Register")]
        public ActionResult Register(AccountRegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                UserModel userModel = new UserModel();
                userModel.Username = user.Username;
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.Password = user.Password;

                bool userExists = db.UserUsernameExists(user.Username);
                bool emailExists = db.UserEmailExists(user.Email);


                // Check to see if the username exists
                
                //if (!db.UserExists(user.Username))
                if (!userExists && !emailExists)
                {
                    // We can then register the user
                    //db.AddUser(userModel);
                }

            }

            //If we hit this, then something failed 
            return View(user);
        }

        [Route("Account/Logout")]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
    }
}
