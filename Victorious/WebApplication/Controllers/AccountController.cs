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
            if (Session["User.UserId"] != null)
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
            if (Session["User.UserId"] != null)
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
                //    Session["User.UserId"] = user.UserID;
                //    return RedirectToAction("Index", "Account");
                //}
                //else
                //{
                //    // There was an error 
                //    return View();
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
            AccountRegisterViewModel model = new AccountRegisterViewModel();

            if (Session["User.UserId"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
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

                DbError userExists = db.UserUsernameExists(user.Username);
                DbError emailExists = db.UserEmailExists(user.Email);

                if (userExists == DbError.DOES_NOT_EXIST && emailExists == DbError.ERROR)
                {
                    // We can then register the user
                    if (db.AddUser(userModel) == DbError.SUCCESS)
                    {
                        // User Registraion was successful
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        // User Registration failed.
                        return View(user);
                    }
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
