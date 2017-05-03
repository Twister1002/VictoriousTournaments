﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;

namespace DatabaseLib
{
    public enum Permission
    {
        // 0 = None
        NONE = 0,
        // 1 = Site Permissions
        SITE_ADMINISTRATOR = 1, SITE_STANDARD,
        // 100 = Tournament Permissions
        TOURNAMENT_ADMINISTRATOR = 100, TOURNAMENT_STANDARD, TOURNAMENT_CREATOR,
        // 200 = Team Permissions
        TEAM_CAPTAIN = 200, TEAM_STANDARD
    }

    public enum BracketType
    {
        SINGLE = 1,
        DOUBLE,
        ROUNDROBIN,
        RRGROUP,
        GSLGROUP,
        SWISS
    }

    public enum DbError
    {
        ERROR = -1, NONE = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_REMOVE, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS, CONCURRENCY_ERROR
    };

    public class DbInterface
    {
        VictoriousEntities context = new VictoriousEntities();

        public Exception interfaceException;

        public DbInterface()
        {
            if (context.BracketTypeModels.Find(1) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 1, Type = BracketType.SINGLE, TypeName = "Single Elimination" });
                context.SaveChanges();
            }
            if (context.BracketTypeModels.Find(2) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 2, Type = BracketType.DOUBLE, TypeName = "Double Elimination" });
                context.SaveChanges();
            }
            if (context.BracketTypeModels.Find(3) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 3, Type = BracketType.ROUNDROBIN, TypeName = "Round Robin" });
                context.SaveChanges();
            }
            if (context.BracketTypeModels.Find(4) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 4, Type = BracketType.RRGROUP, TypeName = "RR Group" });
                context.SaveChanges();
            }
            if (context.BracketTypeModels.Find(5) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 5, Type = BracketType.GSLGROUP, TypeName = "GSL Group" });
                context.SaveChanges();
            }
            if (context.BracketTypeModels.Find(6) == null)
            {
                context.BracketTypeModels.Add(new BracketTypeModel() { BracketTypeID = 6, Type = BracketType.GSLGROUP, TypeName = "Swiss" });
                context.SaveChanges();
            }
        }

        #region Accounts

        public DbError AddAccount(AccountModel account)
        {
            try
            {
                account.CreatedOn = DateTime.Now;
                context.AccountModels.Add(account);
                
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError AccountExists(AccountModel account)
        {
            AccountModel _account;
            try
            {
                _account = context.AccountModels.Find(account.AccountID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                throw;
            }
            if (account == null)
                return DbError.DOES_NOT_EXIST;
            else
                return DbError.EXISTS;
        }

        // Updates LastLogin of passed-in account.
        public DbError LogAccountIn(int accountId)
        {
            try
            {

                AccountModel account = context.AccountModels.Find(accountId);
                account.LastLogin = DateTime.Now;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError UpdateAccount(AccountModel account)
        {
            try
            {
                AccountModel _account = context.AccountModels.Find(account.AccountID);
                context.Entry(_account).CurrentValues.SetValues(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteAccount(AccountModel account)
        {
            AccountModel _account = context.AccountModels.Find(account.AccountID);
            try
            {
                context.AccountModels.Remove(_account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public AccountModel GetAccount(int accountId)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Find(accountId);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                account.AccountID = -1;
            }
            return account;
        }

        public AccountModel GetAccount(string username)
        {
            AccountModel account = new AccountModel();
            try
            {
                account = context.AccountModels.Single(x => x.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                account.AccountID = -1;
                return account;
            }
            return account;
        }

        public List<AccountModel> GetAllAccounts()
        {
            List<AccountModel> accountModels = new List<AccountModel>();
            try
            {
                foreach (var accont in context.AccountModels)
                {
                    accountModels.Add(accont);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                accountModels.Clear();
                WriteException(ex);
                accountModels.Add(new AccountModel() { AccountID = -1 });
            }
            return accountModels;
        }

        public DbError AccountUsernameExists(string username)
        {
            try
            {
                AccountModel account = context.AccountModels.Single(u => u.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError AccountEmailExists(string email)
        {
            try
            {
                AccountModel user = context.AccountModels.Single(u => u.Email == email);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }


        #endregion


        #region Tournaments

        public List<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                tournaments = context.TournamentModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        public DbError AddTournament(TournamentModel tournament)
        {

            TournamentModel _tournament = new TournamentModel();
            try
            {
                _tournament = tournament;

                _tournament.CreatedOn = DateTime.Now;
                _tournament.LastEditedOn = DateTime.Now;
                context.SaveChanges();
                context.TournamentModels.Add(_tournament);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public TournamentModel GetTournament(int id)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.TournamentModels.Single(t => t.TournamentID == id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournament.TournamentID = -1;
            }
            return tournament;
        }

        public List<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            List<TournamentUserModel> list = new List<TournamentUserModel>();

            try
            {
                list = context.TournamentModels.Find(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                list.Clear();
            }
            return list;
        }

        public DbError UpdateTournament(TournamentModel tournament, bool cascade = false)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(tournament.TournamentID);
                context.Entry(_tournament).CurrentValues.SetValues(tournament);
                if (cascade)
                {
                    foreach (BracketModel bracket in tournament.Brackets)
                    {
                        UpdateBracket(bracket, true);
                    }
                    foreach (var tournamentUser in context.TournamentUserModels)
                    {
                        UpdateTournamentUser(tournamentUser);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(int id)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(id);

                foreach (var bracket in _tournament.Brackets.ToList())
                {
                    DeleteBracket(bracket.BracketID);
                }
                foreach (var user in _tournament.TournamentUsers.ToList())
                {
                    DeleteTournamentUser(user.TournamentUserID);
                }
                context.TournamentModels.Remove(_tournament);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();

            try
            {
                List<SqlParameter> sqlparams = new List<SqlParameter>();
                string query = string.Empty;
                //string tournamentIdQuery = "SELECT TournamentID FROM dbo.TournamentModels AS Tournament  ";
                //string rulesIdQuery = "SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";

                //string[] queries = new string[] { "Tournament.Title = @Title", "Tournament.StartDate = @StartDate" };  
                if (searchParams.ContainsKey("Title"))
                {
                    sqlparams.Add(new SqlParameter("@Title", searchParams["Title"]));
                    query = "SELECT TournamentID FROM TournamentModels WHERE Title = @Title";
                }
                if (searchParams.ContainsKey("StartDate"))
                {
                    sqlparams.Add(new SqlParameter("@StartDate", DateTime.Parse(searchParams["TournamentStartDate"])));
                    if (searchParams.ContainsKey("Title"))
                        query += "UNION SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";
                    else
                        query = "SELECT TournamentID FROM TournamentRule WHERE TournamentStartDate = @StartDate";
                }
                tournaments = context.TournamentModels.SqlQuery(query, sqlparams).ToList();
                //if (searchParams.ContainsKey("GameTypeID"))
                //{
                //    sqlparams.Add(new SqlParameter("@Game", searchParams["Game"]));
                //    foreach (var tournament in tournaments)
                //    {
                //        if (tournament)
                //    }
                //}




                //List < TournamentModel > _tournaments = context.TournamentModels.SqlQuery("SELECT * FROM dbo.TournamentModels WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();

            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
            }
            return tournaments;

        }

        public List<TournamentModel> FindTournaments(string title, DateTime startDate)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            //TournamentRulesModel rules = tournament.TournamentRule;
            try
            {
                List<TournamentModel> _tournaments = context.TournamentModels.SqlQuery("SELECT * FROM dbo.TournamentModels WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();
                foreach (var _tournament in _tournaments)
                {
                    if (_tournament.TournamentStartDate == startDate)
                        tournaments.Add(_tournament);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
                return tournaments;
            }
            return tournaments;
        }

        #endregion


        #region TournamentUsers

        public DbError AddTournamentUser(TournamentUserModel user)
        {
            try
            {
                context.TournamentUserModels.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
                throw;
            }
            return DbError.SUCCESS;
        }

        public TournamentUserModel GetTournamentUser(int tournamentUserId)
        {
            TournamentUserModel user = new TournamentUserModel();
            try
            {
                user = context.TournamentUserModels.Find(tournamentUserId);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                user.TournamentUserID = -1;
            }
            return user;
        }

        public DbError UpdateTournamentUser(TournamentUserModel user)
        {
            try
            {
                TournamentUserModel _user = context.TournamentUserModels.Find(user.TournamentUserID);
                context.Entry(_user).CurrentValues.SetValues(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournamentUser(int tournamentUserId)
        {
            try
            {
                TournamentUserModel _user = context.TournamentUserModels.Find(tournamentUserId);
                context.TournamentUserModels.Remove(_user);
                //context.TournamentModels.Include(x => x.).Single(x => x.TournamentID == tournament.TournamentID).Users.Remove(_user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_REMOVE;
            }
            return DbError.SUCCESS;
        }

        public DbError AddTournamentUserToBracket(int tournamentUserId, int bracketId, int seed)
        {
            try
            {
                TournamentUsersBracketModel t = new TournamentUsersBracketModel();
                t.BracketID = bracketId;
                t.Seed = seed;
                t.TournamentUserID = tournamentUserId;
                context.TournamentUsersBracketModels.Add(t);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public int GetTournamentUserSeed(int tournamentUserId, int bracketId)
        {
            int seed = 0;
            try
            {
                seed = context.TournamentUsersBracketModels.Where(x => x.TournamentUserID == tournamentUserId && x.BracketID == bracketId).Single().Seed.Value;
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                seed = -1;
            }
            return seed;
        }

        #endregion


        #region Brackets

        public DbError AddBracket(BracketModel bracket)
        {
            try
            {
                context.BracketModels.Add(bracket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public BracketModel GetBracket(int id)
        {
            BracketModel bracket = new BracketModel();
            try
            {
                bracket = context.BracketModels.Single(b => b.BracketID == id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                bracket.BracketID = -1;
                WriteException(ex);
                return bracket;
            }
            return bracket;
        }

        public List<BracketModel> GetAllBracketsInTournament(int tournamentId)
        {
            List<BracketModel> brackets = new List<BracketModel>();
            try
            {
                brackets = context.BracketModels.Where(x => x.TournamentID == tournamentId).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                brackets.Clear();
            }
            return brackets;
        }

        public DbError UpdateBracket(BracketModel bracket, bool cascade = false)
        {
            try
            {
                BracketModel _bracket = context.BracketModels.Find(bracket.BracketID);
                context.Entry(_bracket).CurrentValues.SetValues(bracket);
                if (cascade)
                {
                    foreach (var match in bracket.Matches)
                    {
                        UpdateMatch(match, true);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteBracket(int id)
        {
            try
            {
                BracketModel _bracket = context.BracketModels.Find(id);
                foreach (var match in _bracket.Matches.ToList())
                {
                    DeleteMatch(match.MatchID);
                }
                context.BracketModels.Remove(_bracket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion


        #region Matches

        public DbError AddMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = new MatchModel();
                _match = match;
                
                _match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                _match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                
                //context.Challengers.Add(new Challenger() { TournamentUserID = _match.ChallengerID, MatchID = _match.MatchID });
                //context.Defenders.Add(new Defender() { TournamentUserID = _match.DefenderID, MatchID = _match.MatchID });


                //context.Matches.Load();
                //context.Users.Load();
                context.MatchModels.Add(_match);
                context.Entry(_match).CurrentValues.SetValues(match);
                context.SaveChanges();
                //bracket.Matches.Add(_match);
                //context.SaveChanges();
                //context.Tournaments.Include(x => x.Brackets).Load();
                //context.Users.Include(x => x.UserID).Load();
                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.ERROR;
            }
            return DbError.SUCCESS;
        }

        public MatchModel GetMatch(int matchId)
        {
            MatchModel match = new MatchModel();
            try
            {
                match = context.MatchModels.Find(matchId);
                match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                match.Defender = context.TournamentUserModels.Find(match.DefenderID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                match.MatchID = -1;
                return match;
            }
            return match;
        }

        public List<MatchModel> GetAllMatchesInBracket(int bracketId)
        {
            List<MatchModel> matches = new List<MatchModel>();

            try
            {
                matches = context.MatchModels.Where(x => x.BracketID == bracketId).ToList();
                foreach (var match in matches)
                {
                    match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                matches.Clear();

            }
            return matches;
        }

        public DbError UpdateMatch(MatchModel match, bool cascade = false)
        {
            try
            {
                MatchModel _match = context.MatchModels.Find(match.MatchID);
                if (_match.ChallengerID != match.ChallengerID || _match.DefenderID != match.DefenderID)
                {
                    _match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    _match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
                UpdateTournamentUser(match.Defender);
                UpdateTournamentUser(match.Challenger);
                if (cascade)
                {
                    foreach (var game in _match.Games.ToList())
                    {
                        UpdateGame(game);
                    }
                }
                context.Entry(_match).CurrentValues.SetValues(match);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteMatch(int matchId)
        {
            try
            {
                MatchModel _match = context.MatchModels.Find(matchId);
                foreach (var game in _match.Games.ToList())
                {
                    DeleteGame(game.GameID);
                }
                context.MatchModels.Remove(_match);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion


        #region Games

        public DbError AddGame(GameModel game)
        {
            try
            {
                context.GameModels.Add(game);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public GameModel GetGame(int id)
        {
            GameModel game = new GameModel();
            try
            {
                game = context.GameModels.Find(id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                game.GameID = -1;
            }
            return game;
        }

        public List<GameModel> GetAllGamesInMatch(int matchId)
        {
            List<GameModel> games = new List<GameModel>();
            try
            {
                games = context.GameModels.Where(x => x.MatchID == matchId).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                games.Clear();
            }
            return games;
        }

        public DbError UpdateGame(GameModel game)
        {
            try
            {
                GameModel _game = context.GameModels.Find(game.GameID);
                context.Entry(_game).CurrentValues.SetValues(game);
                if (_game.ChallengerID != game.ChallengerID || _game.DefenderID != game.DefenderID)
                {
                    _game.ChallengerID = game.ChallengerID;
                    _game.DefenderID = game.DefenderID;
                }
                if (_game.ChallengerID != _game.Match.ChallengerID || _game.DefenderID != _game.Match.DefenderID)
                {
                    _game.ChallengerID = game.Match.ChallengerID;
                    _game.DefenderID = game.Match.DefenderID;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteGame(int id)
        {
            try
            {
                GameModel _game = context.GameModels.Find(id);
                context.GameModels.Remove(_game);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion


        #region GameTypes

        public DbError AddGameType(GameTypeModel gameType)
        {
            try
            {
                context.GameTypeModels.Add(gameType);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError UpdateGameType(GameTypeModel gameType)
        {
            try
            {
                GameTypeModel _gameType = context.GameTypeModels.Find(gameType.GameTypeID);
                context.Entry(_gameType).CurrentValues.SetValues(gameType);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public List<GameTypeModel> GetAllGameTypes()
        {
            List<GameTypeModel> gameTypes = new List<GameTypeModel>();
            try
            {
                gameTypes = context.GameTypeModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                gameTypes.Clear();
                gameTypes.Add(new GameTypeModel() { GameTypeID = -1 });
            }
            return gameTypes;
        }

        public DbError DeleteGameType(int gameTypeId)
        {
            GameTypeModel _gameType = context.GameTypeModels.Find(gameTypeId);
            try
            {
                context.GameTypeModels.Remove(_gameType);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion


        #region BracketTypes

        public List<BracketTypeModel> GetAllBracketTypes()
        {
            List<BracketTypeModel> types = new List<BracketTypeModel>();
            try
            {
                types = context.BracketTypeModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                types.Clear();
                return types;
            }
            return types;
        }


        #endregion


        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }
    }
}
