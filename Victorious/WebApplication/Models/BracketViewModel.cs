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

        public BracketViewModel(int id)
        {
            Model = db.GetBracketByID(id);
            switch (Model.BracketType.Type)
            {
                case BracketTypeModel.BracketType.SINGLE:
                    Bracket = new SingleElimBracket(Model);
                    break;
                case BracketTypeModel.BracketType.DOUBLE:
                    Bracket = new DoubleElimBracket(Model);
                    break;
                case BracketTypeModel.BracketType.ROUNDROBIN:
                    Bracket = new RoundRobinBracket(Model);
                    break;
            }
        }

        public bool ResetBracket()
        {
            DbError result = DbError.SUCCESS;

            // Delete the games first
            for (int i = 1; i <= Bracket.NumberOfMatches; i++)
            {
                IMatch match = Bracket.GetMatch(i);

                // Delete every game associated with this bracket
                foreach (IGame game in match.Games)
                {
                    if (result == DbError.SUCCESS)
                    {
                        result = db.DeleteGame(match.GetModel(), game.GetModel());
                    }
                }
            }

            // Reset the bracket
            Bracket.ResetMatches();

            // Update every match with this bracket
            for (int i = 1; i <= Bracket.NumberOfMatches; i++)
            {
                if (result == DbError.SUCCESS)
                {
                    IMatch match = Bracket.GetMatch(i);
                    result = db.UpdateMatch(match.GetModel());
                }
                else
                {
                    break;
                }
            }

            return result == DbError.SUCCESS;
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