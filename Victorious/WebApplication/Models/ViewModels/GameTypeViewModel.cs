using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.ViewModels
{
    public class GameTypeViewModel : ViewModel
    {
        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public String Title { get; set; }

        public int GameID { get; set; }
    }
}