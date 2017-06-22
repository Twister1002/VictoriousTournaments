using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AdministratorController : VictoriousController
    {
        // GET: Administrator
        [Route("Administrator")]
        public ActionResult Index()
        {
            if (account.IsAdministrator())
            {
                return View("Index", new Admin(service));
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
    }
}