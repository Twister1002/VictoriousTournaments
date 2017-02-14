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
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [Route("Account/Login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/Login")]
        public ActionResult Login(LoginViewModel user)
        {
            String adminName = "Admin";
            String adminPass = "Pass";

            if (ModelState.IsValid)
            {
                //We need now to check the login credentias of the user

                //Query for the username
                if (user.Username == adminName)
                {
                    //Verfy the password is correct
                    if (user.Password == adminPass)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "This is an invalid model");
            }

            return View(user);
        }
        
        [Route("Account/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/Register")]
        public ActionResult Register(RegisterViewModel user)
        {

            //If we hit this, then something failed 
            return View(user);
        }

        [Route("Account/Logout")]
        public ActionResult Logout()
        {
            return View();
        }
    }
}
