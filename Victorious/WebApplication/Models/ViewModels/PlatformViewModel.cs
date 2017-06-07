using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class PlatformViewModel
    {
        [Display(Name = "Platform")]
        [DataType(DataType.Text)]
        public String Platform { get; set; }
    }
}