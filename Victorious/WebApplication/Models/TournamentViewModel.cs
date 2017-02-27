using DataLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class TournamentViewModel
    {
        public DbError Exception = DbError.SUCCESS;

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
}