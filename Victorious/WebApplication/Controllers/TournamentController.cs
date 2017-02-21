using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TournamentController : Controller
    {
        // GET: Tournament
        [Route("Tournament")]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [Route("Tournament/Search/{value?}")]
        public ActionResult Search(String value)
        {
            return View("Search");
        }
        
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid)
        {
            TournamentModel model = new TournamentModel(guid);

            return View("Tournament", model);
        }

        [Route("Tournament/{org}/{guid}")]
        public ActionResult TournamentOrginization(String org, String guid)
        {
            TournamentModel model = new TournamentModel("LALA-LALA-LALA-LALALA", "Merp", "This is fancy!");
            return View("Tournament", model);
        }

        // GET: Tournament/Create
        [Route("Tournament/Create")]
        public ActionResult Create()
        {
            TournamentViewModel model = new TournamentViewModel();
            model.RegistrationStart = DateTime.Now;
            model.RegistrationEnd = DateTime.Now.AddDays(1);
            model.CheckInDateTime = DateTime.Now.AddDays(3);

            return View(model);
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: Tournament/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tournament/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tournament/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
