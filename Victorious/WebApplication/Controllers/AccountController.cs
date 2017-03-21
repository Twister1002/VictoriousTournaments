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
    public class AccountController : VictoriousController
    {
        [Route("Account/Logout")]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

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
            AccountViewModel model = new AccountViewModel();
           
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
        public ActionResult Login(AccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check the username and password
                viewModel.setUserModel(viewModel.Username);
                
                if (viewModel.getUserModel() != null)
                {
                    if (viewModel.Password == viewModel.getUserModel().Password)
                    {
                        Session["User.UserId"] = viewModel.getUserModel().UserID;
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        // There was an error 
                        viewModel.error = ViewModel.ViewError.WARNING;
                        viewModel.message = "The password you provided for this user is invalid";
                    }
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.WARNING;
                    viewModel.message = "The username you provided doesn't exist.";
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.message = "Please enter in the required fields.";
            }

            return View("Login", viewModel);
        }
        
        [Route("Account/Register")]
        public ActionResult Register()
        {
            AccountViewModel model = new AccountViewModel();

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
        public ActionResult Register(AccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool passwordsMatch = viewModel.Password == viewModel.PasswordVerify;
                DbError userExists = db.UserUsernameExists(viewModel.Username);
                DbError emailExists = db.UserEmailExists(viewModel.Email);

                if (userExists == DbError.DOES_NOT_EXIST && emailExists == DbError.DOES_NOT_EXIST)
                {
                    // We can then register the user
                    viewModel.ApplyChanges();
                    UserModel userModel = viewModel.getUserModel();

                    DbError error = db.AddUser(ref userModel);
                    if (error == DbError.SUCCESS)
                    {
                        // User Registraion was successful
                        Session["Message"] = "Registration was successful. Please login to continue.";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        // User Registration failed.
                        viewModel.dbException = db.interfaceException;
                        viewModel.error = ViewModel.ViewError.CRITICAL;
                        viewModel.message = "Well... Something went wrong when creating your account: <br/>";
                        //+"<h2>Message</h2>" + db.e.Message + "<h2>Inner Exception</h2>" + db.e.InnerException;
                        return View(viewModel);
                    }
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.WARNING;
                    viewModel.message = "The username or email is all ready being used. Click <a class='clickable-underline' href='/Account/Login/'>here</a> to login";
                    return View(viewModel);
                }
            }
            else
            {
                //If we hit this, then something failed 
                viewModel.error = ViewModel.ViewError.EXCEPTION;
                viewModel.message = "Please enter in the required fields.";
                return View(viewModel);
            }
        }

        [Route("Account/Update")]
        public ActionResult Update()
        {
            if (Session["User.UserId"] != null)
            {
                AccountViewModel model = new AccountViewModel((int)Session["User.UserId"]);
                model.SetFields();

                return View("Update", model);
            }
            else
            {
                Session["Message"] = "You need you login to update your account.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [Route("Account/Update")]
        public ActionResult Update(AccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["User.UserId"] != null)
                {
                    viewModel.setUserModel((int)Session["User.UserId"]);

                    // Verify the user being updated is legitly the user logged in
                    if (viewModel.getUserModel().UserID == (int)Session["User.UserId"])
                    {
                        // Apply the changes
                        viewModel.ApplyChanges();

                        DbError error = db.UpdateUser(viewModel.getUserModel());
                        if (error == DbError.SUCCESS)
                        {
                            viewModel.error = ViewModel.ViewError.SUCCESS;
                            viewModel.message = "Your account was successfully updated.";
                            Session["Message"] = viewModel.message;
                            Session["Message.Class"] = viewModel.error;
                            return RedirectToAction("Index", "Account");
                        }
                        else
                        {
                            // There was an error updating the account
                            viewModel.dbException = db.interfaceException;
                            viewModel.error = ViewModel.ViewError.CRITICAL;
                            viewModel.message = "There was an error updating your account. Please try again later.";
                        }
                    }
                    else
                    {
                        // Log the user out as I feel this is a hacking attempt
                        Session.RemoveAll();
                        Session["Message"] = "We couldn't validate who you are. Please login and try again.";
                        Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    Session["Message"] = "Please login to edit your account information";
                    Session["Message.Class"] = ViewModel.ViewError.WARNING;
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.message = "Please enter in the fields fully and correctly.";
            }

            return View("Update", viewModel);
        }
    }
}
