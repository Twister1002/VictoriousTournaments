using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;

namespace WebApplication.Models
{
    public enum BracketSection
    {
        UPPER,
        LOWER
    }

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
            Bracket = null;
            Model = null;
        }

        public BracketViewModel(BracketModel model)
        {
            Model = model;
            LoadBracket();
        }

        public BracketViewModel(IBracket bracket)
        {
            Bracket = bracket;
            Model = null;
        }

        public BracketViewModel(int id)
        {
            Model = db.GetBracket(id);
            LoadBracket();
        }

        private void LoadBracket()
        {
            if (Model != null)
            {
                if (Model.Finalized)
                {
                    switch (Model.BracketType.Type)
                    {
                        case BracketType.SINGLE:
                            Bracket = new SingleElimBracket(Model);
                            break;
                        case BracketType.DOUBLE:
                            Bracket = new DoubleElimBracket(Model);
                            break;
                        case BracketType.ROUNDROBIN:
                            Bracket = new RoundRobinBracket(Model);
                            break;
                        case BracketType.SWISS:
                            Bracket = new SwissBracket(Model);
                            break;
                    }
                }
                else
                {
                    List<IPlayer> players = new List<IPlayer>();
                    foreach (TournamentUserModel user in Model.Tournament.TournamentUsers)
                    {
                        players.Add(new User(user));
                    }

                    switch (Model.BracketType.Type)
                    {
                        case BracketType.SINGLE:
                            Bracket = new SingleElimBracket(players);
                            break;
                        case BracketType.DOUBLE:
                            Bracket = new DoubleElimBracket(players);
                            break;
                        case BracketType.ROUNDROBIN:
                            Bracket = new RoundRobinBracket(players);
                            break;
                        case BracketType.SWISS:
                            Bracket = new SwissBracket(players);
                            break;
                    }
                }
            }
        }

        public bool Update()
        {
            if (Model != null)
            {
                return db.UpdateBracket(Model) == DbError.SUCCESS;
            }

            return false;
        }

        public bool Update(BracketModel model)
        {
            //Model = model;
            return db.UpdateBracket(model) == DbError.SUCCESS;
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
                    result = db.UpdateMatch(Bracket.GetMatchModel(i));
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
            TournamentUserModel model = Model.Tournament.TournamentUsers.FirstOrDefault(x => x.AccountID == accountId);

            if (model != null)
            {
                return (Permission)model.PermissionLevel;
            }
            else
            {
                return Permission.NONE;
            }
        }
        public bool IsAdministrator(int accountId)
        {
            Permission permission = TournamentPermission(accountId);
            if (permission == Permission.TOURNAMENT_ADMINISTRATOR || 
                permission == Permission.TOURNAMENT_CREATOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCreator(int accountId)
        {
            return TournamentPermission(accountId) == Permission.TOURNAMENT_CREATOR;
        }

        public bool UsePoints()
        {
            if (Bracket.BracketType == BracketType.GSLGROUP || 
                Bracket.BracketType == BracketType.RRGROUP || 
                Bracket.BracketType == BracketType.SWISS ||
                Bracket.BracketType == BracketType.ROUNDROBIN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<bool> RoundShowing(BracketSection section)
        {
            List<bool> showMatches = new List<bool>();
            bool isPowerRule = IsPowerOfTwo(Bracket.Players.Count);
            int roundNum = 1;

            if (section == BracketSection.UPPER)
            {
                for (int i = 1; Bracket.GetRound(roundNum).Count != 0; i++)
                {
                    bool show = false;

                    if (Bracket.Players.Count >= 8)
                    {
                        if (isPowerRule)
                        {
                            if (i <= 2 || i % 2 == 0) show = true;
                            else show = false;
                        }
                        else
                        {
                            if (i <= 2 || i % 2 == 1) show = true;
                            else show = false;
                        }
                    }
                    else
                    {
                        show = true;
                    }

                    showMatches.Add(show);
                    if (show) roundNum++;
                }
            }
            else if (section == BracketSection.LOWER)
            {
                for (int i = 1; Bracket.GetLowerRound(roundNum).Count != 0; i++)
                {
                    bool show = false;

                    if (isPowerRule)
                    {
                        show = true;
                    }
                    else
                    {
                        if (i == 1) show = false;
                        else show = true;
                    }

                    showMatches.Add(show);
                    if (show) roundNum++;
                }
            }
            
            return showMatches;
        }

        public bool IsPowerOfTwo(int val)
        {
            return (val != 0) && ((val & (val-1)) == 0);
        }
    }
}