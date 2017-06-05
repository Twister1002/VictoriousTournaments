using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class BracketController : VictoriousController
    {
        [Route("Bracket")]
        // GET: Bracket
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}