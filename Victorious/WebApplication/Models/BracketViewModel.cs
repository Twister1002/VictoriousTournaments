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
            Model = null;
        }

        public int TotalRounds()
        {
            if (Bracket.GrandFinal != null)
            {
                return Math.Max(Bracket.NumberOfRounds, Bracket.NumberOfLowerRounds) + 1;
            }
            else
            {
                return Math.Max(Bracket.NumberOfRounds, Bracket.NumberOfLowerRounds);
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