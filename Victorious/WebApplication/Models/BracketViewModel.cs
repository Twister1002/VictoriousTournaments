using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class BracketViewModel : ViewModel
    {
        public IBracket Bracket { get; private set; }
        public BracketModel Model { get; private set; }

        public BracketViewModel()
        {

        }

        public BracketViewModel(IBracket bracket)
        {
            Bracket = bracket;
        }
    }
}