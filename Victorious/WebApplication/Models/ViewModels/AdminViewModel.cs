using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.ViewModels
{
    public class AdminViewModel
    {
        [Display(Name = "Platform")]
        [DataType(DataType.Text)]
        public String Platform { get; set; }

        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public String Title { get; set; }
    }
}