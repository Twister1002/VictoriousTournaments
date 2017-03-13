using DataLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class TournamentFormModel : ViewModel
    {
        public TournamentModel model { get; private set; }

        public TournamentFormModel()
        {

        }

        public TournamentFormModel(TournamentModel model)
        {
            this.model = model;

            Title = model.Title;
            Description = model.Description;
            IsPublic = model.TournamentRules.IsPublic == null ? true : (bool)model.TournamentRules.IsPublic;
            RegistrationStartDate = model.TournamentRules.RegistrationStartDate;
            RegistrationEndDate = model.TournamentRules.RegistrationEndDate;
            TournamentStartDate = model.TournamentRules.TournamentStartDate;
            TournamentEndDate = model.TournamentRules.TournamentEndDate;
        }

        public void ApplyChanges()
        {
            ApplyChanges(this.model);
        }

        public TournamentModel ApplyChanges(TournamentModel model)
        {
            if (model.TournamentRules == null)
            {
                model.TournamentRules = new TournamentRuleModel();
            }

            model.Title = Title;
            model.Description = Description;
            model.TournamentRules.IsPublic = IsPublic;
            model.TournamentRules.RegistrationStartDate = RegistrationStartDate;
            model.TournamentRules.RegistrationEndDate = RegistrationEndDate;
            model.TournamentRules.TournamentStartDate = TournamentStartDate;
            model.TournamentRules.TournamentEndDate = TournamentEndDate;

            return model;
        }

        [Required(ErrorMessage = "Name your tournament")]
        [DataType(DataType.Text)]
        [Display(Name = "Tournament Title")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Tournament Description")]
        public String Description { get; set; }

        [Required(ErrorMessage = "When will registration start?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Registration Start")]
        public DateTime? RegistrationStartDate { get; set; }

        [Required(ErrorMessage = "When will registration end?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Registration End")]
        public DateTime? RegistrationEndDate { get; set; }

        [Required(ErrorMessage = "When will the tournament start?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Tournament Start")]
        public DateTime? TournamentStartDate { get; set; }

        [Required(ErrorMessage = "When will the tournament end?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Tournament End")]
        public DateTime? TournamentEndDate { get; set; }

        //[DataType(DataType.Text)]
        //[Display(Name = "Check-in Date")]
        //public DateTime? CheckInDateTime { get; set; }
        
        [Display(Name = "Public")]
        public bool IsPublic { get; set; }
    }

    public class TournamentSearchViewModel : ViewModel
    {
        public List<TournamentModel> models { get; private set; }
        
        public TournamentSearchViewModel()
        {
            models = db.GetAllTournaments();
        }

        public List<TournamentModel> Search(String title)
        {
            return models.Where(m => m.Title == title).ToList();
        }
    }

    public class TournamentViewModel : ViewModel {
        public ITournament tourny;
        public TournamentModel model;

        public TournamentViewModel(TournamentModel model)
        {
            // We need to grab data about this tournament and feed it
            // Into the tournament class so we can get proper rendering
            this.model = model;

            tourny = new Tournament.Structure.Tournament();
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 1; i <= 10; i++)
            {
                UserModel uModel = new UserModel()
                {
                    UserID = i,
                    FirstName = "FirstName " + i,
                    LastName = "LastName " + i,
                    Username = "Player " + i,
                    Email = "Email" + i
                };

                players.Add(new User(uModel));
            }

            tourny.AddSingleElimBracket(players);
            tourny.Brackets[0].AddWin(1, PlayerSlot.Challenger);
            tourny.Brackets[0].AddWin(2, PlayerSlot.Challenger);
            tourny.Brackets[0].AddWin(3, PlayerSlot.Challenger);
            //tourny.Brackets[0].AddWin(7, PlayerSlot.Challenger);
        }
        
    }
}