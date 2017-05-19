using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Models;
using Newtonsoft.Json;
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

        [HttpPost]
        [Route("Ajax/Tournament/Search")]
        public JsonResult AjaxSearch(String searchData)
        {
            Dictionary<String, String> searchBy = JsonConvert.DeserializeObject<Dictionary<String, String>>(searchData);
            TournamentViewModel viewModel = new TournamentViewModel();
            List<object> dataReturned = new List<object>();

            viewModel.Search(searchBy);
            foreach (TournamentModel tourny in viewModel.SearchedTournaments)
            {
                dataReturned.Add(new
                {
                    id = tourny.TournamentID,
                    title = tourny.Title,
                    game = tourny.GameType.Title,
                    platform = tourny.Platform != null ? tourny.Platform.PlatformName : "None",
                    startDate = tourny.TournamentStartDate.ToShortDateString(),
                    publicRegistration = tourny.PublicRegistration,
                    publicViewing = tourny.PublicViewing,
                    link = Url.Action("Tournament", "Tournament", new { guid = tourny.TournamentID })
                });
            }

            return Json(JsonConvert.SerializeObject(dataReturned));
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
        [Route("Tournament/Update")]
        public ActionResult Update(int tournamentId)
        {
            LoadAccount(Session);
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
            LoadAccount(Session);
            int tournamentId = ConvertToInt(guid);
            TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
            bool isAdmin = viewModel.IsAdministrator(account.AccountId);
            bool isParticipant = viewModel.IsParticipant(account.AccountId);

            if (viewModel.Model != null)
            {
                // Should we check for registrations or view the tournament?
                if (!viewModel.Model.InProgress && !isAdmin)
                {
                    // Verify if the user has an invite code or the invite code is valid
                    if (viewModel.PublicRegistration || viewModel.Model.InviteCode == inviteCode)
                    {
                        // Allow the tournament registration to be shown
                        ViewBag.Tournament = viewModel.Model;
                        ViewBag.isRegistered = viewModel.isRegistered(account.AccountId);
                        TournamentRegistrationFields fields = new TournamentRegistrationFields()
                        {
                            AccountID = account.AccountId,
                            TournamentID = viewModel.Model.TournamentID
                        };

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
            if (Session["User.UserId"] == null)
            {
                Session["Message"] = "You must login to create a tournament.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                if (viewModel.Create((int)Session["User.UserId"]))
                {
                    if (viewModel.AddUser((int)Session["User.UserId"], Permission.TOURNAMENT_CREATOR))
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
        [Route("Tournament/Update")]
        public ActionResult Update(TournamentViewModel viewModel, int tournamentId)
        {
            LoadAccount(Session);
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
            LoadAccount(Session);
            
            if (userData.AccountID == account.AccountId)
            {
                TournamentViewModel viewModel = new TournamentViewModel(userData.TournamentID);

                if (viewModel.AddUser(account.AccountId, Permission.TOURNAMENT_STANDARD))
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
            LoadAccount(Session);
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

        [HttpPost]
        [Route("Ajax/Tournament/Deregister")]
        public JsonResult NoAccountDeRegister(TournamentRegistrationFields userData)
        {
            LoadAccount(Session);
            TournamentViewModel viewModel = new TournamentViewModel(userData.TournamentID);
            bool status = false;
            object data = new { };
            String message = "No action taken";

            // Is an Administrator registering a user?
            if (viewModel.IsAdministrator(account.AccountId))
            {
                status = viewModel.RemoveUser(userData.Name);
                message = "User was " + (status ? "" : "not") + " removed successfully";
            }
            else
            {
                message = "Could not add user to tournament";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                data = data
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/CheckIn")]
        public JsonResult CheckIn(int tournamentId, int tournamentUserId = -1)
        {
            LoadAccount(Session);
            bool status = false;
            bool isCheckedIn = false;
            String message = "No action taken";


            if (tournamentUserId != -1)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.UserCheckIn(tournamentUserId))
                {
                    status = true;
                    message = "User is checkedin";
                }
                else
                {
                    message = "User was not checked in";
                }

                isCheckedIn = viewModel.isUserCheckedIn(tournamentUserId);
            }
            else if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.AccountCheckIn(account.AccountId))
                {
                    status = true;
                    message = "User is checkedin";
                }
                else
                {
                    message = "User was not checked in";
                }
            }
            else
            {
                message = "Account not logged in";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status = status,
                message = message,
                isCheckedIn = isCheckedIn
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/Finalize")]
        public JsonResult Finalize(String jsonData, Dictionary<String, Dictionary<String, int>> roundData)
        {
            LoadAccount(Session);
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            bool status = false;
            String message = "No action was taken";
            String redirect = redirect = Url.Action("Tournament", "Tournament", new { guid = json["tournyVal"] });
            
            if (account != null)
            {
                // Load the tournament
                TournamentViewModel viewModel = new TournamentViewModel(json["tournyVal"]);
                if (viewModel.IsAdministrator((int)Session["User.UserId"]))
                {
                    if (viewModel.FinalizeTournament(roundData))
                    {
                        status = true;
                        message = "Your tournament has been finalized. No changes can be made.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    else
                    {
                        message = "An error occurred while trying to create the matches.";

                        Session["Message"] = message;
                        Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
                    }
                }
                else
                {
                    message = "You are not permitted to do that.";

                    Session["Message"] = message;
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                message = "You must login to do that.";
                redirect = Url.Action("Login", "Account");

                Session["Message"] = message;
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
            }

            return Json(JsonConvert.SerializeObject(new { status = status, message = message, redirect = redirect }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/Delete")]
        public JsonResult Delete(int tournamentId)
        {
            bool status = false;
            String message = "No action taken";
            String redirect = Url.Action("Tournament", "Tournament", new { guid = tournamentId });

            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.IsCreator((int)Session["User.UserId"]))
                {
                    if (viewModel.Delete())
                    {
                        status = true;
                        message = "Tournament was deleted.";
                        redirect = Url.Action("Index", "Tournament");
                    }
                    else
                    {
                        status = false;
                        message = "Unable to delete the tournament due to an error.";
                    }
                }
                else
                {
                    status = false;
                    message = "You do not have permission to do this.";
                }
            }
            else
            {
                status = false;
                message = "Please login in order to modify a tournament.";
            }

            return Json(JsonConvert.SerializeObject(new
            {
                status,
                message,
                redirect
            }));
        }

        [HttpPost]
        [Route("Ajax/Tournament/PermissionChange")]
        public JsonResult PermissionChange(int TournamentId, int targetUser, String action)
        {
            LoadAccount(Session);
            if (account != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(TournamentId);

                object permissionChange = viewModel.ChangePermission(account.Account, targetUser, action);

                return Json(JsonConvert.SerializeObject(permissionChange));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    status = false,
                    message = "You must be logged in to do this action"
                }));
            }
        }
    }
}