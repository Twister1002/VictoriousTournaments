using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class PlatformViewModel : ViewModel
    {
        [Display(Name = "Platform")]
        [DataType(DataType.Text)]
        public String Platform { get; set; }

        public int PlatformID { get; set; }
    }
}