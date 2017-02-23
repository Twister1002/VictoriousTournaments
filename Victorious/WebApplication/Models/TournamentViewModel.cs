using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class TournamentViewModel
    {
        private List<FakeTournamentModel> tournaments;
        public TournamentViewModel()
        {
            tournaments = new List<FakeTournamentModel>(){
                new FakeTournamentModel { Title="Test 1", IsPublic=true, RegistrationStart=DateTime.Now.AddDays(3) },
                new FakeTournamentModel { Title = "Test 2", IsPublic = true, RegistrationStart = DateTime.Now },
                new FakeTournamentModel { Title = "Test 3", IsPublic = true, RegistrationStart = DateTime.Now.AddDays(5) },
                new FakeTournamentModel { Title = "Test 4", IsPublic = true, RegistrationStart = DateTime.Now.AddDays(10) },
                new FakeTournamentModel { Title = "MaybeNow 5", IsPublic = true, RegistrationStart = DateTime.Now }
            };
        }

        [Required(ErrorMessage = "Title is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Tournament Title")]
        public string Title { get; set; }

        //[Required(ErrorMessage = "What is the maximum players?")]
        //[DataType(DataType.Text)]
        //[Display(Name = "Max Players")]
        //public uint Players { get; set; }
        
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
        
        [Display(Name = "Public")]
        public bool IsPublic { get; set; }

        public List<FakeTournamentModel> Search(String title)
        {
            if (title == null) title = this.Title;

            if (title != null)
            {
                return tournaments.Where(x => x.Title.IndexOf(title, 0, StringComparison.OrdinalIgnoreCase) >= 0).ToList<FakeTournamentModel>();
            }
            else
            {
                return tournaments;
            }
        }
    }

    public class FakeTournamentModel
    {
        public String Title { get; set; }
        public DateTime RegistrationStart { get; set; }
        public bool IsPublic { get; set; }
    }
}