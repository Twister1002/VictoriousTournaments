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
            return RedirectToAction("Index");
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
                    Session["Message"] = "Your message could not be sent. Please try again later.";
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
            return View("Rules", model);
        }
    }
}