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
            TournamentSearchViewModel model = new TournamentSearchViewModel();

            return View("Search", model);
        }

        // GET: Tournament/Edit/5
        public ActionResult Edit(int id)
        {
            return View("Edit");
        }

        // GET: Tournament/Delete/5
        public ActionResult Delete(int id)
        {
            return View("Delete");
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentFormModel viewModel)
        {
            if (ModelState.IsValid)
            {
                TournamentModel model = new TournamentModel()
                {
                    Title = viewModel.Title,
                    CreatedByID = (int)Session["User.UserId"],
                    CreatedOn = DateTime.Now,
                    Description = viewModel.Description
                };

                //DbError dbError = db.AddTournament(model);
                int tournamentId = db.AddTournament(model);

                //if (dbError == DbError.SUCCESS)
                if (tournamentId > 0)
                {
                    return RedirectToAction("Tournament/" + tournamentId);
                }
                else
                {
                    // The tournament failed to be created
                    viewModel.error = ViewModel.ViewError.EXCEPTION;
                    viewModel.message = "We could not create the tournament due to an error.";
                    return View(viewModel);
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.message = "Please enter in the required fields listed below.";
                return View(viewModel);
            }
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            // TODO: Add update logic here

            return RedirectToAction("Index");
        }

        // POST: Tournament/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // TODO: Add delete logic here

            return RedirectToAction("Index");
        }
    }
}
