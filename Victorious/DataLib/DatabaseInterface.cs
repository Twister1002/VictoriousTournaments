using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{

    public enum DbError
    {
        ERROR = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_REMOVE, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS
    };

    public class DatabaseInterface
    {
        VictoriousDbContext context = new VictoriousDbContext();

        void VicotriousDatabase()
        {

        }

        // DO NOT EVER CALL THIS FUNCTION OUTSIDE THE DEBUG PROJECT
        public void Clear()
        {
            //context.Brackets.SqlQuery("DELETE FROM Brackets");
            //context.Matches.SqlQuery("DELETE FROM Matches");
            //context.Users.SqlQuery("DELETE FROM Users");
            //context.TournamentRules.SqlQuery("DELETE FROM TournamentRules");
            //context.Tournaments.SqlQuery("DELETE FROM Tournaments");

            context.Database.Delete();
        }

        // For testing purposes only.
        // Call this function to re-seed the database.
        public void Seed()
        {

        }

        #region Users Logic

        public DbError UserExists(UserModel user)
        {

            UserModel _user = context.Users.Find(user.UserID);
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
            catch (Exception)
            {
                return DbError.ERROR;
            }

            return DbError.SUCCESS;
        }

        public DbError UserUsernameExists(string username)
        {
            try
            {
                UserModel user = context.Users.Single(u => u.Username == username);
            }
            catch (Exception)
            {
                return DbError.DOES_NOT_EXIST;

            }

            return DbError.EXISTS;
        }

        public DbError AddUser(UserModel user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError LogUserIn(UserModel user)
        {
            try
            {
                user.LastLogin = DateTime.Now;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        //public bool LogUserInByUsername(string username)
        //{
        //    try
        //    {
        //        UserModel user = context.Users.SingleOrDefault(u => u.Username == username);
        //        user.LastLogin = DateTime.Now;

        //        context.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public DbError UpdateUserEmail(UserModel user, string newEmail)
        {
            try
            {
                user.Email = newEmail;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.ERROR;
            }

            return DbError.SUCCESS;
        }

        public DbError DeleteUser(UserModel user)
        {
            try
            {
                context.Users.Attach(user);
                context.Users.Remove(user);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_REMOVE;
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
            catch (Exception)
            {
                user.UserID = -1;
            }

            return user;
        }

        //public Dictionary<string, string> GetUserByUsername(string username)
        //{
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    try
        //    {
        //        UserModel user = context.Users.SingleOrDefault(u => u.Username == username);
        //        dict.Add("UserID", user.UserID.ToString());
        //        dict.Add("FirstName", user.FirstName);
        //        dict.Add("LastName", user.LastName);
        //        dict.Add("Username", user.Username);
        //        dict.Add("Email", user.Email);
        //        dict.Add("PhoneNumber", user.PhoneNumber);
        //        dict.Add("LastLogin", user.LastLogin.ToString());
        //    }
        //    catch (Exception)
        //    {
        //        dict.Clear();
        //        dict.Add("UserID", "ERROR");
        //        return dict;
        //    }

        //    return dict;
        //}

        #endregion

        #region Tournaments Logic

        public DbError TournamentExists(TournamentModel tournament)
        {
            TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
            if (tournament == null)
                return DbError.DOES_NOT_EXIST;
            else
                return DbError.EXISTS;
        }

        public DbError AddTournament(TournamentModel tournament)
        {
            try
            {
                context.Tournaments.Add(tournament);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError AddUserToTournament(TournamentModel tournament, UserModel user)
        {
            try
            {
                tournament.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public TournamentModel GetTournamentById(int id)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);

            }
            catch (Exception)
            {
                tournament.TournamentID = -1;
            }
            return tournament;

        }

        //public List<int> GetAllUsersInTournament(int id)
        //{
        //    List<int> list = new List<int>();
        //    TournamentModel tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);
        //    try
        //    {
        //        foreach (UserModel user in tournament.Users)
        //        {
        //            list.Add(user.UserID);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        list.Clear();
        //        list.Add(0);
        //        return list;
        //    }

        //    return list;
        //}

        public DbError UpdateTournamentCutoffDate(TournamentModel tournament, DateTime newCutoff)
        {
            TournamentModel tour = context.Tournaments.Find(tournament.TournamentID);

            try
            {
                newCutoff = tour.TournamentRules.CutoffDate.Value;
            }
            catch (Exception)
            {
                return DbError.SUCCESS;
            }

            return DbError.SUCCESS;
        }

        public DbError UpdateTournamentStartDate(TournamentModel tournament, DateTime newStartDate)
        {
            TournamentModel tour = context.Tournaments.Find(tournament.TournamentID);
            try
            {
                newStartDate = tour.TournamentRules.StartDate.Value;
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_UPDATE;
            }

            return DbError.SUCCESS;
        }

        public DbError UpdateTournamentEndDate(TournamentModel tournament, DateTime newEndDate)
        {
            try
            {
                tournament.TournamentRules.EndDate = newEndDate;
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_UPDATE;
            }

            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(TournamentModel tournament)
        {
            try
            {
                context.Tournaments.Attach(tournament);
                context.Tournaments.Remove(tournament);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_REMOVE;
            }
            return DbError.SUCCESS;
        }
        #endregion

        #region TournamentRules Logic

        public DbError TournamentHasRules(TournamentModel tournament)
        {
            try
            {
                TournamentRuleModel tr = tournament.TournamentRules;
            }
            catch (Exception)
            {
                return DbError.DOES_NOT_EXIST;
            }

            return DbError.EXISTS;

            //TournamentRuleModel tournamentRule = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
           
        }

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

        //public Dictionary<string, string> GetTournamentRulesById(int id)
        //{
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    TournamentRuleModel tr = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
        //    dict.Add("TournamentID", tr.TournamentID.ToString());
        //    dict.Add("NumberOfRounds", tr.NumberOfRounds.ToString());
        //    dict.Add("NumberOfPlayers", tr.NumberOfPlayers.ToString());
        //    dict.Add("EntryFee", tr.EntryFee.ToString());
        //    dict.Add("PrizePurse", tr.PrizePurse.ToString());
        //    dict.Add("IsPublic", tr.IsPublic.ToString());
        //    dict.Add("BracketID", tr.BracketID.ToString());
        //    dict.Add("CutoffDate", tr.CutoffDate.ToString());
        //    dict.Add("StartDate", tr.StartDate.ToString());
        //    dict.Add("EndDate", tr.EndDate.ToString());

        //    return dict;
        //}

        #endregion

        #region Brackets Logic
        public bool BracketExists(int id)
        {
            BracketModel bracket = context.Brackets.Find(id);
            if (bracket == null)
            {
                return false;
            }
            else
                return true;
        }

        public int AddBracket(string bracketTypeName)
        {
            BracketModel bracket;
            try
            {
                bracket = new BracketModel()
                {
                    BracketType = bracketTypeName
                };

                context.Brackets.Add(bracket);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            return bracket.BracketID;
        }

        public bool DeletBracketById(int id)
        {
            try
            {
                BracketModel bracket = new BracketModel() { BracketID = id };
                context.Brackets.Attach(bracket);
                context.Brackets.Remove(bracket);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Match Logic

        public DbError MatchExists(MatchModel match)
        {
            try
            {
                MatchModel _match = context.Matches.Find(match.MatchID);
            }
            catch (Exception)
            {
                return DbError.DOES_NOT_EXIST;
            }

            return DbError.EXISTS;
        }

        public DbError AddMatch(MatchModel match)
        {
            try
            {
                context.Matches.Add(match);

                context.SaveChanges();

            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;

        }

        //public int AddByeMatch(int tournamentId, int roundNumber, int userId)
        //{
        //    MatchModel match;
        //    try
        //    {
        //        match = new MatchModel()
        //        {
        //            TournamentID = tournamentId,
        //            RoundNumber = roundNumber,
        //            WinnerID = userId,
        //            IsBye = true
        //        };

        //        match.Winner = context.Users.Find(userId);
        //        match.Tournament = context.Tournaments.Find(tournamentId);

        //        context.Matches.Add(match);

        //        context.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //        throw;
        //    }

        //    return match.MatchID;
        //}

        public MatchModel GetMatch(int id)
        {
            MatchModel match = new MatchModel();
            try
            {
                match = context.Matches.Find(id);
            }
            catch (Exception)
            {
                match.MatchID = -1;
                return match;
            }

            return match;
        }

        #endregion

    }
}
