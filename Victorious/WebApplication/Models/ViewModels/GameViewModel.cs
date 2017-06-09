using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.ViewModels
{
    public class GameViewModel
    {
        public int GameNumber { get; set; }
        public int ChallengerScore { get; set; }
        public int DefenderScore { get; set; }
    }
}