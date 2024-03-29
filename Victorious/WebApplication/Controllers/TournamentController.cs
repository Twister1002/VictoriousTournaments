﻿using System;
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

        /// <summary>
        /// Gets the index of Tournament page.
        /// </summary>
        /// <returns>A redirection to the Search element</returns>
        [HttpGet]
        [Route("Tournament")]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        /// <summary>
        /// Handles the user's that come to this page to automatically search and display tournaments
        /// </summary>
        /// <param name="searchBy">Params sent to be used to search</param>
        /// <returns>The search.cshtml page that will display the tournaments</returns>
        [HttpGet]
        [Route("Tournament/Search")]
        public ActionResult Search(Dictionary<String, String> searchBy)
        {
            Models.Tournament model = new Models.Tournament(service, -1);
            model.Search(searchBy);

            return View("Search", model);
        }
        
        /// <summary>
        /// Allows the user to create a tournament if they are currently logged in
        /// </summary>
        /// <returns></returns>
        // GET: Tournament/Create
        [HttpGet]
        [Route("Tournament/Create")]
        public ActionResult Create()
        {
            if (!account.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Models.Tournament tournament = new Models.Tournament(service, -1);
                ViewBag.Create = true;
                ViewBag.CanEdit = tournament.CanEdit();
                ViewBag.InProgress = tournament.Model.InProgress;
                return View("Create", tournament.viewModel);
            }
        }

        /// <summary>
        /// Sends the user to the update page to update their tournament they have created
        /// </summary>
        /// <param name="tournamentId">ID of the tournament from the URL</param>
        /// <returns></returns>
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
                    ViewBag.Create = false;
                    ViewBag.CanEdit = tourny.CanEdit();
                    ViewBag.InProgress = tourny.Model.InProgress;
                    tourny.SetFields();
                    return View("Create", tourny.viewModel);
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

        /// <summary>
        /// Loads the tournament the user wants to view
        /// </summary>
        /// <param name="guid">The unique ID of the tournament</param>
        /// <param name="inviteCode">The invite code provided for the tournament</param>
        /// <returns>Allows the user to view the tournament or redirects them elsewhere</returns>
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

        /// <summary>
        /// When the user has submitted the form to create the tournament we must process this data
        /// </summary>
        /// <param name="viewModel">The model from the page that was created.</param>
        /// <returns></returns>
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
                        Session["Message"] = "We were unable to create the tournament.";
                        Session["Message.Class"] = ViewError.ERROR;
                    }
                }
                else
                {
                    Session["Message.Class"] = ViewError.ERROR;
                    Session["Message"] = "Please enter in the required fields listed below.";
                }

                ViewBag.Create = true;
                ViewBag.CanEdit = tourny.CanEdit();
                ViewBag.InProgress = tourny.Model.InProgress;
                return View("Create", tourny.viewModel);
            }
        }

        /// <summary>
        /// Updates a tournament when the user has submitted their form.
        /// </summary>
        /// <param name="viewModel">The model of all the form elements.</param>
        /// <param name="tournamentId">ID of the tournament.</param>
        /// <returns></returns>
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
                        Session["Message"] = "We were unable to update your tournament.";
                        Session["Message.Class"] = ViewError.ERROR;
                    }
                }
                else
                {
                    Session["Message"] = "You do not have permission to update this tournament";
                    Session["Message.Class"] = ViewError.ERROR;
                }

                ViewBag.Create = false;
                ViewBag.CanEdit = tourny.CanEdit();
                ViewBag.InProgress = tourny.Model.InProgress;

                return View("Create", viewModel);
            }
            else
            {
                Session["Message"] = "You must login to edit a tournament.";
                Session["Message.Class"] = ViewError.ERROR;
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Allows the user to register for a tournament
        /// </summary>
        /// <param name="userData">The model of the user</param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes the user from the tournament
        /// </summary>
        /// <param name="userData">The model of the user</param>
        /// <returns></returns>
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