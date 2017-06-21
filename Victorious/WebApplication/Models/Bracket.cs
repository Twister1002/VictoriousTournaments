using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using Tournament.Structure;
using Tournaments = Tournament.Structure;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class Bracket 
    {
        public bool roundsModified;
        private IService services;
        private IBracket bracket;
        private IGroupStage groupBracket;
        private BracketModel model;

        public int Id { get; private set; }
        public bool IsLocked { get; private set; }

        public Bracket(IService services, IBracket bracket, BracketModel model)
        {
            this.services = services;
            this.bracket = bracket;
            this.model = model;
            this.groupBracket = bracket as IGroupStage;
            IsLocked = model.IsLocked;
            Init();
        }

        public void Init()
        {
            bracket.MatchesModified += OnMatchesUpdated;
            bracket.RoundAdded += OnRoundAdd;
            bracket.RoundDeleted += OnRoundDelete;
            bracket.GamesDeleted += OnGamesDeleted;

            roundsModified = false;
            Id = bracket.Id;
        }

        public Tournaments.IBracket IBracket { get { return bracket; } }
        public Tournaments.IGroupStage Group { get { return groupBracket; } }

        /// <summary>
        /// Gets a list of matches for a round
        /// </summary>
        /// <param name="roundNum">the round number</param>
        /// <param name="section">the section of the tournament to get</param>
        /// <returns></returns>
        public List<Match> GetRound(int roundNum, BracketSection section)
        {
            List<Match> matches = new List<Match>();
            List<IMatch> origMatches = new List<IMatch>();

            switch (section)
            {
                case BracketSection.UPPER:
                case BracketSection.FINAL:
                    origMatches = bracket.GetRound(roundNum);
                    break;
                case BracketSection.LOWER:
                    origMatches = bracket.GetLowerRound(roundNum);
                    break;
            }

            foreach (IMatch match in origMatches)
            {
                matches.Add(new Match(services, match));
            }

            return matches;
        }

        /// <summary>
        /// Gets the round for a group
        /// </summary>
        /// <param name="groupNum">the group number</param>
        /// <param name="roundNum">the round number</param>
        /// <returns>A list of matches for the group</returns>
        public List<Match> GetRound(int groupNum, int roundNum)
        {
            List<Match> matches = new List<Match>();

            foreach (IMatch match in groupBracket.GetRound(groupNum, roundNum))
            {
                matches.Add(new Match(services, match));
            }

            return matches;
        }

        /// <summary>
        /// Gets the Grand Final match
        /// </summary>
        /// <returns>A match</returns>
        public Match GrandFinal()
        {
            return new Match(services, bracket.GrandFinal);
        }
        
        /// <summary>
        /// Resets the bracket information back to its original state
        /// </summary>
        /// <returns>True if saved; false is failed.</returns>
        public bool Reset()
        {
            // Fires events below
            bracket.ResetMatches();

            return services.Save();
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

            if (bracket.BracketType == DatabaseLib.BracketType.SWISS)
            {
                List<IMatch> matches = bracket.GetRound(head.RoundIndex);

                if (!matches.Any(x => x.IsFinished == false))
                {
                    for (int i = head.RoundIndex; i <= bracket.NumberOfRounds; i++)
                    {
                        matches = bracket.GetRound(i);

                        foreach (IMatch match in matches)
                        {
                            matchesAffected.Add(match.MatchNumber);
                        }
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
                if (bracket.BracketType != DatabaseLib.BracketType.DOUBLE)
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
            if (bracket.BracketType == DatabaseLib.BracketType.SINGLE || bracket.BracketType == DatabaseLib.BracketType.DOUBLE)
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
                case DatabaseLib.BracketType.ROUNDROBIN:
                case DatabaseLib.BracketType.SWISS:
                case DatabaseLib.BracketType.RRGROUP:
                case DatabaseLib.BracketType.GSLGROUP:
                    return true;

                case DatabaseLib.BracketType.SINGLE:
                case DatabaseLib.BracketType.DOUBLE:
                case DatabaseLib.BracketType.STEP:
                    return false;

                default:
                    return false;
            }
        }

        public String BracketName()
        {
            String name = "";
            switch (bracket.BracketType)
            {
                case DatabaseLib.BracketType.SINGLE:
                    name = "Single Elimination";
                    break;
                case DatabaseLib.BracketType.DOUBLE:
                    name = "Double Elimination";
                    break;
                case DatabaseLib.BracketType.ROUNDROBIN:
                    name = "Round Robin";
                    break;
                case DatabaseLib.BracketType.SWISS:
                    name = "Swiss";
                    break;
                case DatabaseLib.BracketType.GSLGROUP:
                    name = "GSL Group";
                    break;
                case DatabaseLib.BracketType.RRGROUP:
                    name = "Round Robin Group";
                    break;
                case DatabaseLib.BracketType.STEP:
                    name = "Step Ladder";
                    break;
            }

            return name;
        }

        public String BracketFileName()
        {
            String file = "";
            switch (bracket.BracketType)
            {
                case DatabaseLib.BracketType.SINGLE:
                case DatabaseLib.BracketType.ROUNDROBIN:
                case DatabaseLib.BracketType.SWISS:
                case DatabaseLib.BracketType.STEP:
                    file = "UpperBracket";
                    break;
                case DatabaseLib.BracketType.DOUBLE:
                    file = "Double";
                    break;
                case DatabaseLib.BracketType.GSLGROUP:
                    file = "GSLGroup";
                    break;
                case DatabaseLib.BracketType.RRGROUP:
                    file = "RoundRobinGroup";
                    break;
            }

            return file;
        }

        /// <summary>
        /// Gets a list of all users in the bracket
        /// </summary>
        /// <returns>A list of users in the bracket</returns>
        public List<TournamentUsersBracketModel> GetPlayers()
        {
            return model.TournamentUsersBrackets.ToList();
        }
        #endregion

        #region Match Stuff
        // TODO: Optomize this by getting rid of the query
        public Match GetMatchById(int matchId)
        {
            int? matchNum = services.Tournament.GetMatch(matchId)?.MatchNumber;
            if (matchNum != null)
            {
                return new Match(services, bracket.GetMatch(matchNum.Value));
            }
            else
            {
                throw new Exception(matchId + " is not a valid id");
            }
        }

        public Match GetMatchByNum(int matchNum)
        {
            return new Match(services, bracket.GetMatch(matchNum));
        }

        public bool UpdateMatch(MatchModel match)
        {
            //services.Tournament.UpdateMatch(match);
            //return services.Save();
            return true;
        }

        public bool AddGame(int matchNum, int CScore, int DScore, PlayerSlot winner)
        {
            GameModel game = bracket.AddGame(matchNum, DScore, CScore, winner);

            // Add the game to the database
            services.Tournament.AddGame(game);

            return services.Save();
        }

        public bool RemoveGame(int matchNum, int gameNum)
        {
            GameModel game = bracket.RemoveGameNumber(matchNum, gameNum);
            return true;
            //services.Tournament.DeleteGame(game.GameID);

            //return services.Save();
        }
        #endregion

        #region Events
        /// <summary>
        /// Updates the match that was given from an event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
        public void OnMatchesUpdated(object sender, BracketEventArgs args)
        {
            foreach (MatchModel matchModel in args.UpdatedMatches)
            {
                //MatchModel match = model.Matches.Single(x => x.MatchNumber == matchModel.MatchNumber);

                //match.MatchID = matchModel.MatchID;
                //match.BracketID = matchModel.BracketID;
                //match.ChallengerID = matchModel.ChallengerID;
                //match.DefenderID = matchModel.DefenderID;
                //match.WinnerID = matchModel.WinnerID;

                //match.ChallengerScore = matchModel.ChallengerScore;
                //match.DefenderScore = matchModel.DefenderScore;

                //match.MatchIndex = matchModel.MatchIndex;
                //match.RoundIndex = matchModel.RoundIndex;
                //match.PrevMatchIndex = matchModel.PrevMatchIndex;

                //match.MatchNumber = matchModel.MatchNumber;
                //match.PrevDefenderMatchNumber = matchModel.PrevDefenderMatchNumber;
                //match.PrevChallengerMatchNumber = matchModel.PrevChallengerMatchNumber;
                //match.NextMatchNumber = matchModel.NextMatchNumber;
                //match.NextLoserMatchNumber = matchModel.NextLoserMatchNumber;

                //match.MaxGames = matchModel.MaxGames;

                //services.Tournament.UpdateMatch(match);

                services.Tournament.UpdateMatch(matchModel);
            }

            foreach (int games in args.DeletedGameIDs)
            {
                services.Tournament.DeleteGame(games);
            }

            services.Save();
        }

        /// <summary>
        /// Updates the match that was given from an event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
        public void OnRoundAdd(object sender, BracketEventArgs args)
        {
            foreach (MatchModel match in args.UpdatedMatches)
            {
                services.Tournament.AddMatch(match);
            }

            if (services.Save())
            {
                roundsModified = true;
            }
        }

        /// <summary>
        /// Updates the match that was given from an event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
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

            if (services.Save())
            {
                roundsModified = true;
            }
        }

        /// <summary>
        /// Removes the games given from the event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
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