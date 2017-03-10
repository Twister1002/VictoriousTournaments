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
        [Required(ErrorMessage = "Title is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Tournament Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "When do we allow registration to start?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Registration Start")]
        public DateTime RegistrationStart { get; set; }

        [Required(ErrorMessage = "When does registration end?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Registration End")]
        public DateTime RegistrationEnd { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Check-in Date and Time")]
        public DateTime CheckInDateTime { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Tournament Description")]
        public String Description { get; set; }

        [Display(Name = "Public")]
        public bool IsPublic { get; set; }
    }

    public class TournamentSearchViewModel : ViewModel
    {
        private List<TournamentModel> models = new List<TournamentModel>();

        public void AddTournament(TournamentModel model)
        {
            models.Add(model);
        }

        public void AddTournaments(List<TournamentModel> models)
        {
            this.models = models;
        }
    }

    public class TournamentViewModel : ViewModel {
        public ITournament tourny;

        public TournamentViewModel()
        {
            tourny = new Tournament.Structure.Tournament();
            for (int i = 1; i <= 8; i++)
            {
                UserModel uModel = new UserModel()
                {
                    UserID = i,
                    FirstName = "FirstName " + i,
                    LastName = "LastName " + i,
                    Username = "Player " + i,
                    Email = "EMail" + i
                };

                tourny.AddPlayer(new User(uModel));
            }

            tourny.CreateSingleElimBracket();
            tourny.Brackets[0].AddWin(1, 0);
            tourny.Brackets[0].AddWin(2, 1);
            tourny.Brackets[0].AddWin(5, 0);
        }
        
    }
}