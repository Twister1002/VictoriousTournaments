using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using DataLib;
using Tournament.Structure;
using Newtonsoft.Json;

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

                if (viewModel.UserPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
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
                DbError result = db.AddTournament(model);

                if (result == DbError.SUCCESS)
                {
                    // Lets now Register the user as an administrator
                    //UserInTournamentModel userInModel = new UserInTournamentModel()
                    //{
                    //     UserID = (int)Session["User.UserId"],
                    //     TournamentID = 
                    //}
                    DbError adminResult = db.AddUserToTournament(model, db.GetUserById((int)Session["User.UserId"]), Permission.TOURNAMENT_ADMINISTRATOR);
                    if (adminResult == DbError.SUCCESS)
                    {
                        // Show a success message.
                        Session["Message"] = "Your tournament was successfully created.";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    else
                    {
                        // Create some log about this.
                    }

                    return RedirectToAction("Tournament", "Tournament", new { guid = model.TournamentID });
                }
                else
                {
                    // The tournament failed to be created
                    viewModel.dbException = db.interfaceException;
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
                if (viewModel.UserPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
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
                        viewModel.dbException = db.interfaceException;
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
                    int userCount = viewModel.Model.UsersInTournament.Count(x => x.UserID == (int)Session["User.UserId"]);

                    if (userCount == 0)
                    {
                        // Add the user to the tournament
                        DbError error = db.AddUserToTournament(new UserInTournamentModel()
                        {
                            TournamentID = viewModel.Model.TournamentID,
                            UserID = (int)Session["User.UserId"],
                            Permission = Permission.TOURNAMENT_STANDARD
                        });

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
        [Route("Tournament/Deregister")]
        public ActionResult Deregister(String tournamentVal)
        {
            int tournamentId = -1;
            if (int.TryParse(tournamentVal, out tournamentId))
            {
                if (Session["User.UserId"] != null)
                {
                    // We have a user logged in.
                    TournamentViewModel viewModel = new TournamentViewModel(tournamentId);
                    UserModel userModel = viewModel.Model.UsersInTournament.First(x => x.UserID == (int)Session["User.UserId"]).User;
                    DbError result = db.RemoveUserFromTournament(viewModel.Model, userModel);
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

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Tournament/Ajax/Finalize")]
        public JsonResult Finalize(String jsonData)
        {
            dynamic jsonResult = new { status=false, message="Not yet supported", redirect="" };
            Dictionary<String, dynamic> json = JsonConvert.DeserializeObject<Dictionary<String, dynamic>>(jsonData);
            

            if (Session["User.UserId"] != null)
            {
                // Load the tournament
                TournamentViewModel viewModel = new TournamentViewModel(ConvertToInt(json["tournyVal"]));
                if (viewModel.UserPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
                {
                    // Set the round best of matches


                    DbError result = viewModel.FinalizeTournament();

                    if (result == DbError.SUCCESS)
                    {
                        Session["Message"] = "Your tournament has been finalized. No changes can be made.";
                        Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    }
                    else
                    {
                        Session["Message"] = "An error occurred while trying to create the matches.<br/>" + viewModel.dbException.Message;
                        Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
                    }
                }
                else
                {
                    Session["Message"] = "You are not permitted to do that.";
                    Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                }
            }
            else
            {
                Session["Message"] = "You must login before you can do that.";
                Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
                return RedirectToAction("Login", "Account");
            }

            return Json(JsonConvert.SerializeObject(jsonResult));
        }

        //[HttpPost]
        //[Route("Tournament/Finalize")]
        //public ActionResult Finalize(String tourny)
        //{
        //    if (Session["User.UserId"] != null)
        //    {
        //        int tournyId = this.ConvertToInt(tourny);
        //        TournamentViewModel viewModel = new TournamentViewModel(tournyId);
        //        if (viewModel.UserPermission((int)Session["User.UserId"]) == Permission.TOURNAMENT_ADMINISTRATOR)
        //        {
        //            DbError result = viewModel.FinalizeTournament();

        //            if (result == DbError.SUCCESS)
        //            {
        //                Session["Message"] = "Your tournament has been finalized. No changes can be made.";
        //                Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
        //            }
        //            else
        //            {
        //                Session["Message"] = "An error occurred while trying to create the matches.<br/>" + viewModel.dbException.Message;
        //                Session["Message.Class"] = ViewModel.ViewError.CRITICAL;
        //            }
        //        }
        //        else
        //        {
        //            Session["Message"] = "You are not permitted to do that.";
        //            Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
        //        }
        //    }
        //    else
        //    {
        //        Session["Message"] = "You must login before you can do that.";
        //        Session["Message.Class"] = ViewModel.ViewError.EXCEPTION;
        //        return RedirectToAction("Login", "Account");
        //    }

        //    // Create the matches
        //    return RedirectToAction("Tournament", "Tournament", new { @guid = tourny });
        //}

        [HttpPost]
        [Route("Tournament/Ajax/Delete")]
        public JsonResult Delete(String jsonData)
        {
            dynamic jsonResult = new { };
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            if (Session["User.UserId"] != null)
            {
                UserModel userModel = this.getUserModel((int)Session["User.UserId"]);
                TournamentViewModel model = new TournamentViewModel(json["tourny"]);
                if (model.Model.CreatedByID == userModel.UserID)
                {
                    DbError result = db.DeleteTournament(model.Model);
                    if (result == DbError.SUCCESS)
                    {
                        jsonResult = new { status = true, message = "Tournament was deleted.", redirect = Url.Action("Index", "Tournament") };
                    }
                    else
                    {
                        jsonResult = new { status = false, message = "Unable to delete the tournament due to an error." };
                    }
                }
                else
                {
                    jsonResult = new { status = false, message = "You are not entitled to do this." };
                }
            }
            else
            {
                jsonResult = new { status = false, message = "Please login in order to modify a tournament." };
            }

            return Json(JsonConvert.SerializeObject(jsonResult));
        }

        [HttpPost]
        [Route("Tournament/Ajax/PermissionChange")]
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

        [HttpPost]
        [Route("Tournament/Ajax/Standings")]
        public JsonResult Standings(String jsonData)
        {
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<String, int>>(jsonData);
            TournamentViewModel viewModel = new TournamentViewModel(json["tournamentId"]);
            viewModel.ProcessTournament();
            IBracket bracket = viewModel.Tourny.Brackets[json["bracketNum"]];

            return Json(JsonConvert.SerializeObject(
                new
                {
                    status = true,
                    data = new
                    {
                        ranks = bracket.Rankings
                    }
                }
            ));
        }

        [HttpPost]
        [Route("Tournament/Ajax/Reset")]
        public JsonResult Reset(String jsonData)
        {
            dynamic jsonResult = new { };
            Dictionary<String, int> json = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            if (Session["User.UserId"] != null)
            {
                TournamentViewModel viewModel = new TournamentViewModel(json["tournyNum"]);
                bool result = viewModel.ResetBrackets((int)Session["User.UserId"]);

                if (result)
                {
                    Session["Message"] = "The bracket has been reset.";
                    Session["Message.Class"] = ViewModel.ViewError.SUCCESS;
                    jsonResult = new
                    {
                        status = true,
                        redirect = Url.Action("Tournament", "Tournament", new { guid = viewModel.Model.TournamentID })
                    };
                }
                else
                {
                    Session["Message"] = "The bracket failed to reset.";
                    Session["Message.Class"] = ViewModel.ViewError.WARNING;
                    jsonResult = new
                    {
                        status = true
                    };
                }
            }
            else
            {
                jsonResult = new
                {
                    status = false,
                    redirect = Url.Action("Login", "Account")
                };
            }

            return Json(JsonConvert.SerializeObject(jsonResult));
        }
    }
}