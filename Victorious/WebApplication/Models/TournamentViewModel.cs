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
        public TournamentModel tournament;

        public TournamentViewModel(int modelId)
        {
            tournament = db.GetTournamentById(modelId);

            tourny = new Tournament.Structure.Tournament();
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 1; i <= 15; i++)
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
            tourny.Brackets[0].AddWin(5, PlayerSlot.Challenger);
            tourny.Brackets[0].AddWin(2, PlayerSlot.Challenger);
            tourny.Brackets[0].AddWin(3, PlayerSlot.Defender);
            tourny.Brackets[0].AddWin(7, PlayerSlot.Challenger);
        }
        
    }
}