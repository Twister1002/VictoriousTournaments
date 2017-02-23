using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
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

        public bool UserExists(int id)
        {
            UserModel user = context.Users.Find(id);
            if (user == null)
                return false;
            else
                return true;
        }

        public bool UserEmailExists(string email)
        {
            try
            {
                UserModel user = context.Users.Single(u => u.Email == email);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool UserUsernameExists(string username)
        {
            UserModel user = context.Users.Single(u => u.Username == username);
            if (user == null)
                return false;
            else
                return true;
        }

        public int AddUser(string firstName, string lastName, string email, string username, string password, string phoneNumber)
        {
            UserModel user;
            try
            {
                user = new UserModel()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Username = username,
                    Password = password,
                    PhoneNumber = phoneNumber,
                    CreatedOn = DateTime.Now,
                    LastLogin = DateTime.Now
                };
                context.Users.Add(user);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            return user.UserID;
        }

        public bool LogUserInById(int id)
        {
            try
            {
                UserModel user = context.Users.SingleOrDefault(u => u.UserID == id);
                user.LastLogin = DateTime.Now;

                context.SaveChanges();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        public bool LogUserInByUsername(string username)
        {
            try
            {
                UserModel user = context.Users.SingleOrDefault(u => u.Username == username);
                user.LastLogin = DateTime.Now;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool UpdateUserEmail(int id, string newEmail)
        {
            try
            {
                UserModel user = context.Users.Single(u => u.UserID == id);
                user.Email = newEmail;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool DeleteUserById(int id)
        {
            try
            {
                UserModel user = new UserModel() { UserID = id };
                context.Users.Attach(user);
                context.Users.Remove(user);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public Dictionary<string, string> GetUserById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                UserModel user = context.Users.Find(id);
                dict.Add("UserID", user.UserID.ToString());
                dict.Add("FirstName", user.FirstName);
                dict.Add("LastName", user.LastName);
                dict.Add("Username", user.Username);
                dict.Add("Email", user.Email);
                dict.Add("PhoneNumber", user.PhoneNumber);
                dict.Add("LastLogin", user.LastLogin.ToString());
            }
            catch (Exception)
            {
                dict.Clear();
                dict.Add("UserID", "ERROR");
                return dict;
            }

            return dict;
        }

        public Dictionary<string, string> GetUserByUsername(string username)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                UserModel user = context.Users.SingleOrDefault(u => u.Username == username);
                dict.Add("UserID", user.UserID.ToString());
                dict.Add("FirstName", user.FirstName);
                dict.Add("LastName", user.LastName);
                dict.Add("Username", user.Username);
                dict.Add("Email", user.Email);
                dict.Add("PhoneNumber", user.PhoneNumber);
                dict.Add("LastLogin", user.LastLogin.ToString());
            }
            catch (Exception)
            {
                dict.Clear();
                dict.Add("UserID", "ERROR");
                return dict;
            }

            return dict;
        }

        #endregion

        #region Tournaments Logic

        public bool TournamentExists(int id)
        {
            TournamentModel tournament = context.Tournaments.Find(id);
            if (tournament == null)
                return false;
            else
                return true;
        }

        public int AddTournament(string title, string description, int createdById)
        {
            TournamentModel tournament;
            try
            {
                tournament = new TournamentModel()
                {
                    Title = title,
                    Description = description,
                    CreatedByID = createdById,
                    CreatedOn = DateTime.Now,
                    LastEditedByID = createdById,
                    LastEditedOn = DateTime.Now
                };

                context.Tournaments.Add(tournament);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            return tournament.TournamentID;
        }

        public bool AddUserToTournament(int tournamentId, int userId)
        {
            TournamentModel tournament;
            try
            {
                tournament = context.Tournaments.Single(t => t.TournamentID == tournamentId);
                UserModel user = context.Users.Single(u => u.UserID == userId);
                tournament.Users.Add(user);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Dictionary<string, string> GetTournamentById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {
                TournamentModel tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);
                dict.Add("TournamentID", tournament.TournamentID.ToString());
                dict.Add("Title", tournament.Title);
                dict.Add("Description", tournament.Description);
                dict.Add("CreatedByID", tournament.CreatedByID.ToString());
                dict.Add("CreatedOn", tournament.CreatedOn.ToString());


            }
            catch (Exception)
            {
                dict.Clear();
                dict.Add("TournamentID", "ERROR");
                return dict;
            }
            return dict;

        }

        public List<int> GetAllUsersInTournament(int id)
        {
            List<int> list = new List<int>();
            TournamentModel tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);
            try
            {
                foreach (UserModel user in tournament.Users)
                {
                    list.Add(user.UserID);
                }
            }
            catch (Exception)
            {

                list.Clear();
                list.Add(0);
                return list;
            }

            return list;
        }

        public DateTime GetTournamentCutoffDate(int id)
        {
            TournamentModel tour = context.Tournaments.Find(id);
            DateTime time;
            try
            {
                time = tour.TournamentRules.CutoffDate.Value;
            }
            catch (Exception)
            {
                time = DateTime.MinValue;
                return time;
            }

            return time;
        }

        public DateTime GetTournamentStartDate(int id)
        {
            TournamentModel tour = context.Tournaments.Find(id);
            DateTime time;
            try
            {
                time = tour.TournamentRules.StartDate.Value;
            }
            catch (Exception)
            {
                time = DateTime.MinValue;
                return time;
            }

            return time;
        }

        public DateTime GetTournamentEndDate(int id)
        {
            TournamentModel tour = context.Tournaments.Find(id);
            DateTime time;
            try
            {
                time = tour.TournamentRules.EndDate.Value;
            }
            catch (Exception)
            {
                time = DateTime.MinValue;
                return time;
            }

            return time;
        }

        public bool DeleteTournamentById(int id)
        {
            try
            {
                TournamentModel tournament = new TournamentModel() { TournamentID = id };
                context.Tournaments.Attach(tournament);
                context.Tournaments.Remove(tournament);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region TournamentRules Logic

        public bool TournamentRulesExist(int id)
        {
            TournamentRuleModel tournamentRule = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
            if (tournamentRule == null)
                return false;
            else
                return true;
        }

        public int AddTournamentRules(int tournamentId, int numberOfRounds, decimal entryFee, decimal prizePurse, int numberOfPlayers, bool isPublic,
            int bracketTypeId, DateTime cutoffDate, DateTime startDate, DateTime endDate)
        {
            TournamentRuleModel tr;
            try
            {
                tr = new TournamentRuleModel();
                tr.TournamentID = tournamentId;
                tr.NumberOfRounds = numberOfRounds;
                tr.EntryFee = entryFee;
                tr.PrizePurse = prizePurse;
                tr.NumberOfPlayers = numberOfPlayers;
                tr.IsPublic = isPublic;
                tr.BracketID = bracketTypeId;
                tr.CutoffDate = cutoffDate;
                tr.StartDate = endDate;
                tr.EndDate = endDate;
                if (entryFee == 0)
                    tr.HasEntryFee = false;
                else
                    tr.HasEntryFee = true;
                
                context.TournamentRules.Add(tr);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            return tr.TournamnetRulesID;
        }

        public Dictionary<string, string> GetTournamentRulesById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            TournamentRuleModel tr = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
            dict.Add("TournamentID", tr.TournamentID.ToString());
            dict.Add("NumberOfRounds", tr.NumberOfRounds.ToString());
            dict.Add("NumberOfPlayers", tr.NumberOfPlayers.ToString());
            dict.Add("EntryFee", tr.EntryFee.ToString());
            dict.Add("PrizePurse", tr.PrizePurse.ToString());
            dict.Add("IsPublic", tr.IsPublic.ToString());
            dict.Add("BracketID", tr.BracketID.ToString());
            dict.Add("CutoffDate", tr.CutoffDate.ToString());
            dict.Add("StartDate", tr.StartDate.ToString());
            dict.Add("EndDate", tr.EndDate.ToString());

            return dict;
        }

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

        public bool MatchExists(int id)
        {
            MatchModel match = context.Matches.Find(id);
            if (match == null)
                return false;
            else
                return true;
        }

        public int AddMatch(int tournamentId, int challengerId, int defenderId, int roundNumber, DateTime startDateTime, DateTime endDateTime, TimeSpan matchDuration)
        {
            MatchModel match;
            try
            {
                match = new MatchModel()
                {
                    TournamentID = tournamentId,
                    RoundNumber = roundNumber,
                    ChallengerID = challengerId,
                    DefenderID = defenderId,
                    StartDateTime = startDateTime,
                    EndDateTime = startDateTime,
                    IsBye = false
                };

                match.Challenger = context.Users.Find(challengerId);
                match.Defender = context.Users.Find(defenderId);
                match.Tournament = context.Tournaments.Find(tournamentId);

                context.Matches.Add(match);

                context.SaveChanges();

            }
            catch (Exception)
            {
                return 0;
            }

            return match.MatchID;

        }

        public int AddByeMatch(int tournamentId, int roundNumber, int userId)
        {
            MatchModel match;
            try
            {
                match = new MatchModel()
                {
                    TournamentID = tournamentId,
                    RoundNumber = roundNumber,
                    WinnerID = userId,
                    IsBye = true
                };

                match.Winner = context.Users.Find(userId);
                match.Tournament = context.Tournaments.Find(tournamentId);

                context.Matches.Add(match);

                context.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
                throw;
            }

            return match.MatchID;
        }

        public Dictionary<string, string> GetMatchById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                MatchModel match = context.Matches.Find(id);
                dict.Add("MatchID", match.MatchID.ToString());
                dict.Add("TournamentId", match.TournamentID.ToString());
                dict.Add("ChallengerId", match.ChallengerID.ToString());
                dict.Add("DefenderId", match.DefenderID.ToString());
                dict.Add("RoundNumber", match.RoundNumber.ToString());
                dict.Add("StartDateTime", match.StartDateTime.ToString());
                dict.Add("EndDateTime", match.EndDateTime.ToString());
                dict.Add("MatchDuration", match.MatchDuration.ToString());
                dict.Add("IsBye", match.IsBye.ToString());
                dict.Add("ChallengerScore", match.ChallengerScore.ToString());
                dict.Add("DefenderScore", match.DefenderScore.ToString());
            }
            catch (Exception)
            {
                dict.Clear();
                dict.Add("MatchID", "ERROR");
                return dict;
            }

            return dict;
        }

        #endregion

    }
}
