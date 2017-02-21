using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class VictoriousDatabase
    {
        VictoriousDbContext context = new VictoriousDbContext();

        void VicotriousDatabase()
        {

        }

        // DO NOT EVER CALL THIS FUNCTION OUTSIDE THE DEBUG PROJECT
        public void Clear()
        {
            context.Brackets.SqlQuery("DELETE FROM Brackets");
            context.Matches.SqlQuery("DELETE FROM Matches");
            context.Users.SqlQuery("DELETE FROM Users");
            context.TournamentRules.SqlQuery("DELETE FROM TournamentRules");
            context.Tournaments.SqlQuery("DELETE FROM Tournaments");
        }

        #region Users Logic

        public bool UserExists(int id)
        {
            User user = context.Users.Find(id);
            if (user == null)
                return false;
            else
                return true;
        }

        public bool UserEmailExists(string email)
        {
            try
            {
                User user = context.Users.Single(u => u.Email == email);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool UserUsernameExists(string username)
        {
            User user = context.Users.Single(u => u.Username == username);
            if (user == null)
                return false;
            else
                return true;
        }

        public int AddUser(string firstName, string lastName, string email, string username, string password, string phoneNumber)
        {
            User user;
            try
            {
                user = new User()
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
                User user = context.Users.SingleOrDefault(u => u.UserID == id);
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
                User user = context.Users.SingleOrDefault(u => u.Username == username);
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
                User user = context.Users.Single(u => u.UserID == id);
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
                User user = new User() { UserID = id };
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
                User user = context.Users.Find(id);
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
                dict.Add("error", "error");
                return dict;
            }

            return dict;
        }

        public Dictionary<string, string> GetUserByUsername(string username)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                User user = context.Users.SingleOrDefault(u => u.Username == username);
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
                dict.Add("error", "error");
                return dict;
            }

            return dict;
        }
        #endregion

        #region Tournaments Logic

        public bool TournamentExists(int id)
        {
            Tournament tournament = context.Tournaments.Find(id);
            if (tournament == null)
                return false;
            else
                return true;
        }

        public int AddTournament(string title, string description, int createdById)
        {
            Tournament tournament;
            try
            {
                tournament = new Tournament()
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
            Tournament tournament;
            try
            {
                tournament = context.Tournaments.Single(t => t.TournamentID == tournamentId);
                User user = context.Users.Single(u => u.UserID == userId);
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
            Tournament tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);
            dict.Add("Title", tournament.Title);
            dict.Add("Description", tournament.Description);
            dict.Add("CreatedByID", tournament.CreatedByID.ToString());
            dict.Add("CreatedOn", tournament.CreatedOn.ToString());

            return dict;
        }

        public bool DeleteTournamentById(int id)
        {
            try
            {
                Tournament tournament = new Tournament() { TournamentID = id };
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
            TournamentRule tournamentRule = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
            if (tournamentRule == null)
                return false;
            else
                return true;
        }

        public int AddTournamentRules(int tournamentId, int numberOfRounds, decimal entryFee, decimal prizePurse, int numberOfPlayers, bool isPublic,
            int bracketTypeId, DateTime cutoffDate, DateTime startDate, DateTime endDate)
        {
            TournamentRule tr;
            try
            {
                tr = new TournamentRule();
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
            TournamentRule tr = context.TournamentRules.Single(t => t.TournamnetRulesID == id);
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
            Bracket bracket = context.Brackets.Find(id);
            if (bracket == null)
            {
                return false;
            }
            else
                return true;
        }

        public int AddBracket(string bracketTypeName)
        {
            Bracket bracket;
            try
            {
                bracket = new Bracket()
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
                Bracket bracket = new Bracket() { BracketID = id };
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

    }
}
