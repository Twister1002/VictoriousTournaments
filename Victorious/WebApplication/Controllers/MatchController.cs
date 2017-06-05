using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MatchController : VictoriousController
    {
        [Route("Match")]
        public ActionResult Index()
        {
            Session["Message"] = "You do not belong here";
            Session["Message.Error"] = ViewModel.ViewError.WARNING;
            return RedirectToAction("Index", "Home");
        }
    }
}