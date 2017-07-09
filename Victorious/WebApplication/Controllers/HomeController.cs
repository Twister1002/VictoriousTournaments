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
            Home model = new Home(service);
            Session["Message"] = "Our blog is currently not setup yet.";
            Session["Message.Class"] = ViewError.NONE;

            return View("Blog", model);
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
                if (model.SendEmail(viewModel))
                {
                    Session["Message"] = "Your message has been sent. We'll get back to you when we can!";
                    Session["Message.Class"] = ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "There was an unexpected error sending your mail. Please try again.";
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