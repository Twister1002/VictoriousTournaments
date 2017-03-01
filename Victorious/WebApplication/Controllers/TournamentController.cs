using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using DataLib;

namespace WebApplication.Controllers
{
    public class TournamentController : Controller
    {
        DatabaseInterface db = new DatabaseInterface();

        [Route("Tournament")]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid)
        {
            int guidInt = -1;
            TournamentViewModel model = new TournamentViewModel();

            //if (int.TryParse(guid, out guidInt))
            //{
            //    TournamentModel dbModel = db.GetTournamentById(guidInt);
            //    if (dbModel != null)
            //    {
            //        model = dbModel;
            //    }
            //}




            return View("Tournament", model);
        }

        // GET: Tournament/Create
        [Route("Tournament/Create")]
        public ActionResult Create()
        {
            TournamentFormModel viewModel = new TournamentFormModel();

            return View(viewModel);
        }

        [Route("Tournament/Search/{title?}")]
        public ActionResult Search(String title)
        {
            TournamentModel model = new TournamentModel();

            return View("Search", model);
        }
        
        

        // GET: Tournament/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // GET: Tournament/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentFormModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    TournamentModel model = new TournamentModel();
                    model.Title = viewModel.Title;
                    model.CreatedByID = (int)Session["User.UserId"];
                    model.CreatedOn = DateTime.Now;
                    model.Description = viewModel.Description;

                    viewModel.Exception = db.AddTournament(model);

                    if (viewModel.Exception == DbError.SUCCESS)
                    {
                        return RedirectToAction("Tournament/"+model.TournamentID);
                    }
                    else
                    {
                        return View(viewModel);
                    }
                }

                return View(viewModel);
            }
            catch
            {
                return View(viewModel);
            }
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
