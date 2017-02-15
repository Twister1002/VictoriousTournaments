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
        public ActionResult Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                //We need now to check the login credentias of the user
                //Query for the username
                if (user.Username == "admin")
                {
                    //Verfy the password is correct
                    if (user.Password == "pass")
                    {
                        Session["UserId"] = 0;
                        Session["UserName"] = user.Username;
                        return RedirectToAction("Index", "Account");
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
        public ActionResult Register(RegisterViewModel user)
        {

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
