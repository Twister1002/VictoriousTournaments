using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tournaments = Tournament.Structure;
using WebApplication.Models;
using DatabaseLib;
using WebApplication.Models.ViewModels;

namespace WebApplication.Controllers
{
    public class TournamentController : VictoriousController
    {

        [HttpGet]
        [Route("Tournament")]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        // Tournament Search
        [HttpGet]
        [Route("Tournament/Search")]
        public ActionResult Search(Dictionary<String, String> searchBy)
        {
            Models.Tournament model = new Models.Tournament(service, -1);
            model.Search(searchBy);

            return View("Search", model);
        }
        
        // GET: Tournament/Create
        [HttpGet]
        [Route("Tournament/Create")]
        public ActionResult Create()
        {
            if (Session["User.UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Models.Tournament tournament = new Models.Tournament(service, -1);
                return View("Create", tournament.viewModel);
            }
        }

        // GET: Tournament/Edit/5
        [HttpGet]
        [Route("Tournament/Update/{tournamentId}")]
        public ActionResult Update(int tournamentId)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tourny = new Models.Tournament(service, tournamentId);
                if (tourny.IsAdmin(account.Model.AccountID))
                {
                    ViewBag.CanEdit = tourny.CanEdit();
                    ViewBag.InProgress = tourny.Model.InProgress;
                    tourny.SetFields();
                    return View("Update", tourny.viewModel);
                }
                else
                {
                    Session["Message"] = "You do not have permission to do that.";
                    Session["Message.Class"] = ViewError.ERROR;

                    return RedirectToAction("Tournament", "Tournament", new { guid = tourny.Model.TournamentID });
                }
            }
            else
            {
                Session["Message"] = "You need to login to do that";
                Session["Message.Class"] = ViewError.ERROR;

                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid, String inviteCode)
        {
            int tournamentId = ConvertToInt(guid);
            Models.Tournament tourny = new Models.Tournament(service, tournamentId);

            if (tournamentId == 0) {
                Session["Message"] = "This tournament is invalid.";
                Session["Message.Class"] = ViewError.ERROR;
                return RedirectToAction("Index", "Tournament");
            }

            if (tourny.Model != null)
            {
                bool isAdmin = tourny.IsAdmin(account.Model.AccountID);
                bool isParticipant = tourny.IsParticipent(account.Model.AccountID);

                // Should we check for registrations or view the tournament?
                if (!tourny.Model.InProgress && !isAdmin)
                {
                    // Verify if the user has an invite code or the invite code is valid
                    if (tourny.Model.PublicRegistration || tourny.Model.InviteCode == inviteCode)
                    {
                        TournamentRegisterViewModel fields = new TournamentRegisterViewModel()
                        {
                            AccountID = account.Model.AccountID,
                            TournamentID = tourny.Model.TournamentID
                        };

                        // Allow the tournament registration to be shown
                        ViewBag.Tournament = tourny.Model;
                        ViewBag.isRegistered = tourny.isRegistered(account.Model.AccountID);
                        ViewBag.CanRegister = tourny.CanRegister();


                        return View("RegisterForm", fields);
                    }
                    else
                    {
                        Session["Message"] = "This tournament is not accepting registrations.";
                        Session["Message.Class"] = ViewError.WARNING;
                    }
                }
                else
                {
                    // Verify if the user is allowed to view the tournament
                    if (tourny.Model.PublicViewing || tourny.Model.InviteCode == inviteCode || isAdmin || isParticipant)
                    {
                        return View("Tournament", tourny);
                    }
                    else
                    {
                        Session["Message"] = "This tournament is not available to view.";
                        Session["Message.Class"] = ViewError.WARNING;
                    }
                }
            }
            else
            {
                Session["Message"] = "The tournament you're looking for doesn't exist or is not publicly shared.";
                Session["Message.Class"] = ViewError.WARNING;
            }


            return RedirectToAction("Search", "Tournament");
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentViewModel viewModel)
        {
            // Verify the user is logged in first
            if (!account.IsLoggedIn())
            {
                Session["Message"] = "You must login to create a tournament.";
                Session["Message.Class"] = ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Models.Tournament tourny = new Models.Tournament(service, -1);
                if (ModelState.IsValid)
                {
                    
                    if (tourny.Create(viewModel, account))
                    {
                        return RedirectToAction("Tournament", "Tournament", new { guid = tourny.Model.TournamentID });
                    }
                    else
                    {
                        // Show a success message.
                        Session["Message"] = "We were unable to create your account.";
                        Session["Message.Class"] = ViewError.ERROR;
                    }
                }
                else
                {
                    Session["Message.Class"] = ViewError.ERROR;
                    Session["Message"] = "Please enter in the required fields listed below.";
                }
                return View("Create", tourny.viewModel);
            }

            
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        [Route("Tournament/Update/{tournamentId}")]
        public ActionResult Update(TournamentViewModel viewModel, int tournamentId)
        {
            if (account.IsLoggedIn())
            {
                Models.Tournament tourny = new Models.Tournament(service, tournamentId);

                if (tourny.IsAdmin(account.Model.AccountID))
                {
                    if (tourny.Update(viewModel, account.Model.AccountID))
                    {
                        Session["Message"] = "Edits to the tournament was successful";
                        Session["Message.Class"] = ViewError.SUCCESS;

                        return RedirectToAction("Tournament", "Tournament", new { guid = tourny.Model.TournamentID });
                    }
                    else
                    {
                        Session["Message"] = "We were unable to update your tournament. Please try again soon";
                        Session["Message.Class"] = ViewError.ERROR;
                    }
                }
                else
                {
                    Session["Message"] = "You do not have permission to update this tournament";
                    Session["Message.Class"] = ViewError.ERROR;
                }

                ViewBag.CanEdit = tourny.CanEdit();
                ViewBag.InProgress = tourny.Model.InProgress;
                tourny.SetFields();
                return View("Update", tourny.viewModel);
            }
            else
            {
                Session["Message"] = "You must login to edit a tournament.";
                Session["Message.Class"] = ViewError.ERROR;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [Route("Tournament/Register")]
        public ActionResult Register(TournamentRegisterViewModel userData)
        {
            if (userData.AccountID == account.Model.AccountID)
            {
                Models.Tournament viewModel = new Models.Tournament(service, userData.TournamentID);

                if (viewModel.AddUser(account, Permission.TOURNAMENT_STANDARD))
                {
                    Session["Message"] = "You have been registered to this tournament";
                    Session["Message.Class"] = ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "We were unable to add you to the tournament";
                    Session["Message.Class"] = ViewError.ERROR;
                }
            }
            else
            {
                Session["Message"] = "You must login to register for this tournament";
                Session["Message.Class"] = ViewError.ERROR;
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Tournament", "Tournament", new { guid = userData.TournamentID });
        }

        [HttpPost]
        [Route("Tournament/Deregister")]
        public ActionResult Deregister(TournamentRegisterViewModel userData)
        {
            if (userData.AccountID == account.Model.AccountID)
            {
                Models.Tournament viewModel = new Models.Tournament(service, userData.TournamentID);
                if (viewModel.RemoveUser(account.Model.AccountID))
                {
                    Session["Message"] = "You have been removed from this tournament.";
                    Session["Message.Class"] = ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "We could not remove you from the tournament due to an error.";
                    Session["Message.Class"] = ViewError.ERROR;
                }
            }
            else
            {
                Session["Message"] = "You must login to do this action.";
                Session["Message.Class"] = ViewError.ERROR;
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Tournament", "Tournament", new { guid = userData.TournamentID });
        }
    }
}