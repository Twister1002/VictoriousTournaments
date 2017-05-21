using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DatabaseLib;

namespace WebApplication.Models
{
    public abstract class AccountFields : ViewModel
    {
        public int AccountId { get; set; } 

        //[Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [StringLength(AccountModel.FirstNameLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [StringLength(AccountModel.LastNameLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        [StringLength(AccountModel.UsernameLength)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        [StringLength(AccountModel.EmailLength)]
        [DataType(DataType.Text)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(AccountModel.PasswordLength)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [StringLength(AccountModel.PasswordLength)]
        [Display(Name = "Verify Password")]
        public string PasswordVerify { get; set; }

        public abstract void ApplyChanges();
        public abstract void SetFields();
    }

    public abstract class TournamentFields : ViewModel
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

        public List<BracketTypeModel> BracketTypes { get; protected set; }
        public List<GameTypeModel> GameTypes { get; protected set; }
        public List<PlatformModel> PlatformTypes { get; protected set; }
        public List<TournamentUserModel> Users { get; set; }

        [Display(Name = "Bracket Type")]
        [Required(ErrorMessage ="Select a bracket type")]
        public int BracketType { get; set; }

        [Display(Name ="Game")]
        [Required(ErrorMessage ="Select a game")]
        public int? GameType { get; set; }

        [Display(Name = "Platform")]
        [Required(ErrorMessage = "Choose a platform")]
        public int PlatformType { get; set; } 

        [Display(Name = "Public Viewing")]
        public bool PublicViewing { get; set; }
        [Display(Name = "Public Registation")]
        public bool PublicRegistration { get; set; }

        public abstract void ApplyChanges();
        public abstract void SetFields();
    }

    public abstract class BracketFields : ViewModel
    {
        [Display(Name = "Best Of ")]
        public int BestOfMatches { get; set; }
    }

    public abstract class MatchFields : ViewModel
    {
    } 

    public abstract class GameFields : ViewModel
    {
        public int GameNumber { get; set; }
        public int ChallengerScore { get; set; }
        public int DefenderScore { get; set; }
    }

    public class TournamentRegistrationFields : ViewModel
    {
        public String Name { get; set; }
        public int TournamentID { get; set; }
        public int AccountID { get; set; }
    }

    public abstract class GameTypeFields : ViewModel
    {
        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public String Title { get; set; }
    }

    public abstract class PlatformTypeFields : ViewModel
    {
        [Display(Name = "Platform")]
        [DataType(DataType.Text)]
        public String Platform { get; set; }
    }
}