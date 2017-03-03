using DataLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{

    public class AccountViewModel
    {
        public DbError Exception = DbError.NONE;
        public UserModel userModel { get; private set; }
        public String ErrorMessage { get; set; }

        public AccountViewModel(UserModel model)
        {
            userModel = model;
        }
    }

    public class AccountLoginViewModel
    {
        public DbError Exception = DbError.NONE;
        public String ErrorMessage { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.UsernameLength)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(UserModel.PasswordLength)]
        [Display(Name = "Password")]
        public string Password { get; set; }


    }

    public class AccountRegisterViewModel
    {
        public DbError Exception = DbError.NONE;
        public String ErrorMessage { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.FirstNameLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.LastNameLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        [StringLength(UserModel.UsernameLength)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(UserModel.EmailLength)]
        [DataType(DataType.Text)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(UserModel.PasswordLength)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [StringLength(UserModel.PasswordLength)]
        [Display(Name = "Verify Password")]
        public string PasswordVerify { get; set; }
    }
}