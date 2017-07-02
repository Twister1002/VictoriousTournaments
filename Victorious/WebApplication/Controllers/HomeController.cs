using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Models.ViewModels;

namespace WebApplication.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : VictoriousController
    {

        [Route("")]
        public ActionResult Index()
        {
            Home model = new Home(service);

            return View("Index", model);
        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            Home model = new Home(service);
            return View("FAQ", model);
        }

        [Route("Blog")]
        public ActionResult Blog()
        {
            Session["Message"] = "The blog is currently not working.";
            Session["Message.Class"] = ViewError.ERROR;

            return RedirectToAction("Index", "Home");
        }

        [Route("About")]
        public ActionResult About()
        {
            Home model = new Home(service);
            return View("About", model);
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            Contact model = new Contact(service);
            Session["Message"] = "Our mail server is currently not working. Please wait while we fix this.";
            Session["Message.Class"] = ViewError.NONE;
            

            return View("Contact", model.viewModel);
        }

        [HttpPost]
        [Route("Contact")]
        public ActionResult Contact(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Contact model = new Contact(service);
                if (model.SendEmail())
                {
                    Session["Message"] = "Your message is now pending to be sent";
                    Session["Message.Class"] = ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "Our mail server is currently not working. Please wait while we fix this.";
                    Session["Message.Class"] = ViewError.ERROR;
                }
            }
            else
            {
                Session["Message"] = "Please fix the errors below";
                Session["Message.Class"] = ViewError.ERROR;
            }

            return View("Contact", viewModel);
        }

        [Route("Rules")]
        public ActionResult Rules()
        {
            Home model = new Home(service);
            Session["Message"] = "We're currenly working up the basic details for the tournaments. More details will come shortly";
            Session["Message.Class"] = ViewError.NONE;

            return View("Rules", model);
        }
    }
}