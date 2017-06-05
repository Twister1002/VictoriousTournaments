using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication.Models;
using DatabaseLib;

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
            TournamentViewModel model = new TournamentViewModel();
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
                TournamentViewModel viewModel = new TournamentViewModel();
                return View("Create", viewModel);
            }
        }

        // GET: Tournament/Edit/5
        [HttpGet]
        [Route("Tournament/Update/{tournamentId}")]
        public ActionResult Update(int tournamentId)
        {
            if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.IsAdministrator(account.AccountId))
                {
                    return View("Update", viewModel);
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

        /// <summary>
        /// Displays the tournament
        /// </summary>
        /// <param name="guid">the ID of the tournament</param>
        /// <param name="inviteCode">The Invite code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid, String inviteCode)
        {
            int tournamentId = ConvertToInt(guid);
            TournamentViewModel viewModel = new TournamentViewModel(tournamentId);

            if (viewModel.Model != null)
            {
                bool isAdmin = viewModel.IsAdministrator(account.AccountId);
                bool isParticipant = viewModel.IsParticipant(account.AccountId);

                // Should we check for registrations or view the tournament?
                if (!viewModel.Model.InProgress && !isAdmin)
                {
                    // Verify if the user has an invite code or the invite code is valid
                    if (viewModel.PublicRegistration || viewModel.Model.InviteCode == inviteCode)
                    {
                        TournamentRegistrationFields fields = new TournamentRegistrationFields()
                        {
                            AccountID = account.AccountId,
                            TournamentID = viewModel.Model.TournamentID
                        };

                        // Allow the tournament registration to be shown
                        ViewBag.Tournament = viewModel.Model;
                        ViewBag.isRegistered = viewModel.isRegistered(account.AccountId);
                        ViewBag.CanRegister = viewModel.CanRegister();


                        return View("RegisterForm", fields);
                    }
                    else
                    {
                        Session["Message"] = "This tournament is not accepting registrations.";
                        Session["Message.Class"] = ViewModel.ViewError.WARNING;
                    }
                }
                else
                {
                    // Verify if the user is allowed to view the tournament
                    if (viewModel.Model.PublicViewing || viewModel.Model.InviteCode == inviteCode || isAdmin || isParticipant)
                    {
                        return View("Tournament", viewModel);
                    }
                    else
                    {
                        Session["Message"] = "This tournament is not available to view.";
                        Session["Message.Class"] = ViewModel.ViewError.WARNING;
                    }
                }
            }
            else
            {
                Session["Message"] = "The tournament you're looking for doesn't exist or is not publicly shared.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
            }


            return RedirectToAction("Search", "Tournament");
        }

        // POST: Tournament/Create
        [HttpPost]
        [Route("Tournament/Create")]
        public ActionResult Create(TournamentViewModel viewModel)
        {
            // Verify the user is logged in first
            if (account == null)
            {
                Session["Message"] = "You must login to create a tournament.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                //TODO COmbine Create and AddUser()
                if (viewModel.Create(account.AccountId))
                {
                    if (viewModel.AddUser(account, Permission.TOURNAMENT_CREATOR))
                    {
                        // Show a success message.
                        Session["Message"] = "Your tournament was successfully created.";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    //TODO: This should redirect to the tournament
                    return RedirectToAction("Tournament", "Tournament", new { guid = viewModel.Model.TournamentID });
                }
                else
                {
                    // Show a success message.
                    Session["Message"] = "We were unable to create your account.";
                    Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
                }
            }
            else
            {
                viewModel.error = ViewModel.ViewError.CRITICAL;
                viewModel.message = "Please enter in the required fields listed below.";
            }

            return View("Create", viewModel);
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        [Route("Tournament/Update/{tournamentId}")]
        public ActionResult Update(TournamentViewModel viewModel, int tournamentId)
        {
            if (account != null)
            {
                viewModel.LoadData(tournamentId);

                if (viewModel.IsAdministrator(account.AccountId))
                {
                    if (viewModel.Update(account.AccountId))
                    {
                        Session["Message"] = "Edits to the tournament was successful";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;

                        return RedirectToAction("Tournament", "Tournament", new { guid = viewModel.Model.TournamentID });
                    }
                    else
                    {
                        Session["Message"] = "We were unable to update your tournament. Please try again soon";
                        Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                    }
                }
                else
                {
                    Session["Message"] = "You do not have permission to update this tournament";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                Session["Message"] = "You must login to edit a tournament.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Login", "Account");
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        [Route("Tournament/Register")]
        public ActionResult Register(TournamentRegistrationFields userData)
        {
            if (userData.AccountID == account.AccountId)
            {
                TournamentViewModel viewModel = new TournamentViewModel(userData.TournamentID);

                if (viewModel.AddUser(account, Permission.TOURNAMENT_STANDARD))
                {
                    Session["Message"] = "You have been registered to this tournament";
                    Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "We were unable to add you to the tournament";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                Session["Message"] = "You must login to register for this tournament";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Tournament", "Tournament", new { guid = userData.TournamentID });
        }

        [HttpPost]
        [Route("Tournament/Deregister")]
        public ActionResult Deregister(TournamentRegistrationFields userData)
        {
            if (userData.AccountID == account.AccountId)
            {
                TournamentViewModel viewModel = new TournamentViewModel(userData.TournamentID);
                if (viewModel.RemoveUser(account.AccountId))
                {
                    Session["Message"] = "You have been removed from this tournament.";
                    Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                }
                else
                {
                    Session["Message"] = "We could not remove you from the tournament due to an error.";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                Session["Message"] = "You must login to do this action.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Tournament", "Tournament", new { guid = userData.TournamentID });
        }
    }
}