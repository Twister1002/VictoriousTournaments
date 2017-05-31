using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : VictoriousController
    {
        [Route("")]
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            return View("Index", model);
        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            HomeViewModel model = new HomeViewModel();
            return View("FAQ", model);
        }

        [Route("Blog")]
        public ActionResult Blog()
        {
            Session["Message"] = "The blog is currently not working.";
            Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
            return RedirectToAction("Index");
        }

        [Route("About")]
        public ActionResult About()
        {
            HomeViewModel model = new HomeViewModel();
            return View("About", model);
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            HomeViewModel model = new HomeViewModel();
            return View("Contact", model);
        }

        [Route("Rules")]
        public ActionResult Rules()
        {
            HomeViewModel model = new HomeViewModel();
            return View("Rules", model);
        }
    }
}