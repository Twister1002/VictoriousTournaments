using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{

    // Use FAILED_TO_DELETE only when something is being deleted from database, otherwise user FAILED_TO_ADD
    public enum DbError
    {
        ERROR = -1, NONE = 0, SUCCESS, FAILED_TO_ADD, FAILED_TO_REMOVE, FAILED_TO_UPDATE, FAILED_TO_DELETE, TIMEOUT, DOES_NOT_EXIST, EXISTS
    };

    public class DatabaseInterface
    {
        VictoriousDbContext context = new VictoriousDbContext();
        public Exception interfaceException;
        public DatabaseInterface()
        {

            if (context.BracketTypes.Find(1) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 1, Type = BracketTypeModel.BracketType.SINGLE });
            }
            if (context.BracketTypes.Find(2) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 2, Type = BracketTypeModel.BracketType.DOUBLE });
            }
            if (context.BracketTypes.Find(3) == null)
            {
                context.BracketTypes.Add(new BracketTypeModel() { BracketTypeID = 3, Type = BracketTypeModel.BracketType.ROUNDROBIN });
            }


            context.Tournaments
                .Include(x => x.Brackets)
                .Include(x => x.Users)
                .Load();
            //context.Users
            //    .Include(x => x.Tournaments)
            //    .Load();
            context.Brackets
                .Include(x => x.Matches)
                .Load();
            context.Matches
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
                interfaceException = ex;
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
                interfaceException = ex;
                Console.WriteLine("Exception " + ex.ToString() + " in UserUsernameExists");
                return DbError.DOES_NOT_EXIST;

            }

            return DbError.EXISTS;
        }

        public DbError AddUser(ref UserModel user)
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
                Console.WriteLine("Exception " + ex.ToString() + " in AddUser");
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
                interfaceException = ex;
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateUser");
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
            catch (Exception ex)
            {
                interfaceException = ex;
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
                interfaceException = ex;
                Console.WriteLine("Exception " + ex.ToString() + " in GetUserByUsername");
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
                Console.WriteLine("Exceltion " + ex.ToString() + " in GetAllUsers");
                users.Add(new UserModel() { UserID = 0 });
            }
            return users;
        }

        public DbError SetUserTournamentPermission(UserModel user, TournamentModel tournament, Permission permission)
        {
            UsersInTournamentsModel uitm = new UsersInTournamentsModel();
            try
            {
                context.UsersInTournaments.Add(new UsersInTournamentsModel() { User = user, Tournament = tournament, Permission = permission });
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return DbError.ERROR;
                throw;
            }
            return DbError.ERROR;
        }

        public DbError UpdateUserTournamentPermission(UserModel user, TournamentModel tournament, Permission permission)
        {
            UsersInTournamentsModel uitm = new UsersInTournamentsModel();
            try
            {
                uitm = context.UsersInTournaments.Where(x => x.UserID == user.UserID && x.TournamentID == tournament.TournamentID).Single();
                uitm.Permission = permission;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return DbError.FAILED_TO_UPDATE;
                throw;
            }
            return DbError.ERROR;
        }

        #endregion

        #region Tournaments

        public DbError TournamentExists(TournamentModel tournament)
        {
            TournamentModel _tournament = context.Tournaments.Find(tournament.TournamentID);
            if (tournament == null)
                return DbError.DOES_NOT_EXIST;
            else
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
                tournaments.Clear();
                Console.WriteLine("Exception " + ex.ToString() + " in GetAllTournaments");
                tournaments.Add(new TournamentModel() { TournamentID = 0 });
            }
            return tournaments;
        }

        public DbError AddTournament(ref TournamentModel tournament)
        {
            TournamentRuleModel _rules = new TournamentRuleModel();
            TournamentModel _tournament = new TournamentModel();
            try
            {
                // Tournament.TournamentRules.TournamentID isn't being properly set on insert of tournament, will be fixed
                _tournament = tournament;
                _rules = tournament.TournamentRules;
                context.TournamentRules.Add(_rules);
                context.SaveChanges();
                context.Tournaments.Add(tournament);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                Console.WriteLine("Exception " + ex.ToString() + " in AddTournament");
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        [Obsolete("Use AddUserToTournament(TournamentModel tournament, UserModel user, Permission permission")]
        public DbError AddUserToTournament(TournamentModel tournament, UserModel user)
        {
            try
            {
                //context.Users.Add(user);
                tournament.Users.Add(user);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                interfaceException = ex;
                return DbError.FAILED_TO_ADD;
            }

            return DbError.SUCCESS;
        }

        public DbError AddUserToTournament(TournamentModel tournament, UserModel user, Permission permission)
        {
            try
            {
                context.UsersInTournaments.Add(new UsersInTournamentsModel() { Tournament = tournament, User = user, Permission = permission });
                //context.Users.Add(user);
                tournament.Users.Add(user);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                interfaceException = ex;
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
            catch (Exception ex)
            {
                interfaceException = ex;
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
                //context.Entry(tournament).State = System.Data.Entity.EntityState.Deleted;
                //context.Tournaments.Attach(_tournament);
                context.Tournaments.Remove(_tournament);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in DeleteTournament");
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public DbError RemoveUserFromTournament(TournamentModel tournament, UserModel user)
        {

            try
            {
                tournament.Users.Remove(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return DbError.FAILED_TO_REMOVE;
                throw;
            }
            return DbError.SUCCESS;

        }


        #endregion

        #region TournamentRules

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
                Console.WriteLine("Exception " + ex.ToString() + " in AddRules");
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError UpdateRules(TournamentRuleModel tournamentRules)
        {
            try
            {
                TournamentRuleModel _tournamentRules = context.TournamentRules.Find(tournamentRules.TournamentID);
                _tournamentRules = tournamentRules;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in UpdateRules");
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
                //context.TournamentRules.Attach(tournamentRules);
                //context.TournamentRules.Remove(tournamentRules);
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

        #region Brackets
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

        public DbError UpdateBracket(BracketModel bracket)
        {
            try
            {
                BracketModel _bracket = context.Brackets.Find(bracket.BracketID);
                _bracket = bracket;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception" + ex.ToString() + " in UpdateBracket");
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
                //context.Brackets.Attach(bracket);
                context.Brackets.Remove(_bracket);
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

        #region BracketTypes



        #endregion

        #region Match

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

        [Obsolete("Use AddMatch(ref MatchModel match, BracketModel bracket).")]
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

        [Obsolete("Use UpdateMatch.")]
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

        [Obsolete("Use UpdateMatch.")]
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

        [Obsolete("Use AddMatch(ref MatchModel match, BracketModel bracket)")]
        public int AddMatch(MatchModel match, BracketModel bracket)
        {
            try
            {
                bracket.Matches.Add(match);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return -1;

            }

            return match.MatchID;
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
                MatchModel _match = context.Matches.Find(match.MatchID);
                //context.Matches.Attach(match);
                context.Matches.Remove(_match);
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

        #region BracketSeeds

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

        #endregion

        #region Teams

        public DbError AddTeam(ref TeamModel team)
        {
            try
            {
                context.Teams.Add(team);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.ToString() + " in AddTeam");
                team.TeamID = -1;
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTeam(TeamModel team)
        {
            TeamModel _team = context.Teams.Find(team.TeamID);
            try
            {
                foreach (var teamMember in _team.TeamMembers.ToList())
                {

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

        public DbError AddTeamMember(ref TeamMemberModel teamMember)
        {
            try
            {
                context.TeamMembers.Add(teamMember);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                Console.WriteLine("Exception " + ex.ToString() + " in AddTeam");
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleeteTeamMember(TeamMemberModel teamMember)
        {
            TeamMemberModel _teamMember = new TeamMemberModel();
            try
            {
                _teamMember = context.TeamMembers.Find(teamMember.UserID);
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

        public Permission GetUserPermission(UserModel user, TournamentModel tournament)
        {
            Permission permission;
            try
            {
                permission = context.UsersInTournaments.Where(x => x.UserID == user.UserID && x.TournamentID == tournament.TournamentID).Single().Permission;
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            return permission;
        }

        //public Permission GetUserPermission(UserModel user, TeamModel team)
        //{

        //}

        #endregion

        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }



    }
}
