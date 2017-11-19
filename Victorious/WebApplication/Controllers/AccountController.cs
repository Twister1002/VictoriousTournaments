using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security.Facebook;
using WebApplication.Models;
using WebApplication.Models.ViewModels;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            if (account.IsLoggedIn())
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
                account.SetupViewModel();
                return View("Login", account.viewModel);
            }
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.message = "Please enter in the required fields.";
                viewModel.errorType = ViewError.ERROR;
            }
            else
            {
                if (account.Login(viewModel))
                {
                    Session["User.UserId"] = account.Model.AccountID;
                    return RedirectToAction("Index", "Account");
                }
            }

            return View("Login", viewModel);
        }

        [Route("Account/Register")]
        public ActionResult Register()
        {
            if (account.IsLoggedIn())
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Register", account.viewModel);
            }
        }

        [HttpPost]
        [Route("Account/Register")]
        public ActionResult Register(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid && viewModel.ProviderID == 0)
            {
                viewModel.message = "Please enter in the required fields.";
                viewModel.errorType = ViewError.ERROR;

                return View(viewModel);
            }
            else
            {
                if (account.Create(viewModel))
                {
                    // User Registraion was successful
                    viewModel.message = "Registration was successful. Please login to continue.";
                    viewModel.errorType = ViewError.SUCCESS;

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return View("Register", viewModel);
                }
            }
        }

        [Route("Account/Update")]
        public ActionResult Update()
        {
            if (account.IsLoggedIn())
            {
                account.SetFields();
                return View("Update", account.viewModel);
            }
            else
            {
                Session["Message"] = "You need you login to update your account.";
                Session["Message.Class"] = ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [Route("Account/Update")]
        public ActionResult Update(AccountViewModel viewModel)
        {
            // Require the logged in account and the username to match to continue
            if (account.IsLoggedIn() && viewModel.Username == account.GetUsername())
            {
                if (account.Update(viewModel))
                {
                    Session["User.Name"] = viewModel.FirstName;
                    Session["Message"] = "Your account was successfully updated.";
                    Session["Message.Class"] = ViewError.SUCCESS;

                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    // There was an error updating the account
                    Session["Message"] = "There was an error updating your account.";
                    Session["Message.Class"] = ViewError.ERROR;
                }
            }
            else
            {
                Session.RemoveAll();
                Session["Message"] = "Please login to edit your account information";
                Session["Message.Class"] = ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }

            return View("Update", viewModel);
        }

        [HttpGet]
        [Route("Account/Forgot")]
        public ActionResult Forgot()
        {
            if (!account.IsLoggedIn())
            {
                return View("Forgot", account.viewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("Account/Forgot")]
        public ActionResult Forgot(AccountViewModel viewModel)
        {
            if (!account.IsLoggedIn())
            {
                return View("Forgot", account.viewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
