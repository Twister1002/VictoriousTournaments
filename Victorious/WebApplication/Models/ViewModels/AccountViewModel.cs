using DatabaseLib;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class AccountViewModel : ViewModel
    {
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

        [DataType(DataType.Password)]
        [StringLength(AccountModel.PasswordLength)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        // Email subscriptions
        [Display(Name = "Content Updates")]
        public bool EmailMajorUpdates { get; set; }

        public string SocialID { get; set; }
        public int ProviderID { get; set; }
        public List<SocialProviderModel> Providers { get; set; }
    }
}