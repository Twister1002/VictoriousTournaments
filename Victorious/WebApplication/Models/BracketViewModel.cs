using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class BracketViewModel : BracketFields
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

        public int TotalRounds()
        {
            if (Bracket.GrandFinal != null)
            {
                return Bracket.NumberOfRounds + 1;
            }
            else
            {
                return Bracket.NumberOfRounds;
            }
        }

        public List<IMatch> UpperMatches(int round)
        {
            return Bracket.GetRound(round);
        }

        public List<IMatch> LowerMatches(int round)
        {
            return Bracket.GetLowerRound(round);
        }
    }
}