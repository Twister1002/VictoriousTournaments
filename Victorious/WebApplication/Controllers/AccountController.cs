using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Models.ViewModels;

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
                return View("Login", account.viewModel);
            }
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                Session["Message"] = "Please enter in the required fields.";
                Session["Message.Class"] = ViewError.ERROR;
            }
            else
            {
                if (account.Login(viewModel))
                {
                    Session["User.UserId"] = account.Model.AccountID;
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    Session["Message"] = "The username or password is invalid.";
                    Session["Message.Class"] = ViewError.WARNING;
                    viewModel.e = service.e;
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
            if (!ModelState.IsValid)
            {
                Session["Message"] = "Please enter in the required fields.";
                Session["Message.ClasS"] = ViewError.ERROR;

                return View(viewModel);
            }
            else
            {
                if (account.Create(viewModel))
                {
                    // User Registraion was successful
                    Session["Message"] = "Registration was successful. Please login to continue.";
                    Session["Message.Class"] = ViewError.SUCCESS;

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Session["Message"] = "We were unable to register your account. Please try again";
                    Session["Message.Class"] = ViewError.ERROR;
                    viewModel.e = service.e;

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
            if (account.IsLoggedIn())
            {
                //// Verify the user being updated is legitly the user logged in
                //if (viewModel.AccountId == account.Model.AccountID)
                //{
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
                //}
                //else
                //{
                //    // Log the user out as I feel this is a hacking attempt
                //    Session.RemoveAll();
                //    Session["Message"] = "Unfortunately, we're unable to update your account. Please login and try again.";
                //    Session["Message.Class"] = ViewError.ERROR;
                //    return RedirectToAction("Login", "Account");
                //}
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
    }
}
