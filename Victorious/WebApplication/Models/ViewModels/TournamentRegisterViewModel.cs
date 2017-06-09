using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.ViewModels
{
    public class TournamentRegisterViewModel
    {
        public String Name { get; set; }
        public int TournamentID { get; set; }
        public int AccountID { get; set; }
    }
}