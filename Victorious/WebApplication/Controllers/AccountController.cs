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
                AccountViewModel model = new AccountViewModel(db.GetUserById((int)Session["User.UserId"]));
                return View("Index", model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [Route("Account/Login")]
        public ActionResult Login()
        {
            AccountLoginViewModel model = new AccountLoginViewModel();

            if (Session["User.UserId"] != null)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View("Login", model);
            }
            
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(AccountLoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check the username and password
                UserModel user = db.GetUserByUsername(viewModel.Username);
                
                if (user != null)
                {
                    if (user.Password == viewModel.Password)
                    {
                        Session["User.UserId"] = user.UserID;
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        // There was an error 
                        viewModel.error = ViewModel.ViewError.WARNING;
                        viewModel.ErrorMessage = "The password you provided for this user is invalid";
                    }
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.WARNING;
                    viewModel.ErrorMessage = "The username you provided doesn't exist.";
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.ErrorMessage = "Please enter in the required fields.";
            }

            return View("Login", viewModel);
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
        public ActionResult Register(AccountRegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UserModel userModel = new UserModel()
                {
                    Username = viewModel.Username,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Password = viewModel.Password
                };

                DbError userExists = db.UserUsernameExists(viewModel.Username);
                DbError emailExists = db.UserEmailExists(viewModel.Email);

                if (userExists == DbError.DOES_NOT_EXIST && emailExists == DbError.DOES_NOT_EXIST)
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
                        viewModel.error = ViewModel.ViewError.WARNING;
                        viewModel.ErrorMessage = "Well... Something went wrong when creating your account.";
                        return View(viewModel);
                    }
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.WARNING;
                    viewModel.ErrorMessage = "The username of email is all ready being used. Click <a href='/Account/Login/'>here</a> to login";
                    return View(viewModel);
                }
            }
            else
            {
                //If we hit this, then something failed 
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.ErrorMessage = "Please enter in the required fields.";
                return View(viewModel);
            }
        }

        [Route("Account/Logout")]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
    }
}
