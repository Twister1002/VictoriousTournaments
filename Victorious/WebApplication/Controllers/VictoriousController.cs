using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tournaments = Tournament.Structure;
using Tournament.Structure;
using WebApplication.Models;
using WebApplication.Utility;

namespace WebApplication.Controllers
{
    public abstract class VictoriousController : Controller
    {
        protected Account account;
        protected IUnitOfWork work;
        protected Service service;
        Dictionary<String, object> jsonResponse;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            LoadAccount(requestContext.HttpContext.Session);
        }

        public VictoriousController()
        {
#if DEBUG
            work = new UnitOfWork("Debug");
#else
            work = new UnitOfWork("Production");
#endif
            jsonResponse = new Dictionary<String, object>();
            service = new Service(work);
        }

        protected override void Dispose(bool disposing)
        {
            work.Dispose();
        }

        public void LoadAccount(HttpSessionStateBase Session)
        {
            if (Session != null)
            {
                if (Session["User.UserId"] != null)
                {
                    account = new Account(service, (int)Session["User.UserId"]);
                }
                else
                {
                    account = new Account(service, -1);
                }
            }
            else
            {
                account = null;
            }

            // Set some Viewbag data
            ViewBag.IsLoggedin = account.IsLoggedIn();
            ViewBag.Username = account.GetUsername();
        }

        [Obsolete("Use Account.IsLoggedIn()")]
        public bool IsLoggedIn()
        {
            if (account != null && account.Model.AccountID > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int ConvertToInt(String x)
        {
            int i = -1;
            int.TryParse(x, out i);
            return i;
        }

        protected object JsonPlayerDataResponse(IPlayer player, int score = -1)
        {
            return new
            {
                id = player.Id,
                name = player.Name,
                score = score
            };
        }
    }
}