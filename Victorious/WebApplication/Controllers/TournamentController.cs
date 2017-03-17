﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using DataLib;
using Tournament.Structure;

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
        public ActionResult Update(TournamentViewModel viewModel, int id)
        {
            if (Session["User.UserId"] != null)
            {
                viewModel.SetModel(id);
                if (viewModel.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    viewModel.ApplyChanges((int)Session["User.UserId"]);

                    DbError tourny = db.UpdateTournament(viewModel.Model);
                    DbError rules = db.UpdateRules(viewModel.Model.TournamentRules);

                    if (tourny == DbError.SUCCESS && rules == DbError.SUCCESS)
                    {
                        viewModel.error = ViewModel.ViewError.SUCCESS;
                        viewModel.message = "Edits to the tournament was successful";

                        Session["Message"] = viewModel.message;
                        Session["Message.Class"] = viewModel.error;

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
        [Route("Tournament/Register/{tournamentVal}")]
        public ActionResult Register(String tournamentVal)
        {
            int tournamentId = -1;
            if (int.TryParse(tournamentVal, out tournamentId))
            {
                if (Session["User.UserId"] != null)
                {
                    // Verify the user doesn't exist in the tournament all ready
                    // Dont want duplicates
                    TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                    List<UserModel> usersInTournament = db.GetAllUsersInTournament(viewModel.Model);
                    int userCount = usersInTournament.FindAll(x => x.UserID == (int)Session["User.UserId"]).Count;

                    if (userCount == 0)
                    {
                        // Add the user to the tournament
                        DbError error = db.AddUserToTournament(viewModel.Model, db.GetUserById((int)Session["User.UserId"]));
                        if (error == DbError.SUCCESS)
                        {
                            Session["Message"] = "You have been registered to this tournament";
                            Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                        }
                        else
                        {
                            Session["Message"] = "There was an error in registering you in the tournament";
                            Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                        }
                    }
                    else
                    {
                        Session["Message"] = "You have all ready registered for this tournament";
                        Session["Message.Class"] = ViewModel.ViewError.WARNING;
                    }
                }
                else
                {
                    Session["Message"] = "You must login to register for this tournament";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                Session["Message"] = "We don't seem to recognize this tournament.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
            }
            return RedirectToAction("Tournament", "Tournament", new { guid = tournamentId });
        }

        [HttpPost]
        [Route("Tournament/Deregister/{id}")]
        public ActionResult Deregister(String tournamentVal)
        {
            int tournamentId = -1;
            if (int.TryParse(tournamentVal, out tournamentId))
            {
                if (Session["User.UserId"] != null)
                {
                    // We have a user logged in.
                    //viewModel.SetModel(tournamentId);

                    DbError result = DbError.NONE; //db.AddUserToTournament(viewModel.Model, db.GetUserById((int)Session["User.UserId"]));
                    if (result == DbError.SUCCESS)
                    {
                        Session["Message"] = "You have registered for this tournament.";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    else
                    {
                        Session["Message"] = "We were not able to register you for this tournament. Please notify the tournament administrator.";
                        Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                    }
                }
                else
                {
                    Session["Message"] = "You must login before you can register for a tournament.";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                Session["Message"] = "We don't seem to recognize this tournament.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
            }
            //return View("Tournament", viewModel);

        
            Session["Message"] = "Currently you are not able to deregister from a tournament.";
            Session["Message.Class"] = ViewModel.ViewError.WARNING;
            return RedirectToAction("Index", "Home");
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

        [HttpPost]
        [Route("Tournament/Ajax/Match/Update")]
        public JsonResult MatchUpdate(String match, String tournamentId, String seedWin)
        {
            if (Session["User.UserId"] != null)
            {
                int tournyId = int.Parse(tournamentId);
                int matchId = int.Parse(match);
                int seedId = int.Parse(seedWin);
                PlayerSlot winner;

                TournamentViewModel viewModel = new TournamentViewModel(tournyId);
                viewModel.ProcessTournament();

                if (viewModel.Tourny.Brackets[0].GetMatch(matchId).ChallengerIndex() == seedId)
                {
                    winner = PlayerSlot.Challenger;
                }
                else
                {
                    winner = PlayerSlot.Defender;
                }

                if (viewModel.Model.CreatedByID == (int)Session["User.UserId"])
                {
                    try
                    {
                        viewModel.Tourny.Brackets[0].AddWin(matchId, winner);
                        return Json(new { status = true, message = "Match was updated successfully" });
                    }
                    catch (Exception e)
                    {
                        return Json(new { status = false, message = "Exception thrown: "+e.Message });
                    }
                }
                else
                {
                    return Json(new { status = false, message = "You are not allowed to update matches" });
                }
            }

            return Json(new { status = false, message = "You must login before adjusting matches" });
        }
    }
}