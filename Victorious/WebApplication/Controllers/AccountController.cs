using System.Web.Mvc;
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
            if (Session["User.UserId"] != null)
            {
                AccountViewModel model = new AccountViewModel((int)Session["User.UserId"]);
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
            if (!ModelState.IsValid)
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
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
            if (!ModelState.IsValid)
            {
                //If we hit this, then something failed 
                viewModel.error = ViewModel.ViewError.EXCEPTION;
                viewModel.message = "Please enter in the required fields.";
                return View(viewModel);
            }
            else
            { 
                viewModel.ApplyChanges();

                if (viewModel.Create())
                {
                    // User Registraion was successful
                    Session["Message"] = "Registration was successful. Please login to continue.";
                    Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    viewModel.error = ViewModel.ViewError.CRITICAL;
                    viewModel.message = "We were unable to register your account. Please try again";
                    return View("Register", viewModel);
                }
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
            if (Session["User.UserId"] != null)
            {
                // Verify the user being updated is legitly the user logged in
                if (viewModel.Account.AccountID == (int)Session["User.UserId"])
                {
                    // Apply the changes
                    viewModel.ApplyChanges();

                    if (viewModel.Update())
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
                        viewModel.error = ViewModel.ViewError.CRITICAL;
                        viewModel.message = "There was an error updating your account. Please try again later.";
                    }
                }
                else
                {
                    // Log the user out as I feel this is a hacking attempt
                    Session.RemoveAll();
                    Session["Message"] = "Unfortunately, we're unable to update your account. Please login and try again.";
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

            return View("Update", viewModel);
        }
    }
}
