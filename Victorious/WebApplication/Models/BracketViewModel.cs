using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;

namespace WebApplication.Models
{
    public class BracketViewModel : BracketFields
    {
        public struct RoundData
        {
            public int roundNum;
            public int bestOf;
        };

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
            Model = db.GetBracket(id);

            switch (Model.BracketType.Type)
            {
                case BracketTypes.SINGLE:
                    Bracket = new SingleElimBracket(Model);
                    break;
                case BracketTypes.DOUBLE:
                    Bracket = new DoubleElimBracket(Model);
                    break;
                case BracketTypes.ROUNDROBIN:
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
                        result = db.DeleteGame(game.Id);
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
                    db.UpdateMatch(Bracket.GetMatch(i).GetModel());
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

        public List<RoundData> RoundInfo()
        {
            List<RoundData> data = new List<RoundData>();
            int totalRounds = TotalRounds();

            for (int i = 1; i <= totalRounds; i++)
            {
                RoundData? roundInfo = null;

                if (Bracket.GetRound(i).Count > 0)
                {
                    roundInfo = new RoundData() { roundNum = i, bestOf = Bracket.GetRound(i)[0].MaxGames };
                }
                else if (i == totalRounds && Bracket.GrandFinal != null)
                {
                    roundInfo = new RoundData() { roundNum = i, bestOf = Bracket.GrandFinal.MaxGames };
                }

                if (roundInfo == null)
                {
                    // Check the lower rounds;
                    if (Bracket.GetLowerRound(i).Count > 0)
                    {
                        roundInfo = new RoundData() { roundNum = i, bestOf = Bracket.GetLowerRound(i)[0].MaxGames };
                    }
                }

                data.Add((RoundData)roundInfo);
            }

            return data;
        }

        public List<int> MatchesAffectedList(int matchNum)
        {
            List<int> matchesAffected = new List<int>();
            IMatch head = Bracket.GetMatch(matchNum);
            matchesAffected.Add(matchNum);
            
            if (head.NextMatchNumber != -1)
            {
                List<int> matches = MatchesAffectedList(head.NextMatchNumber);

                foreach (int match in matches) {
                    if (!matchesAffected.Exists((i) => i == match))
                    {
                        matchesAffected.Add(match);
                    }
                }
            }

            if (head.NextLoserMatchNumber != -1)
            {
                List<int> matches = MatchesAffectedList(head.NextLoserMatchNumber);

                foreach (int match in matches)
                {
                    if (!matchesAffected.Exists((i) => i == match))
                    {
                        matchesAffected.Add(match);
                    }
                }
            }

            return matchesAffected;
        }

        public Permission TournamentPermission(int accountId)
        {
            return Model.Tournament.TournamentUsers.First(x => x.AccountID == accountId).Permission;
        }
    }
}