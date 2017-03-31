using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataLib;

namespace WebApplication.Models
{
    public abstract class AccountFields : ViewModel
    {
        //[Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.FirstNameLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.LastNameLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.UsernameLength)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        [StringLength(UserModel.EmailLength)]
        [DataType(DataType.Text)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(UserModel.PasswordLength)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [StringLength(UserModel.PasswordLength)]
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
        
        public List<BracketTypeModel> BracketTypes { get; protected set; }

        [Display(Name = "Bracket Type")]
        [Required(ErrorMessage ="Select a bracket type")]
        public int BracketType { get; set; }

        [Display(Name = "Public")]
        public bool IsPublic { get; set; }

        public abstract void ApplyChanges(int userId);
        public abstract void SetFields();
    }

    public abstract class MatchFields : ViewModel {
        [DataType(DataType.Text)]
        public int? ChallengerScore { get; set; }
        
        [DataType(DataType.Text)]
        public int? DefenderScore { get; set; }

        [DataType(DataType.Text)]
        public int? WinnerID { get; set; }

        public abstract void ApplyChanges(int userId);
        public abstract void SetFields();
    } 

    public abstract class AdministratorFields : ViewModel
    {

    }
}