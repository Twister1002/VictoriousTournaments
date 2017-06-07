using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AdministratorController : VictoriousController
    {
        // GET: Administrator
        [Route("Administrator")]
        public ActionResult Index()
        {
            if (account.IsAdministrator())
            {
                return View("Index", new AdministratorViewModel(uow));
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
    }
}