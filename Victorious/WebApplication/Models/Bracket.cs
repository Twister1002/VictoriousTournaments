﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using Tournament.Structure;
using Tournaments = Tournament.Structure;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class Bracket 
    {
        Service services;
        Tournaments.IBracket bracket;

        public Bracket(Service services, Tournaments.IBracket bracket)
        {
            this.services = services;
            this.bracket = bracket;
            Init();
        }

        public void Init()
        {
            bracket.MatchesModified += OnMatchesUpdated;
            bracket.RoundAdded += OnRoundAdd;
            bracket.RoundDeleted += OnRoundDelete;
            bracket.GamesDeleted += OnGamesDeleted;
        }

        public Match GetMatch(int matchNum)
        {
            return new Match(services, bracket.GetMatch(matchNum));
        }

        #region CRUD
        public bool Crate()
        {
            throw new NotImplementedException("Bracket can not be created from here");
        }

        public bool Retrieve()
        {
            throw new NotImplementedException("Bracket can not be retrieved from here");
        }

        public bool Update()
        {
            throw new NotImplementedException("Bracket can not be updated from here");
        }

        public bool Delete()
        {
            throw new NotImplementedException("Bracket can not be deleted from here");
        }
        #endregion

        #region RoundHelpers
        public List<int> MatchesAffectedList(int matchNum)
        {
            List<int> matchesAffected = new List<int>();
            IMatch head = bracket.GetMatch(matchNum);

            if (bracket.BracketType == BracketType.SWISS)
            {
                List<IMatch> matches = bracket.GetRound(head.RoundIndex);

                if (!matches.Any(x => x.IsFinished == false))
                {
                    matches = bracket.GetRound(head.RoundIndex + 1);
                    foreach (IMatch match in matches)
                    {
                        matchesAffected.Add(match.MatchNumber);
                    }
                }

                matchesAffected.Add(head.MatchNumber);
            }
            else
            {

                matchesAffected.Add(matchNum);

                if (head.NextMatchNumber != -1)
                {
                    List<int> matches = MatchesAffectedList(head.NextMatchNumber);

                    foreach (int match in matches)
                    {
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
            }
            return matchesAffected;
        }

        public int TotalDispalyRounds(List<bool> upper, List<bool> lower)
        {
            int maxDisplay = Math.Max(upper.Count, lower.Count);

            if (bracket.GrandFinal != null) maxDisplay++;

            return maxDisplay;
        }

        public List<bool> RoundShowing(BracketSection section)
        {
            List<bool> showMatches = new List<bool>();
            bool isPowerRule = IsPowerOfTwo(bracket.Players.Count);
            int roundNum = 1;

            if (bracket.NumberOfRounds > 0)
            {
                if (bracket.BracketType != BracketType.DOUBLE)
                {
                    for (int i = 0; i < bracket.NumberOfRounds; i++) showMatches.Add(true);
                    return showMatches;
                }

                if (section == BracketSection.UPPER)
                {
                    for (int i = 1; bracket.GetRound(roundNum).Count != 0; i++)
                    {
                        bool show = false;

                        if (bracket.Players.Count >= 8)
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
                    for (int i = 1; bracket.GetLowerRound(roundNum).Count != 0; i++)
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
            }
            return showMatches;
        }

        private String RoundTitle(int roundNum, int maxRounds)
        {
            if (bracket.BracketType == BracketType.SINGLE || bracket.BracketType == BracketType.DOUBLE)
            {
                if (roundNum == maxRounds)
                {
                    return "Finals";
                }
                else if (roundNum == (maxRounds - 1))
                {
                    return "Semi-Finals";
                }
                else if (roundNum == (maxRounds - 2))
                {
                    return "Quarter Finals";
                }
                else
                {
                    return "Round " + roundNum;
                }
            }
            else
            {
                return "Round " + roundNum;
            }
        }

        public RoundHeader GetRoundHeaders(int roundNum, BracketSection section)
        {
            RoundHeader headers = new RoundHeader();
            List<IMatch> matches = new List<IMatch>();

            if (section == BracketSection.UPPER)
            {
                matches = bracket.GetRound(roundNum);
                headers.title = RoundTitle(roundNum, bracket.NumberOfRounds);
            }
            else if (section == BracketSection.LOWER)
            {
                matches = bracket.GetLowerRound(roundNum);
                headers.title = RoundTitle(roundNum, bracket.NumberOfLowerRounds);
            }
            else if (section == BracketSection.FINAL)
            {
                if (bracket.GrandFinal != null)
                {
                    matches.Add(bracket.GrandFinal);
                    headers.title = "Grand Final";
                }
            }

            if (matches.Count > 0)
            {
                headers.roundNum = matches[0].RoundIndex;
                headers.bestOf = matches[0].MaxGames;
            }

            return headers;
        }
        #endregion

        #region Helpers
        public int TotalRounds()
        {
            int maxRounds = Math.Max(bracket.NumberOfRounds, bracket.NumberOfLowerRounds);
            if (bracket.GrandFinal != null) maxRounds++;

            return maxRounds;
        }

        public bool IsPowerOfTwo(int val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }

        public bool UsePoints()
        {
            switch (bracket.BracketType)
            {
                case BracketType.ROUNDROBIN:
                case BracketType.SWISS:
                case BracketType.RRGROUP:
                case BracketType.GSLGROUP:
                    return true;

                case BracketType.SINGLE:
                case BracketType.DOUBLE:
                    return false;

                default:
                    return false;
            }
        }
        #endregion

        #region Events
        public void OnMatchesUpdated(object sender, BracketEventArgs args)
        {
            foreach (MatchModel match in args.UpdatedMatches)
            {
                services.Tournament.UpdateMatch(match);
            }

            foreach (int games in args.DeletedGameIDs)
            {
                services.Tournament.DeleteGame(games);
            }

            services.Save();
        }

        public void OnRoundAdd(object sender, BracketEventArgs args)
        {
            foreach (MatchModel match in args.UpdatedMatches)
            {
                services.Tournament.AddMatch(match);
            }

            services.Save();
        }

        public void OnRoundDelete(object sender, BracketEventArgs args)
        {
            foreach (int games in args.DeletedGameIDs)
            {
                services.Tournament.DeleteGame(games);
            }

            foreach (MatchModel match in args.UpdatedMatches)
            {
                services.Tournament.DeleteMatch(match.MatchID);
            }

            services.Save();
        }

        public void OnGamesDeleted(object sender, BracketEventArgs args)
        {
            foreach (int gameId in args.DeletedGameIDs)
            {
                services.Tournament.DeleteGame(gameId);
            }

            services.Save();
        }
        #endregion
    }
}