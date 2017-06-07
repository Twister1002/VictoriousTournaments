using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class PlatformTypeViewModel
    {
        [Display(Name = "Platform")]
        [DataType(DataType.Text)]
        public String Platform { get; set; }
    }
}