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
            int id = -1;

            if (int.TryParse(guid, out id))
            {
                TournamentViewModel viewModel = new TournamentViewModel(db.GetTournamentById(id));

                if (viewModel.model != null)
                {
                    return View("Tournament", viewModel);
                }
                else
                {
                    Session["Server.Message"] = "The tournmanet you're looking for doesn't exist or is not publicly shared.";
                    return RedirectToAction("Search", "Tournament");
                }
            }
            else
            {
                return RedirectToAction("Search", "Tournament");
            }
        }

        // GET: Tournament/Create
        [Route("Tournament/Create")]
        public ActionResult Create()
        {
            if (Session["User.UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TournamentFormModel viewModel = new TournamentFormModel();

                return View(viewModel);
            }
        }

        [Route("Tournament/Search/{title?}")]
        public ActionResult Search(String title)
        {
            TournamentSearchViewModel model = new TournamentSearchViewModel();

            return View("Search", model);
        }

        // GET: Tournament/Edit/5
        [Route("Tournament/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            TournamentModel model = db.GetTournamentById(id);
            if (Session["User.UserId"] != null && model.CreatedByID == (int)Session["User.UserId"])
            {
                if (model != null)
                {
                    TournamentFormModel viewModel = new TournamentFormModel(model);
                    return View("Edit", viewModel);
                }
                else
                {
                    Session["Message"] = "That tournament doesn't exist.";
                }
            }
            else
            {
                Session["Message"] = "You do not have permission to do that.";
            }


            return RedirectToAction("Search", "Tournament");
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
            // Verify the user is logged in first
            if (Session["User.UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                TournamentModel model = new TournamentModel()
                {
                    Title = viewModel.Title,
                    CreatedByID = (int)Session["User.UserId"],
                    CreatedOn = DateTime.Now,
                    Description = viewModel.Description
                };
                TournamentRuleModel rules = new TournamentRuleModel()
                {
                    IsPublic = viewModel.IsPublic
                };

                //DbError dbError = db.AddTournament(model);
                DbError result = db.AddTournament(ref model);
                
                if (result == DbError.SUCCESS) {
                    rules.TournamentID = model.TournamentID;
                    //DbError ruleResult = db.AddRules(ref rules, model);
                    //if (ruleResult == DbError.SUCCESS)
                    //{
                    return RedirectToAction("Tournmanet", "Tournament", new { guid = model.TournamentID });
                    //}
                    //else
                    //{
                    //    db.DeleteTournament(model);
                    //    viewModel.dbException = db.e;
                    //    viewModel.error = ViewModel.ViewError.CRITICAL;
                    //    viewModel.message = "Unable to create the rules for the tournament.";
                    //    return View(viewModel);
                    //}
                }
                else
                {
                    // The tournament failed to be created
                    viewModel.dbException = db.e;
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
        [Route("Tournament/Edit/{id}")]
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
