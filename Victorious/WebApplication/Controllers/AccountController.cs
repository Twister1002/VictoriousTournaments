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
        // Remove when we have ability to access database
        List<User> users = new List<User>();

        //users.(new User{ FirstName = "User", LastName = "1", UserName="admin", UserID=1, Password="pass" });
        //users.Add(new User { FirstName="User", LastName="2", UserName="User", UserID=2, Password="pass2" });

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
                //User user = users.Find(u => u.UserName == viewModel.Username);
                if (viewModel.Username == "admin")
                {
                    //Verfy the password is correct
                    if (viewModel.Password == "pass")
                    {
                        Session["UserId"] = 0;
                        Session["UserName"] = viewModel.Username;
                        return RedirectToAction("Index", "Account");
                    }
                }
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
                if (users.Select(u => u.UserName == user.Email).Count() > 0)
                {
                    // There is a user that exists with this username all ready. Send the back with an error
                    return View();
                }
                else
                {
                    // Verify the password is strong enough

                    // Email Verification?

                    // Add the user to the database
                    //users.Add(new User(user));
                    //User dbUser = new User();
                    //dbUser.UserName = user.Username;
                    //dbUser.Password = user.Password;
                    //dbUser.Email = user.Email;

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
