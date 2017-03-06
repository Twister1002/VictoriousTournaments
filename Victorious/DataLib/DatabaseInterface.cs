using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{

    public enum DbError
    {
        ERROR = -1, NONE = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS
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
            if (context.Database.Connection.State != System.Data.ConnectionState.Closed)
            {
                context.Database.Connection.Close();
            }
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
            UserModel _user;
            try
            {
                _user = context.Users.Find(user.UserID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption " + ex.ToString() + " in UserExists");
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
                Console.WriteLine("Exception " + ex.ToString() + " in UserEmailExists");
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
                Console.WriteLine("Exception " + ex.ToString() + " in UserUsernameExists");
                return DbError.DOES_NOT_EXIST;

            }

            return DbError.EXISTS;
        }

        // Adds user to the users table. DOES NOT ASSIGN USER TO TOURNAMENT.
        // Returns database ID of passed-in user.
        public int AddUser(ref UserModel user)
        {
            try
            {
                user.CreatedOn = DateTime.Now;
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in AddUser");
                return -1;
            }

            return user.UserID;
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
                Console.WriteLine("Exception " + ex.ToString() + " in LogUserIn");
                return DbError.FAILED_TO_UPDATE;
            }

            return DbError.SUCCESS;
        }

        public DbError UpdateUser(UserModel user)
        {
            try
            {
                UserModel _user = context.Users.Find(user.UserID);
                _user = user;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateUser");
                return DbError.FAILED_TO_UPDATE;
                throw;
            }

            return DbError.SUCCESS;
        }

        // Sets the email of the passed-in user to the newEmail.
        // Returns ERROR if the email failed to update.
        [Obsolete("Depricated. Use UpdateUser.")]
        public DbError UpdateUserEmail(UserModel user, string newEmail)
        {
            try
            {
                user.Email = newEmail;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateUserEmail");
                return DbError.ERROR;
            }
            return DbError.SUCCESS;
        }

        [Obsolete("Depricated. Use UpdateModel.")]
        public DbError UpdateUserPassword(UserModel user, string newPassword)
        {
            try
            {
                user.Password = newPassword;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateUserPassword");
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
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in DeleteUser");
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
            catch (Exception)
            {
                user.UserID = -1;
            }

            return user;
        }

        public UserModel GetUserByUsername(string username)
        {
            UserModel user = new UserModel();
            try
            {
                user = context.Users.SingleOrDefault(u => u.Username == username);
                //if (user.Password == null || user.Email == null || user.FirstName == null || user.LastName == null)
                //    throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in GetUserByUsername");
                user.UserID = -1;
                return user;
            }

            return user;
        }

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

        // Adds the passed-in tournament to the database
        public DbError AddTournament(ref TournamentModel tournament)
        {
            
            try
            {
                
                context.Tournaments.Add(tournament);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in AddTournament");

                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError AddUserToTournament(TournamentModel tournament, UserModel user)
        {
            try
            {
                context.Users.Add(user);
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

        public List<UserModel> GetAllUsersInTournament(TournamentModel tournament)
        {
            List<UserModel> list = new List<UserModel>();

            try
            {
                //foreach (UserModel user in tournament.Users)
                //{
                //    list.Add(user.UserID);
                //}

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
                _tournament = tournament;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateTournament");
                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        [Obsolete("Depricated. Use UpdateTournament.")]
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

        [Obsolete("Depricated. User UpdateTournament.")]
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

        [Obsolete("Depricated. User UpdateTournament.")]
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
                return DbError.FAILED_TO_DELETE;
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

        public DbError DeleteTournamentRules(TournamentRuleModel tournamentRules)
        {
            try
            {
                context.TournamentRules.Attach(tournamentRules);
                context.TournamentRules.Remove(tournamentRules);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in DeleteTournamentRules");
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
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

        [Obsolete("Depricated. Use AddBracket that takes in a BracketModel refernece")]
        public int AddBracket(TournamentModel tournament, BracketModel bracket)
        {
            try
            {
                context.Brackets.Add(bracket);
                tournament.Brackets.Add(bracket);
                bracket.Tournament = tournament;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }

            return bracket.BracketID;
        }

        // Adds the passed-in bracket to the databaase and also adds the bracket to the passed-in tournament's list of brackets
        public DbError AddBracket(ref BracketModel bracket, TournamentModel tournament)
        {
            try
            {
                context.Brackets.Add(bracket);
                tournament.Brackets.Add(bracket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in AddBracket");
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
            catch (Exception)
            {
                bracket.BracketID = -1;
                return bracket;
            }

            return bracket;
        }

        [Obsolete("Depricated. Use DeleteBracket.")]
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

        public DbError DeleteBracket(BracketModel bracket)
        {
            try
            {
                context.Brackets.Attach(bracket);
                context.Brackets.Remove(bracket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in DeleteBracket");
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
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

        [Obsolete("Depricated. Use AddMatch that takes a match and bracket.")]
        public DbError AddMatch(ref MatchModel match)
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

        [Obsolete("Depricated. Use UpdateMatch.")]
        public DbError AddDefender(MatchModel match, UserModel user)
        {
            try
            {
                match.Defender = user;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        [Obsolete("Depricated. Use UpdateMatch.")]
        public DbError AddChallenger(MatchModel match, UserModel user)
        {
            try
            {
                match.Challenger = user;
                context.SaveChanges();
            }
            catch
            {
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError UpdateMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = context.Matches.Find(match.MatchID);
                _match = match;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateMatch");
                return DbError.FAILED_TO_UPDATE;
               
            }
            return DbError.SUCCESS;
        }

        // Adds the passed-in match to the database and also adds it to the passed-in bracket's list of matches.
        public DbError AddMatch(ref MatchModel match, BracketModel bracket)
        {
            try
            {
                context.Matches.Add(match);
                bracket.Matches.Add(match);
                context.SaveChanges();
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                match.MatchID = -1;
                return match;
            }

            return match;
        }

        public DbError DeleteMatch(MatchModel match)
        {
            try
            {
                context.Matches.Attach(match);
                context.Matches.Remove(match);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in DeleteMatch");
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        #endregion

        public DbError SetUserBracketSeed(UserModel user, BracketModel bracket, int seed)
        {
            UserBracketSeedModel ubs = new UserBracketSeedModel();
            try
            {
                ubs.User = user;
                ubs.Bracket = bracket;
                ubs.Tournament = bracket.Tournament;
                ubs.Seed = seed;

                context.UserBracketSeeds.Add(ubs);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excetption " + ex.ToString() + " in SetUserBracketSeed");
                return DbError.ERROR;              
            }

            return DbError.SUCCESS;
        }
        
        // Gets the user's seed in the passed-in bracket
        // Returns -1 if the user is not found in the bracket
        public int GetUserSeedInBracket(UserModel user, BracketModel bracket)
        {
            UserBracketSeedModel ubs = new UserBracketSeedModel();
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in GetUserSeedInBracket");
                return -1;
            }

            return ubs.Seed.Value;
        }


    }
}
