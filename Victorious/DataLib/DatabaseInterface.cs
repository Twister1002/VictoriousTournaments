﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace DataLib
{
  

    // Use FAILED_TO_DELETE only when something is being deleted from database, otherwise user FAILED_TO_REMOVE
    public enum DbError
    {
        ERROR = -1, NONE = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_REMOVE, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS, CONCURRENCY_ERROR
    };

    public class DatabaseInterface : IDisposable
    {
        VictoriousDbContext context = new VictoriousDbContext();
        public Exception interfaceException;
        public DatabaseInterface()
        {
            //context.Database.Connection.Open();
            if (context.BracketTypes.Find(1) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 1, Type = BracketTypeModel.BracketType.SINGLE, TypeName = "Single Elimination" });
                context.SaveChanges();
            }
            if (context.BracketTypes.Find(2) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 2, Type = BracketTypeModel.BracketType.DOUBLE, TypeName = "Double Elimination" });
                context.SaveChanges();

            }
            if (context.BracketTypes.Find(3) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 3, Type = BracketTypeModel.BracketType.ROUNDROBIN, TypeName = "Round Robin" });
                context.SaveChanges();

            }

            context.Tournaments
                .Include(x => x.Brackets)
                .Include(x => x.Users)
                .Include(x => x.Teams)
                .Include(x => x.TournamentRules)
                .Include(x => x.UsersInTournament)
                .Load();
            context.Brackets
                .Include(x => x.Matches)
                .Include(x => x.UserSeeds)
                .Load();
            context.Matches
                .Include(x => x.Games)
                .Load();
            context.Teams
                .Include(x => x.TeamMembers)
                .Load();
            context.TeamMembers
                .Load();
            context.Users
                .Include(x => x.ChallengerMatches)
                .Include(x => x.DefenderMatches)
                .Load();
            context.GameTypes
                .Load();
            context.Games
                .Load();




        }
        // DO NOT EVER CALL THIS FUNCTION OUTSIDE THE DEBUG PROJECT
        public void Clear()
        {
            //context.Brackets.SqlQuery("DELETE FROM Brackets");
            //context.Matches.SqlQuery("DELETE FROM Matches");
            //context.Users.SqlQuery("DELETE FROM Users");
            //context.TournamentRules.SqlQuery("DELETE FROM TournamentRules");
            //context.Tournaments.SqlQuery("DELETE FROM Tournaments");
            //if (context.Database.Connection.State != System.Data.ConnectionState.Closed)
            //{
            //    context.Database.Connection.Close();
            //}
            //context.Database.Delete();

            //foreach (var user in context.Users.ToList())
            //{
            //    DeleteUser(user);
            //}

            //foreach (var tournament in context.Tournaments.ToList())
            //{
            //    DeleteTournament(tournament);
            //}

            //foreach (var bracket in context.Brackets.ToList())
            //{
            //    DeleteBracket(bracket);
            //}

            //foreach (var rule in context.TournamentRules.ToList())
            //{
            //    DeleteTournamentRules(rule);
            //}


        }

        // For testing purposes only.
        // Call this function to re-seed the database.
        public void Seed()
        {

        }

        // Don't use this yet
        //public List<int> Search(object obj, string keywords)
        //{
        //    List<int> list = new List<int>();
        //    var _keywords = keywords.Split(',');


        //    if (obj.GetType() == typeof(UserModel))
        //    {    
        //        UserModel user = (UserModel)obj;
        //        var result = context.Users.SqlQuery("SELECT Username FROM dbo.Users",)
        //    }
        //}

        #region Users

        public DbError UserExists(UserModel user)
        {
            UserModel _user;
            try
            {
                _user = context.Users.Find(user.UserID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                throw;
            }
            if (user == null)
                return DbError.DOES_NOT_EXIST;
            else
                return DbError.EXISTS;
        }

        public DbError UserEmailExists(string email)
        {
            try
            {
                UserModel user = context.Users.Single(u => u.Email == email);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }

            return DbError.EXISTS;
        }

        // Checks to see if the username exists.
        // Returns DOES_NOT_EXIST if the username does exist.
        public DbError UserUsernameExists(string username)
        {
            try
            {
                UserModel user = context.Users.Single(u => u.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError AddUser(UserModel user)
        {
            try
            {
                user.CreatedOn = DateTime.Now;
                context.Users.Add(user);
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

        // Updates LastLogin of passed-in user.
        public DbError LogUserIn(UserModel user)
        {
            try
            {
                user.LastLogin = DateTime.Now;
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

        public DbError UpdateUser(UserModel user)
        {
            try
            {
                UserModel _user = context.Users.Find(user.UserID);
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

        public DbError DeleteUser(UserModel user)
        {
            UserModel _user = context.Users.Find(user.UserID);
            try
            {
                context.Users.Remove(_user);
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

        public UserModel GetUserById(int id)
        {
            UserModel user = new UserModel();
            try
            {
                user = context.Users.Find(id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                user.UserID = -1;
            }
            return user;
        }

        public UserModel GetUserByUsername(string username)
        {
            UserModel user = new UserModel();
            try
            {
                user = context.Users.Single(x => x.Username == username);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                user.UserID = -1;
                return user;
            }
            return user;
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                users = context.Users.Include(x => x.Tournaments).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                users.Clear();
                WriteException(ex);
                users.Add(new UserModel() { UserID = -1 });
            }
            return users;
        }

        #endregion

        #region Tournaments

        public DbError TournamentExists(TournamentModel tournament)
        {
            try
            {
                TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public List<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                tournaments = context.Tournaments.Include(x => x.Brackets).Include(x => x.Users).ToList();
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

        public DbError AddTournament(TournamentModel tournament)
        {
            TournamentRuleModel _rules = new TournamentRuleModel();
            TournamentModel _tournament = new TournamentModel();
            try
            {
                _tournament = tournament;
                _rules = tournament.TournamentRules;
                context.TournamentRules.Add(_rules);
                _tournament.CreatedOn = DateTime.Now;
                _tournament.LastEditedOn = DateTime.Now;
                context.SaveChanges();
                context.Tournaments.Add(_tournament);
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

        [Obsolete("Use AddUserToTournament(UserInTournamentModel user)")]
        public DbError AddUserToTournament(TournamentModel tournament, UserModel user, Permission permission)
        {
            try
            {
                context.UsersInTournaments.Add(new UserInTournamentModel() { TournamentID = tournament.TournamentID, UserID = user.UserID, Permission = permission });
                context.Tournaments.Include(x => x.Users).Load();
                context.Tournaments.Include(x => x.Users).Single(x => x.TournamentID == tournament.TournamentID).Users.Add(user);
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

        public DbError AddUserToTournament(UserInTournamentModel user)
        {
            try
            {
                context.UsersInTournaments.Add(new UserInTournamentModel() { TournamentID = user.TournamentID, UserID = user.UserID, Permission = user.Permission });
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

        public TournamentModel GetTournamentById(int id)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.Tournaments.Single(t => t.TournamentID == id);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournament.TournamentID = -1;
            }
            return tournament;
        }

        [Obsolete("Use 'Users' collection of tournament")]
        public List<UserModel> GetAllUsersInTournament(TournamentModel tournament)
        {
            List<UserModel> list = new List<UserModel>();

            try
            {
                list = tournament.Users.ToList();
            }
            catch (Exception)
            {
                list.Clear();
                list.Add(new UserModel() { UserID = 0 });
                return list;
            }

            return list;
        }

        public DbError UpdateTournament(TournamentModel tournament)
        {
            try
            {
                TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
                context.Entry(_tournament).CurrentValues.SetValues(tournament);
                context.Entry(_tournament.TournamentRules).CurrentValues.SetValues(tournament.TournamentRules);
                foreach (BracketModel bracket in tournament.Brackets)
                {
                    UpdateBracket(bracket);
                }
                //context.Entry(_tournament.Brackets).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ce)
            {
                interfaceException = ce;
                WriteException(ce);
                return DbError.CONCURRENCY_ERROR;

            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(TournamentModel tournament)
        {
            try
            {
                TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
                if (_tournament.TournamentRules != null)
                {
                    TournamentRuleModel _rule = context.TournamentRules.Find(tournament.TournamentRules.TournamentRulesID);
                    DeleteTournamentRules(_rule);
                }
                foreach (var bracket in _tournament.Brackets.ToList())
                {
                    DeleteBracket(bracket);
                }
                foreach (var user in _tournament.Users.ToList())
                {
                    RemoveUserFromTournament(_tournament, user);
                }
                context.Tournaments.Remove(_tournament);
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

        public DbError RemoveUserFromTournament(TournamentModel tournament, UserModel user)
        {
            try
            {
                context.UsersInTournaments.Remove(context.UsersInTournaments.Single(x => x.TournamentID == tournament.TournamentID && x.UserID == user.UserID));
                TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
                UserModel _user = context.Users.Find(user.UserID);
                context.Tournaments.Include(x => x.Users).Single(x => x.TournamentID == tournament.TournamentID).Users.Remove(_user);
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

        /// <summary>
        /// Takes in a Dictionary of strings in which the key is the name of the parameter being searched for
        /// and the value is the value of that parameter.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();

            try
            {
                List<SqlParameter> sqlparams = new List<SqlParameter>();
                string query = string.Empty;
                //string tournamentIdQuery = "SELECT TournamentID FROM dbo.Tournaments AS Tournament  ";
                //string rulesIdQuery = "SELECT TournamentID FROM TournamentRules WHERE TournamentStartDate = @StartDate";

                //string[] queries = new string[] { "Tournament.Title = @Title", "Tournament.StartDate = @StartDate" };  
                if (searchParams.ContainsKey("Title"))
                {
                    sqlparams.Add(new SqlParameter("@Title", searchParams["Title"]));
                    query = "SELECT TournamentID FROM Tournaments WHERE Title = @Title";
                }
                if (searchParams.ContainsKey("StartDate"))
                {
                    sqlparams.Add(new SqlParameter("@StartDate", DateTime.Parse(searchParams["TournamentStartDate"])));
                    if (searchParams.ContainsKey("Title"))
                        query += "UNION SELECT TournamentID FROM TournamentRules WHERE TournamentStartDate = @StartDate";
                    else
                        query = "SELECT TournamentID FROM TournamentRules WHERE TournamentStartDate = @StartDate";
                }
                tournaments = context.Tournaments.SqlQuery(query, sqlparams).ToList();
                //if (searchParams.ContainsKey("GameTypeID"))
                //{
                //    sqlparams.Add(new SqlParameter("@Game", searchParams["Game"]));
                //    foreach (var tournament in tournaments)
                //    {
                //        if (tournament)
                //    }
                //}




                //List < TournamentModel > _tournaments = context.Tournaments.SqlQuery("SELECT * FROM dbo.Tournaments WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();

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
            //TournamentRuleModel rules = tournament.TournamentRules;
            try
            {
                List<TournamentModel> _tournaments = context.Tournaments.SqlQuery("SELECT * FROM dbo.Tournaments WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();
                foreach (var _tournament in _tournaments)
                {
                    if (_tournament.TournamentRules.TournamentStartDate == startDate)
                        tournaments.Add(_tournament);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        public List<TournamentModel> FindTournaments(string title)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();

            try
            {
                tournaments = context.Tournaments.SqlQuery("SELECT * FROM dbo.Tournaments WHERE Title LIKE @Title", new SqlParameter("@Title", "%" + title + "%")).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        public List<TournamentModel> FindUser(UserModel userInfo)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            List<UserModel> users = new List<UserModel>();
            try
            {
                users = context.Users.SqlQuery("SELECT * FROM dbo.Users WHERE Username LIKE @Username AND FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    new SqlParameter("@Username", "%" + userInfo.Username + "%"),
                    new SqlParameter("@FirstName", "%" + userInfo.FirstName + "%"),
                    new SqlParameter("@LastName", "%" + userInfo.LastName + "%")).ToList();
                foreach (var user in users)
                {
                    foreach (var tournament in user.Tournaments)
                    {
                        tournaments.Add(tournament);
                    }
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

        #region TournamentRules

        public DbError TournamentHasRules(TournamentModel tournament)
        {
            try
            {
                TournamentRuleModel tr = tournament.TournamentRules;
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        [Obsolete("Use AddRules(ref TournamentRuleModel tournamentRules, TournamentModel tournament).")]
        public DbError AddRulesToTournament(TournamentModel tounrnament, TournamentRuleModel tournamentRules)
        {
            try
            {
                context.TournamentRules.Add(tournamentRules);
                tounrnament.TournamentRules = tournamentRules;
                tournamentRules.TournamentID = tounrnament.TournamentID;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError AddRules(ref TournamentRuleModel tournamentRules, TournamentModel tournament)
        {
            TournamentRuleModel rules = new TournamentRuleModel();
            try
            {
                rules = tournamentRules;
                context.TournamentRules.Add(tournamentRules);
                //context.SaveChanges();

                tournamentRules = rules;
                tournament.TournamentRules = tournamentRules;
                //tournamentRules.TournamentID = tournament.TournamentID;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                interfaceException = ex;
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError UpdateRules(TournamentRuleModel tournamentRules)
        {
            try
            {
                TournamentRuleModel _tournamentRules = context.TournamentRules.Find(tournamentRules.TournamentRulesID);
                context.Entry(_tournamentRules).CurrentValues.SetValues(tournamentRules);
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

        public DbError DeleteTournamentRules(TournamentRuleModel tournamentRules)
        {
            try
            {
                TournamentRuleModel _rules = context.TournamentRules.Find(tournamentRules.TournamentRulesID);
                context.TournamentRules.Remove(_rules);
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

        #region Brackets
        public DbError BracketExists(BracketModel bracket)
        {
            try
            {
                BracketModel _bracket = context.Brackets.Find(bracket.BracketID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        // Adds the passed-in bracket to the databaase and also adds the bracket to the passed-in tournament's list of brackets
        public DbError AddBracket(ref BracketModel bracket, TournamentModel tournament)
        {
            try
            {
                context.Brackets.Add(bracket);
                tournament.Brackets.Add(bracket);
                bracket.Tournament = tournament;
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

        public BracketModel GetBracketByID(int id)
        {
            BracketModel bracket = new BracketModel();
            try
            {
                bracket = context.Brackets.Single(b => b.BracketID == id);
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

        public DbError UpdateBracket(BracketModel bracket, bool updateMatches = true)
        {
            try
            {
                BracketModel _bracket = context.Brackets.Find(bracket.BracketID);
                context.Entry(_bracket).CurrentValues.SetValues(bracket);
                if (updateMatches)
                {
                    foreach (var match in bracket.Matches)
                    {
                        match.Challenger = context.Users.Find(match.ChallengerID);
                        match.Defender = context.Users.Find(match.DefenderID);
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

        public DbError DeleteBracket(BracketModel bracket)
        {
            try
            {
                BracketModel _bracket = context.Brackets.Find(bracket.BracketID);
                foreach (var match in bracket.Matches.ToList())
                {
                    DeleteMatch(match);
                }
                context.Brackets.Remove(_bracket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion

        #region Matches

        public DbError MatchExists(MatchModel match)
        {
            try
            {
                MatchModel _match = context.Matches.Find(match.MatchID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.DOES_NOT_EXIST;
            }
            return DbError.EXISTS;
        }

        public DbError UpdateMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = context.Matches.Find(match.MatchID);
                _match.Challenger = context.Users.Find(match.ChallengerID);
                _match.Defender = context.Users.Find(match.DefenderID);
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

        // Adds the passed-in match to the database and also adds it to the passed-in bracket's list of matches.
        [Obsolete]
        public DbError AddMatch(MatchModel match, BracketModel bracket)
        {
            MatchModel _match = new MatchModel();
            throw new Exception("Don't Call this Function");
            try
            {
                _match = match;

                _match.Challenger = context.Users.Find(match.ChallengerID);
                _match.Defender = context.Users.Find(match.DefenderID);

                //context.Matches.Load();
                context.Users.Load();
                context.Matches.Add(_match);
                context.Entry(_match).CurrentValues.SetValues(match);
                context.SaveChanges();
                bracket.Matches.Add(_match);
                //context.SaveChanges();
                context.Tournaments.Include(x => x.Brackets).Load();
                //context.Users.Include(x => x.UserID).Load();
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.ERROR;
            }
            return DbError.SUCCESS;
        }

        public MatchModel GetMatchById(int id)
        {
            MatchModel match = new MatchModel();
            try
            {
                match = context.Matches.Find(id);
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

        public DbError DeleteMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = context.Matches.Find(match.MatchID);
                context.Matches.Remove(_match);
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

        #region BracketSeeds

        [Obsolete]
        public DbError SetUserBracketSeed(UserBracketSeedModel userBracketSeed)
        {
            throw new Exception("Don't use this function");
            try
            {
                context.UserBracketSeeds.Add(userBracketSeed);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                interfaceException = ex;
                return DbError.ERROR;
            }

            return DbError.SUCCESS;
        }

        // Gets the user's seed in the passed-in bracket
        // Returns -1 if the user is not found in the bracket
        public int GetUserSeedInBracket(UserModel user, BracketModel bracket)
        {
            UserBracketSeedModel ubs = new UserBracketSeedModel();
            int seed;
            try
            {
                seed = context.UserBracketSeeds.Single(e => e.UserID == user.UserID && e.BracketID == bracket.BracketID).Seed;
            }
            catch (Exception ex)
            {
                WriteException(ex);
                interfaceException = ex;
                return -1;
            }
            return seed;
        }

        public DbError DeleteUserBracketSeed(UserBracketSeedModel userBracketSeed)
        {
            try
            {
                context.UserBracketSeeds.Remove(context.UserBracketSeeds.Single(e => e.BracketID == userBracketSeed.BracketID &&
                e.TournamentID == userBracketSeed.TournamentID &&
                e.UserID == userBracketSeed.UserID));
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

        #region Teams

        public DbError AddTeam(TeamModel team)
        {
            try
            {
                context.Teams.Add(team);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                team.TeamID = -1;
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public List<TeamModel> GetAllTeams()
        {
            List<TeamModel> teams = new List<TeamModel>();
            try
            {
                teams = context.Teams.Include(x => x.TeamMembers).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                throw;
            }
            return teams;
        }

        public DbError DeleteTeam(TeamModel team)

        {
            TeamModel _team = context.Teams.Find(team.TeamID);
            try
            {
                foreach (var teamMember in _team.TeamMembers.ToList())
                {
                    DeleteTeamMember(teamMember);
                }
                context.Teams.Remove(_team);
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

        #region TeamMembers

        // Takes in a fully filled out TeamMember and adds it to the database
        public DbError AddTeamMember(TeamMemberModel teamMember)
        {
            try
            {
                context.TeamMembers.Add(teamMember);
                //UserModel user = new UserModel();
                //user = context.Users.Find(teamMember.User.UserID);

                teamMember.User.Teams.Add(context.Teams.Find(teamMember.Team.TeamID));
                //teamMember.User.Teams.Add(teamMember.Team);
                context.Teams.Find(teamMember.TeamID).TeamMembers.Add(teamMember);
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

        // Removes a TeamMember from a Team without deleteing them from the database
        public DbError RemoveTeamMemeber(TeamMemberModel teamMember)
        {
            try
            {
                context.Teams.Find(teamMember.TeamID).TeamMembers.Remove(teamMember);
                teamMember.DateLeft = DateTime.Now;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                throw;
                return DbError.FAILED_TO_REMOVE;
            }
            return DbError.SUCCESS;
        }

        // Removes a TeamMember from a Team and deletes them from the database
        public DbError DeleteTeamMember(TeamMemberModel teamMember)
        {
            TeamMemberModel _teamMember = new TeamMemberModel();
            try
            {
                _teamMember = context.TeamMembers.Single(x => x.Team.TeamID == teamMember.TeamID && x.User.UserID == teamMember.UserID);
                context.TeamMembers.Remove(_teamMember);
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

        #region Permissions

        public Permission GetUserSitePermission(UserModel user)
        {
            Permission permission = Permission.NONE;
            try
            {
                permission = context.Users.Find(user.UserID).SitePermission.Permission;
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return permission;
            }
            return permission;
        }

        public Permission GetUserTournamentPermission(UserModel user, TournamentModel tournament)
        {
            Permission permission = Permission.NONE;
            try
            {
                context.Users.Load();
                context.Tournaments.Load();
                context.UsersInTournaments.Load();
                permission = context.UsersInTournaments.SingleOrDefault(e => e.UserID == user.UserID && e.TournamentID == tournament.TournamentID).Permission;
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return permission;
            }
            return permission;
        }

        public DbError UpdateUserTournamentPermission(UserModel user, TournamentModel tournament, Permission permission)
        {
            UserInTournamentModel uitm = new UserInTournamentModel();
            try
            {
                uitm = context.UsersInTournaments.Where(x => x.TournamentID == tournament.TournamentID && x.UserID == user.UserID).Single();
                uitm.Permission = permission;
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

        public DbError UpdateUserSitePermission(UserModel user, Permission permission)
        {
            try
            {
                UserModel _user = context.Users.Find(user.UserID);
                _user.SitePermission.Permission = permission;
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


        [Obsolete("Use GetUserTournamentPermission or GetUserSitePermission")]
        public Permission GetUserPermission(UserModel user, TournamentModel tournament) // Rename and seperate all get permission calls/functions
        {
            Permission permission = new Permission();
            try
            {

                //permission = context.UsersInTournaments.Include(x => x.Tournament).Include(x => x.User).Single().Permission;
                //permission = context.UsersInTournaments.Where(x => x.UserID == user.UserID && x.TournamentID == tournament.TournamentID).First().Permission;
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
            }
            return permission;
        }

        #endregion

        #region BracketTypes

        public List<BracketTypeModel> GetAllBracketTypes()
        {
            List<BracketTypeModel> types = new List<BracketTypeModel>();
            try
            {
                types = context.BracketTypes.ToList();
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

        #region Games

        public DbError AddGame(MatchModel match, GameModel game)
        {
            try
            {
                context.Games.Add(game);
                match.Games.Add(game);
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

        public DbError UpdateGame(GameModel game)
        {
            try
            {
                GameModel _game = context.Games.Find(game.GameID);
                context.Entry(_game).CurrentValues.SetValues(game);
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

        public List<GameModel> GetAllGamesInMatch(MatchModel match)
        {
            List<GameModel> games = new List<GameModel>();
            try
            {
                games = match.Games.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                games.Clear();
                games.Add(new GameModel() { GameID = -1 });
            }
            return games;
        }

        [Obsolete("Use DeleteGame(GameModel game)")]
        public DbError DeleteGame(MatchModel match, GameModel game)
        {
            try
            {
                match.Games.Remove(game);
                GameModel _game = context.Games.Find(game.GameID);
                context.Games.Remove(_game);
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

        public DbError DeleteGame(GameModel game)
        {
            try
            {
                GameModel _game = context.Games.Find(game.GameID);
                context.Games.Remove(_game);
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
                context.GameTypes.Add(gameType);
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
                GameTypeModel _gameType = context.GameTypes.Find(gameType.GameTypeID);
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
                gameTypes = context.GameTypes.ToList();
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

        public DbError DeleteGameType(GameTypeModel gameType)
        {
            GameTypeModel _gameType = context.GameTypes.Find(gameType.GameTypeID);
            try
            {
                context.GameTypes.Remove(_gameType);
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


        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DatabaseInterface() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }
}
