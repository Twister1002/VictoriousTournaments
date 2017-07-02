using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.ViewModels
{
    public class ContactViewModel
    {

        [DataType("Text")]
        [Required(ErrorMessage = "What is your email address.")]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [DataType("Text")]
        [Required(ErrorMessage = "Please tell us your name.")]
        [Display(Name = "Name")]
        public String Name { get; set; }

        [DataType("Text")]
        [Required(ErrorMessage = "Summary of what you want to say.")]
        [Display(Name = "Subject")]
        public String Subject { get; set; }

        [DataType("Text")]
        [Required(ErrorMessage = "What's going on?")]
        [Display(Name = "Body")]
        public String Body { get; set; }
    }
}