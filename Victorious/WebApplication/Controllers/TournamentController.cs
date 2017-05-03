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
        [Route("Tournament")]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        // Tournament Search
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
                    startDate = tourny.TournamentStartDate.ToShortDateString(),
                    isPublic = tourny.IsPublic,
                    link = Url.Action("Tournament", "Tournament", new { guid = tourny.TournamentID })
                });
            }

            return Json(JsonConvert.SerializeObject(dataReturned));
        }

        // Tournament Info
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid)
        {
            TournamentViewModel viewModel = new TournamentViewModel(guid);
            viewModel.ProcessTournament();

            if (viewModel.Model != null)
            {
                return View("Tournament", viewModel);
            }
            else
            {
                Session["Message"] = "The tournament you're looking for doesn't exist or is not publicly shared.";
                Session["Message.Class"] = ViewModel.ViewError.WARNING;
            }

            return RedirectToAction("Search", "Tournament");
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
                TournamentViewModel viewModel = new TournamentViewModel(id);
                if (viewModel.IsAdministrator((int)Session["User.UserId"]))
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
                    return RedirectToAction("Index", "Account");
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
        [Route("Tournament/Update/{id}")]
        public ActionResult Update(TournamentViewModel viewModel, int id)
        {
            if (Session["User.UserId"] != null)
            {
                viewModel.LoadData(id);

                if (viewModel.IsAdministrator((int)Session["User.UserId"]))
                {
                    if (viewModel.Update((int)Session["User.UserId"]))
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
        [Route("Tournament/Register/{tournamentVal}")]
        public ActionResult Register(int tournamentId)
        {
            if (Session["User.UserId"] != null)
            {
                // Verify the user doesn't exist in the tournament all ready
                TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                if (viewModel.AddUser((int)Session["User.UserId"], Permission.TOURNAMENT_STANDARD))
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

            return RedirectToAction("Tournament", "Tournament", new { guid = tournamentId });
        }

        [HttpPost]
        [Route("Tournament/Deregister")]
        public ActionResult Deregister(String tournamentVal)
        {
            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(tournamentVal);
                if (viewModel.RemoveUser((int)Session["User.UserId"]))
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
                Session["Message"] = "You must login to do this action.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Index", "Account");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Ajax/Tournament/Finalize")]
        public JsonResult Finalize(String jsonData, Dictionary<String, int> roundData)
        {
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            bool status = false;
            String message = "No action was taken";
            String redirect = redirect = Url.Action("Tournament", "Tournament", new { guid = json["tournyVal"] });

            if (Session["User.UserId"] != null)
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
                        message = "An error occurred while trying to create the matches.<br/>" + viewModel.dbException.Message;

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
        public JsonResult PermissionChange(String jsonData)
        {
            Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(ConvertToInt(json["tournyVal"]));

                Dictionary<String, dynamic> permissionChange = viewModel.ChangePermission((int)Session["User.UserId"], ConvertToInt(json["userVal"]), json["action"]);

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