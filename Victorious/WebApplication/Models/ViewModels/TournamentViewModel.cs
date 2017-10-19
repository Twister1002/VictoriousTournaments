using DatabaseLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class TournamentViewModel : ViewModel
    {
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
        public DateTime RegistrationStartDate { get; set; }

        [Required(ErrorMessage = "When will registration end?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Registration End")]
        public DateTime RegistrationEndDate { get; set; }

        [Required(ErrorMessage = "When will the tournament start?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Tournament Start")]
        public DateTime TournamentStartDate { get; set; }

        [Required(ErrorMessage = "When will the tournament end?")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Tournament End")]
        public DateTime TournamentEndDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "CheckIn Start")]
        public DateTime CheckinStartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "CheckIn Ends")]
        public DateTime CheckinEndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime RegistrationStartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime RegistrationEndTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime TournamentStartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime TournamentEndTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime CheckinStartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "hh:mm tt")]
        public DateTime CheckinEndTime { get; set; }
        
        public List<GameTypeModel> GameTypes { get; set; }
        public List<PlatformModel> PlatformTypes { get; set; }

        public List<BracketViewModel> BracketData { get; set; }

        [Display(Name = "Game")]
        [Required(ErrorMessage = "Select a game")]
        public int GameTypeID { get; set; }

        [Display(Name = "Platform")]
        [Required(ErrorMessage = "Choose a platform")]
        public int PlatformID { get; set; }

        [Display(Name = "Public Viewing")]
        public bool PublicViewing { get; set; }
        [Display(Name = "Public Registation")]
        public bool PublicRegistration { get; set; }

        // UserInTournament
        public List<TournamentUserModel> Participants { get; set; }
        public Dictionary<int, String> Permissions { get; set; }


        // Bracket stuff
        [Display(Name = "Best Of ")]
        public int BestOfMatches { get; set; }
        [Display(Name = "Bracket Type")]
        public List<BracketTypeModel> BracketTypes { get; set; }
        [Display(Name = "Rounds")]
        public List<int> NumberOfRounds { get; set; }
        [Display(Name = "Groups")]
        public List<int> NumberOfGroups { get; set; }
        [Display(Name = "Number of player to advance")]
        public List<int> NumberPlayersAdvance { get; set; }
    }
}