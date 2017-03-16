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

        // Tournament Search
        [Route("Tournament/Search/{title?}")]
        public ActionResult Search(String title)
        {
            TournamentViewModel model = new TournamentViewModel();
            model.Search(title);

            return View("Search", model);
        }

        // Tournament Info
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid)
        {
            int id = -1;

            if (int.TryParse(guid, out id))
            {
                TournamentViewModel viewModel = new TournamentViewModel(db.GetTournamentById(id));
                viewModel.ProcessTournament();

                if (viewModel.Model != null)
                {
                    return View("Tournament", viewModel);
                }
                else
                {
                    Session["Message"] = "The tournament you're looking for doesn't exist or is not publicly shared.";
                    Session["Message.Class"] = ViewModel.ViewError.WARNING;
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
                TournamentViewModel viewModel = new TournamentViewModel();
                return View("Create", viewModel);
            }
        }

        // GET: Tournament/Edit/5
        [Route("Tournament/Update/{id}")]
        public ActionResult Update(int id)
        {
            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(db.GetTournamentById(id));

                if (viewModel.Model != null && viewModel.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    return View("Edit", viewModel);
                }
                else
                {
                    Session["Message"] = "You do not have permission to do that.";
                    Session["Message.Class"] = ViewModel.ViewError.CRITICAL;

                    return RedirectToAction("Tournament", "Tournament", new { guid = viewModel.Model.TournamentID });
                }
            }
            else
            {
                Session["Message"] = "You need to login to do that";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;

                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Tournament/Delete/5
        [Route("Tournament/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(id);

                if ((int)Session["User.UserId"] == viewModel.Model.CreatedByID)
                {
                    return View("Delete", viewModel);
                }
                else
                {
                    Session["Message"] = "You do not have permission to do that.";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;

                    return RedirectToAction("Tournament", "Tournament", new { @guid = viewModel.Model.TournamentID });
                }
            }
            else
            {
                Session["Message"] = "You need to login to do that";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;

                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentViewModel viewModel)
        {
            // Verify the user is logged in first
            if (Session["User.UserId"] == null)
            {
                Session["Message"] = "You must login to create a tournament.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                viewModel.ApplyChanges((int)Session["User.UserId"]);

                TournamentModel model = viewModel.Model;
                DbError result = db.AddTournament(ref model);

                if (result == DbError.SUCCESS)
                {
                    return RedirectToAction("Tournament", "Tournament", new { guid = model.TournamentID });
                }
                else
                {
                    // The tournament failed to be created
                    viewModel.dbException = db.exception;
                    viewModel.error = ViewModel.ViewError.EXCEPTION;
                    viewModel.message = "We could not create the tournament due to an error.";
                    return View("Create", viewModel);
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.message = "Please enter in the required fields listed below.";
                return View("Create", viewModel);
            }
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        [Route("Tournament/Update/{id}")]
        public ActionResult Update(TournamentViewModel viewModel)
        {
            if (Session["User.UserId"] != null)
            {
                if (viewModel.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    viewModel.ApplyChanges((int)Session["User.UserId."]);

                    DbError tourny = db.UpdateTournament(viewModel.Model);
                    DbError rules = db.UpdateRules(viewModel.Model.TournamentRules);

                    if (tourny == DbError.SUCCESS && rules == DbError.SUCCESS)
                    {
                        viewModel.error = ViewModel.ViewError.SUCCESS;
                        viewModel.message = "Edits to the tournament was successful";

                        //Session["Message"] = viewModel.message;
                        //Session["Message.Class"] = viewModel.error;

                        return RedirectToAction("Tournament", "Tournament", new { guid = viewModel.Model.TournamentID });
                    }
                    else
                    {
                        viewModel.error = ViewModel.ViewError.CRITICAL;
                        viewModel.message = "Something went wrong while trying to update your tournament. Please try again or submit a ticket.";
                        viewModel.dbException = db.exception;
                    }
                }
                else
                {
                    viewModel.message = "You do not have permission to update this tournament";
                    viewModel.error = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                Session["Message"] = "You must login to edit a tournament.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Login", "Account");
            }

            return View("Edit", viewModel);
        }

        // POST: Tournament/Delete/5
        [HttpPost]
        [Route("Tournament/Delete/{guid}")]
        public ActionResult Delete(TournamentViewModel viewModel)
        {
            // TODO: When ability comes in, check against administrators
            if (Session["User.UserId"] != null)
            {
                // Is this user authorized to make changes?
                if (viewModel.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    DbError tourny = db.DeleteTournament(viewModel.Model);
                    DbError rules = db.DeleteTournamentRules(viewModel.Model.TournamentRules);

                    if (tourny == DbError.SUCCESS && rules == DbError.SUCCESS)
                    {
                        viewModel.error = ViewModel.ViewError.SUCCESS;
                        viewModel.message = "The tournament was successfully deleted.";
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        viewModel.error = ViewModel.ViewError.CRITICAL;
                        viewModel.message = "Unable to update the tournament. Please try again later.";
                        viewModel.dbException = db.exception;
                    }
                }
                else
                {
                    Session["Message"] = "You do not have permission to update this tournament";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                    return RedirectToAction("Tournament", "Tournament", new { @id = viewModel.Model.TournamentID });
                }
            }
            else
            {
                Session["Message"] = "You do not have permission to update this tournament";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
            }

            return View("Delete", viewModel);
        }

        [HttpPost]
        [Route("Tournament/Ajax/Delete")]
        public JsonResult Delete(String id)
        {
            JsonResult json = new JsonResult();

            int uid = -1;
            if (Session["User.UserId"] != null && int.TryParse(id, out uid))
            {
                TournamentViewModel model = new TournamentViewModel(uid);
                if (model.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    return Json(new { status=true, message="Tournament was deleted." });
                }
                else
                {
                    return Json(new { status = false, message = "User is not the owner of this tournament." });
                }
            }
            else
            {
                return Json(new { status = false, message = "User needs to login." });
            }
        }
    }
}