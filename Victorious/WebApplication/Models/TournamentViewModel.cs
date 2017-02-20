using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class TournamentViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Tornament Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Set the maximum amount of players")]
        [DataType(DataType.Text)]
        [Display(Name = "Max Players")]
        public string Players { get; set; }
    }
}