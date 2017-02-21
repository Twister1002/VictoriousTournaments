using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        // Remove when we have ability to access database
        List<UserModel> users = new List<UserModel>();

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
                //We need now to check the login credentias of the user
                //Query for the username
                UserModel user = users.Find(u => u.Username == viewModel.Username);
                //if (user.Username == "admin")
                //{
                //    //Verfy the password is correct
                //    if (user.Password == "pass")
                //    {
                //        Session["UserId"] = 0;
                //        Session["UserName"] = user.Username;
                //        return RedirectToAction("Index", "Account");
                //    }
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
                if (users.Select(u => u.Username == user.Email).Count() > 0)
                {
                    // There is a user that exists with this username all ready. Send the back with an error
                    return View();
                }
                else
                {
                    // Verify the password is strong enough

                    // Email Verification?

                    // Add the user to the database
                    users.Add(new UserModel(user));

                    // Let's just have the user login 
                    return RedirectToAction("Login", "Account");
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
