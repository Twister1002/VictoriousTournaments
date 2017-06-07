﻿using System.Web.Mvc;
using WebApplication.Models;

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
        [Route("Account/Index")]
        public ActionResult Index()
        {
#if DEBUG
            // Since this is debug, save me the time and just log me in
            if (!IsLoggedIn()) Session["User.UserId"] = 1;
#endif
            if (IsLoggedIn())
            {
                return View("Index", account);
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
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View("Login", account);
            }
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.error = ViewModel.ViewError.ERROR;
                viewModel.message = "Please enter in the required fields.";
            }
            else
            {
                if (viewModel.Login())
                {
                    Session["User.UserId"] = viewModel.Account.AccountID;
                    Session["User.Name"] = viewModel.Account.FirstName;
                }
                else
                {
                    Session["Message"] = "The username or password is invalid.";
                    Session["Message.Class"] = ViewModel.ViewError.WARNING; 
                }
            }

            return RedirectToAction("Index", "Account");
        }

        [Route("Account/Register")]
        public ActionResult Register()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Register", account);
            }
        }

        [HttpPost]
        [Route("Account/Register")]
        public ActionResult Register(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //If we hit this, then something failed 
                viewModel.error = ViewModel.ViewError.ERROR;
                viewModel.message = "Please enter in the required fields.";
                return View(viewModel);
            }
            else
            {
                if (viewModel.Create())
                {
                    // User Registraion was successful
                    Session["Message"] = "Registration was successful. Please login to continue.";
                    Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.ERROR;
                    viewModel.message = "We were unable to register your account. Please try again";
                    return View("Register", viewModel);
                }
            }
        }

        [Route("Account/Update")]
        public ActionResult Update()
        {
            if (account != null)
            {
                account.SetFields();
                return View("Update", account);
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
            if (IsLoggedIn())
            {
                // Verify the user being updated is legitly the user logged in
                if (viewModel.AccountId == account.AccountId)
                {
                    if (account.Update(viewModel))
                    {
                        viewModel.error = ViewModel.ViewError.SUCCESS;
                        viewModel.message = "Your account was successfully updated.";
                        Session["User.Name"] = viewModel.FirstName;
                        Session["Message"] = viewModel.message;
                        Session["Message.Class"] = viewModel.error;
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        // There was an error updating the account
                        viewModel.error = ViewModel.ViewError.ERROR;
                        viewModel.message = "There was an error updating your account.";
                    }
                }
                else
                {
                    // Log the user out as I feel this is a hacking attempt
                    Session.RemoveAll();
                    Session["Message"] = "Unfortunately, we're unable to update your account. Please login and try again.";
                    Session["Message.Class"] = ViewModel.ViewError.ERROR;
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                Session.RemoveAll();
                Session["Message"] = "Please login to edit your account information";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }

            return View("Update", viewModel);
        }
    }
}
