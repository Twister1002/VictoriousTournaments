﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;
using DatabaseLib.Services;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public enum BracketSection
    {
        UPPER,
        LOWER,
        FINAL
    }

    public struct RoundHeader
    {
        public int roundNum;
        public int bestOf;
        public String title;
    }

    public class BracketViewModel : BracketFields, IViewModel
    {
        public bool roundsModified { get; private set; }
        public IBracket Bracket { get; private set; }
        public BracketModel Model { get; private set; }

        public BracketViewModel()
        {
            Bracket = null;
            Model = null;
            Init();
        }

        public BracketViewModel(BracketModel model)
        {
            Model = model;
            Bracket = null;
            Init();
        }

        public BracketViewModel(IBracket bracket)
        {
            Bracket = bracket;
            Model = null;
            Init();
        }

        public BracketViewModel(int id)
        {
            Model = tournamentService.GetBracket(id);
            Init();
        }

        public void Init()
        {
            if (Model != null && Bracket == null)
            {
                Bracket = new Tournament.Structure.Tournament().RestoreBracket(Model);
            }

            if (Bracket != null)
            {
                LoadEvents();
            }
        }

        private void LoadEvents()
        {
            Bracket.MatchesModified += OnMatchesUpdated;
            Bracket.RoundAdded += OnRoundAdd;
            Bracket.RoundDeleted += OnRoundDelete;
            Bracket.GamesDeleted += OnGamesDeleted;
        }

        //private void LoadBracket()
        //{
        //    if (Model != null)
        //    {
        //        if (Model.Finalized)
        //        {
        //            switch (Model.BracketType.Type)
        //            {
        //                case BracketType.SINGLE:
        //                    Bracket = new SingleElimBracket(Model);
        //                    break;
        //                case BracketType.DOUBLE:
        //                    Bracket = new DoubleElimBracket(Model);
        //                    break;
        //                case BracketType.ROUNDROBIN:
        //                    Bracket = new RoundRobinBracket(Model);
        //                    break;
        //                case BracketType.SWISS:
        //                    Bracket = new SwissBracket(Model);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            List<IPlayer> players = new List<IPlayer>();
        //            foreach (TournamentUserModel user in Model.Tournament.TournamentUsers)
        //            {
        //                players.Add(new User(user));
        //            }

        //            switch (Model.BracketType.Type)
        //            {
        //                case BracketType.SINGLE:
        //                    Bracket = new SingleElimBracket(players);
        //                    break;
        //                case BracketType.DOUBLE:
        //                    Bracket = new DoubleElimBracket(players);
        //                    break;
        //                case BracketType.ROUNDROBIN:
        //                    Bracket = new RoundRobinBracket(players);
        //                    break;
        //                case BracketType.SWISS:
        //                    Bracket = new SwissBracket(players);
        //                    break;
        //            }
        //        }
        //    }
        //}

        public bool Update()
        {
            if (Model != null)
            {
                tournamentService.UpdateBracket(Model);
                return Save();
            }

            return false;
        }

        public bool Update(BracketModel model)
        {
            tournamentService.UpdateBracket(model);
            return Save();
        }

        public int TotalRounds()
        {
            int maxRounds = Math.Max(Bracket.NumberOfRounds, Bracket.NumberOfLowerRounds);
            if (Bracket.GrandFinal != null) maxRounds++;

            return maxRounds;
        }

        public List<int> MatchesAffectedList(int matchNum)
        {
            List<int> matchesAffected = new List<int>();
            IMatch head = Bracket.GetMatch(matchNum);

            if (Bracket.BracketType == BracketType.SWISS)
            {
                List<IMatch> matches = Bracket.GetRound(head.RoundIndex);

                if (!matches.Any(x => x.IsFinished == false))
                {
                    matches = Bracket.GetRound(head.RoundIndex + 1);
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

        public RoundHeader GetRoundHeaders(int roundNum, BracketSection section)
        {
            RoundHeader headers = new RoundHeader();
            List<IMatch> matches = new List<IMatch>();

            if (section == BracketSection.UPPER)
            {
                matches = Bracket.GetRound(roundNum);
                headers.title = RoundTitle(roundNum, Bracket.NumberOfRounds);
            }
            else if (section == BracketSection.LOWER)
            {
                matches = Bracket.GetLowerRound(roundNum);
                headers.title = RoundTitle(roundNum, Bracket.NumberOfLowerRounds);
            }
            else if (section == BracketSection.FINAL)
            {
                if (Bracket.GrandFinal != null)
                {
                    matches.Add(Bracket.GrandFinal);
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

        private String RoundTitle(int roundNum, int maxRounds)
        {
            if (Bracket.BracketType == BracketType.SINGLE || Bracket.BracketType == BracketType.DOUBLE)
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

        public int TotalDispalyRounds(List<bool> upper, List<bool> lower)
        {
            //int upper = RoundShowing(BracketSection.UPPER).Count;
            //int lower = RoundShowing(BracketSection.LOWER).Count;

            int maxDisplay = Math.Max(upper.Count, lower.Count);

            if (Bracket.GrandFinal != null) maxDisplay++;

            return maxDisplay;
        }

        public List<bool> RoundShowing(BracketSection section)
        {
            List<bool> showMatches = new List<bool>();
            bool isPowerRule = IsPowerOfTwo(Bracket.Players.Count);
            int roundNum = 1;

            if (Bracket.NumberOfRounds > 0)
            {
                if (Bracket.BracketType != BracketType.DOUBLE)
                {
                    for (int i = 0; i < Bracket.NumberOfRounds; i++) showMatches.Add(true);
                    return showMatches;
                }

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
            }
            return showMatches;
        }

        #region HelperMethods
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

        public bool IsPowerOfTwo(int val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }
        #endregion

        #region Events
        public void OnMatchesUpdated(object sender, BracketEventArgs args)
        {
            foreach (MatchModel match in args.UpdatedMatches)
            {
                tournamentService.UpdateMatch(match);
            }

            foreach (int games in args.DeletedGameIDs)
            {
                tournamentService.DeleteGame(games);
            }

            Save();
        }

        public void OnRoundAdd(object sender, BracketEventArgs args)
        {
            this.roundsModified = true;
            foreach (MatchModel match in args.UpdatedMatches)
            {
                tournamentService.AddMatch(match);
            }

            Save();
        }
        
        public void OnRoundDelete(object sender, BracketEventArgs args)
        {
            this.roundsModified = true;
            foreach (int games in args.DeletedGameIDs)
            {
                tournamentService.DeleteGame(games);
            }

            foreach (MatchModel match in args.UpdatedMatches)
            {
                tournamentService.DeleteMatch(match.MatchID);
            }

            Save();
        }

        public void OnGamesDeleted(object sender, BracketEventArgs args)
        {
            foreach (int gameId in args.DeletedGameIDs)
            {
                tournamentService.DeleteGame(gameId);
            }

            Save();
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void SetFields()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}