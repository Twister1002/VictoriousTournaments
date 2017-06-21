using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.ViewModels
{
    public class BracketViewModel : ViewModel
    {
        public int BracketTypeID { get; set; }
        public int NumberOfRounds { get; set; }
        public int NumberOfGroups { get; set; }
        public int NumberPlayersAdvance { get; set; }
    }
}