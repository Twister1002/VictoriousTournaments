using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Sql;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace DatabaseDebugConsole
{
    class Program
    {

        static void Main(string[] args)
        {

            DatabaseInterface db = new DatabaseInterface();


            #region Old Crap

            //db.Clear();

            //TournamentModel tournament = db.GetTournamentById(1);

            //UserBracketSeedModel ubs = new UserBracketSeedModel();
            //for (int i = 0; i < 5; i++)
            //{
            //    TournamentModel tournament = new TournamentModel()
            //    {
            //        Title = "Tournament " + i.ToString(),
            //        Description = "Test Tournament",
            //        CreatedByID = i
            //    };
            //    db.AddTournament(ref tournament);

            //}
            //foreach (var user in tournament.Users)
            //{
            //    Console.WriteLine(user.FirstName + ' ' + user.LastName);
            //}

            //List<UserModel> users = new List<UserModel>();
            //users = db.GetAllUsersInTournament(tournament);

            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.FirstName + ' ' + user.LastName);
            //}

            //tournament.Users.ElementAt(0).Email = "keltonr01@gmail.com";
            //db.UpdateUserEmail(tournament.Users.ElementAt(0), "keltonr01@gmail.com");
            //Console.WriteLine(tournament.Users.ElementAt(0).Email);

            //db.SetUserBracketSeed(db.GetUserById(0), db.GetBracketByID(0), 1);

            //PrintAllMatches(db, tournament);

            //UserModel user = db.GetUserById(1);
            //Console.WriteLine(user.Username);

            //List<TournamentModel> tournaments = new List<TournamentModel>(db.GetAllTournaments());
            //foreach (var t in tournaments)
            //{
            //    Console.WriteLine(t.CreatedByID);
            //}
            //user.Username = "testUsername";
            //db.UpdateUser(user);
            //Console.WriteLine(user.Username);

            //foreach (var match in tournament.Brackets.ElementAt(0).Matches)
            //{
            //    Console.WriteLine(match.MatchNumber);
            //}

            //if (db.TournamentHasRules(tournament) == DbError.EXISTS)
            //{
            //    Console.WriteLine("Yes");
            //}

            //TournamentRuleModel rules = new TournamentRuleModel()
            //{
            //    StartDate = DateTime.Now
            //};
            //db.AddRules(ref rules, tournament);
            //Console.WriteLine(tournament.Users.ElementAt(0).Email);

            //if (db.TournamentHasRules(tournament) == DbError.EXISTS)
            //{
            //    Console.WriteLine(tournament.TournamentRules.TournamentBeginsDate.ToString());
            //}

            //foreach (var tournament in db.GetAllTournaments())
            //{
            //    PrintAllMatches(db, tournament);
            //}

            //TournamentModel tournament = new TournamentModel()
            //{
            //    Title = "Tournament Rules Test",
            //    Description = "Test Tournament",
            //    CreatedByID = 1
            //};
            //TournamentRuleModel rules = new TournamentRuleModel()
            //{
            //    RegistrationStartDate = DateTime.Now
            //};

            //tournament.TournamentRules = rules;

            //db.AddTournament(ref tournament);



            //BracketModel bracket = new BracketModel()
            //{
            //    BracketTitle = "Test Bracket"
            //};
            //BracketTypeModel type = new BracketTypeModel()
            //{
            //    Type = BracketTypeModel.BracketType.SINGLE
            //};
            //bracket.BracketType = type;
            //db.AddBracket(ref bracket, tournament);

            //Console.WriteLine(db.GetAllTournaments()[0].Brackets.ToList()[0].BracketType.Type.ToString());





            //PrintUser(db, user);
            //user.Username = "TestUsername";
            //user.FirstName = "Ryan";
            //user.LastName = "Kelton";
            //user.PhoneNumber = "(123) 456-789";
            //db.UpdateUser(user);
            //PrintUser(db, user);

            //db.AddUserToTournament(db.GetAllTournaments()[0], user, Permission.TOURNAMENT_ADMINISTRATOR);
            //db.RemoveUserFromTournament(db.GetAllTournaments()[0], user);

            //PrintAllUsersInTournament(db, db.GetAllTournaments()[0]);

            //PrintAllUsersInTournament(db, db.GetAllTournaments()[1]);

            //TournamentModel tournament = new TournamentModel();
            //tournament = db.GetAllTournaments()[0];
            //UserModel user = new UserModel();
            //user = db.GetAllUsers()[0];
            //tournament = user.Tournaments.ToList()[0];
            //Console.WriteLine(db.GetUserPermission(user, tournament).ToString());
            //db.SetUserTournamentPermission(user, tournament, Permission.TOURNAMENT_ADMINISTRATOR);
            //db.UpdateUserTournamentPermission(user, tournament, Permission.TOURNAMENT_STANDARD);
            //Console.WriteLine(db.GetUserPermission(user, tournament).ToString());

            //db.RemoveUserFromTournament(tournament, user);

            //Console.WriteLine(tournament.Users.Count);

            //BracketModel bracket = new BracketModel();
            //bracket = user.Tournaments.ElementAt(0).Brackets.ElementAt(0);
            //PrintBracket(db, bracket);


            //PrintAllBracketsInTournament(db, db.GetAllTournaments()[0]);
            //CreateTeam(db);
            //Console.WriteLine(db.GetAllTeams()[0].TeamMembers.Count);
            #endregion

            //DeleteAllTournaments(db);
            //DeleteAllUsers(db);

            //Seed(db);
            TournamentModel tournament = new TournamentModel();
            //tournament = db.GetAllTournaments()[0];
            tournament.Title = "Tournament 1";
            TournamentModel foundTournament = db.FindTournaments(tournament, true)[0];
            Console.WriteLine(foundTournament.Title);
            //UserModel user = new UserModel();
            //user = db.GetUserById(5);
            //db.RemoveUserFromTournament(tournament, user);
            //PrintAllUsersInTournament(db, tournament);

            //BracketModel bracket = tournament.Brackets.ToList()[0];
            //bracket.BracketTitle = "Test Bracket";
            //db.UpdateBracket(bracket);


            //db.AddUserToTournament(db.GetAllTournaments().ToList()[1], user, Permission.TOURNAMENT_ADMINISTRATOR);

            //PrintUser(db, user);
            //user.Username = "TestUsername";
            //user.FirstName = "Ryan";
            //user.LastName = "Kelton";
            //user.PhoneNumber = "(123) 456-789";
            //db.UpdateUser(user);
            //PrintUser(db, user);


            //List<UserModel> allUsers = db.GetAllUsers().ToList();
            //List<TeamModel> allTeams = db.GetAllTeams().ToList();

            //List<UserModel> allUsers = db.GetAllUsers().ToList();
            //List<TeamModel> allTeams = db.GetAllTeams().ToList();
            //for (int i = 0; i < 4; i++)
            //{
            //    CreateTeamMember(db, allUsers[i], allTeams[0]);
            //}
            //for (int i = 4; i < 8; i++)
            //{
            //    CreateTeamMember(db, allUsers[i], allTeams[1]);
            //}

            //PrintAllUsersOnTeam(db, allTeams[0]);

            //PrintAllUsersOnTeam(db, allTeams[1]);
            //CreateTeams(db, 5);
            //CreateTeamMember(db, user);
            //Console.WriteLine(db.GetAllTeams()[0].TeamMembers.Count);

            //db.DeleteTeam(db.GetAllTeams()[0]);

            //DeleteAllTeams(db);

            Console.WriteLine("Done");
            Console.ReadLine();



        }


        static void Seed(DatabaseInterface db)
        {
            TournamentModel tournament = new TournamentModel()
            {
                Title = "Tournament 1",
                Description = "Test Tournament 1",
                CreatedByID = 1
            };
            TournamentRuleModel rules = new TournamentRuleModel()
            {
                RegistrationStartDate = DateTime.Now
            };
            tournament.TournamentRules = rules;
            TournamentModel tournament2 = new TournamentModel()
            {
                Title = "Tournament 2",
                Description = "Test Tournament 2",
                CreatedByID = 1
            };
            TournamentRuleModel rules2 = new TournamentRuleModel()
            {
                RegistrationStartDate = DateTime.Now
            };
            tournament2.TournamentRules = rules2;

            db.AddTournament(ref tournament);
            db.AddTournament(ref tournament2);


            using (StreamReader reader = new StreamReader("..\\..\\Random User Info.txt"))
            {
                for (int i = 0; i < 16; i++)
                {
                    string firstName = reader.ReadLine().Split(':')[1];
                    string lastName = reader.ReadLine().Split(':')[1];
                    string email = reader.ReadLine().Split(':')[1];
                    string username = reader.ReadLine().Split(':')[1];
                    string password = reader.ReadLine().Split(':')[1];
                    string phoneNumber = reader.ReadLine().Split(':')[1];
                    reader.ReadLine();
                    UserModel user = new UserModel() { FirstName = firstName, LastName = lastName, Email = email, Username = username, Password = password, PhoneNumber = phoneNumber };
                    if (i == 0)
                        db.AddUserToTournament(tournament, user, Permission.TOURNAMENT_ADMINISTRATOR);
                    else
                        db.AddUserToTournament(tournament, user, Permission.TOURNAMENT_STANDARD);

                    //db.AddUser(user);
                }
            }



            BracketModel bracket = new BracketModel()
            {
                BracketTitle = "Single Elimination Bracket",
                BracketTypeID = 1
            };
            db.AddBracket(ref bracket, tournament);

            BracketModel bracket2 = new BracketModel()
            {
                BracketTitle = "Double Elimination Bracket",
                BracketTypeID = 2
            };
            db.AddBracket(ref bracket2, tournament);


            for (int i = 1; i < 17; i += 2)
            {
                MatchModel match = new MatchModel();
                UserModel challenger = new UserModel();
                UserModel defender = new UserModel();
                match.RoundIndex = 1;
                challenger = db.GetUserById(i);
                match.Challenger = challenger;
                defender = db.GetUserById(i + 1);
                //db.AddDefender(match, defender);
                match.Defender = defender;
                //db.UpdateMatch(match);
                //db.AddMatch(match, db.GetBracketByID(1));
                if (i < 9)
                {
                    db.AddMatch(ref match, bracket);
                }
                else
                {
                    db.AddMatch(ref match, bracket2);
                }
            }

        }

        static void PrintUser(DatabaseInterface db, UserModel user)
        {
            Console.WriteLine(user.UserID);
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Password);
            Console.WriteLine(user.PhoneNumber);
            Console.WriteLine(user.Email);
            Console.WriteLine(db.GetUserPermission(user, user.Tournaments.ToList()[0]).ToString());
            Console.WriteLine("Number of active tournaments: " + user.Tournaments.Count);
            Console.WriteLine("Number of active teams: " + user.Teams.Count);

        }

        static void PrintAllUsersInTournament(DatabaseInterface db, TournamentModel tournament)
        {
            foreach (var user in tournament.Users)
            {
                PrintUser(db, user);
            }
        }

        static void PrintAllBracketsInTournament(DatabaseInterface db, TournamentModel tournament)
        {
            foreach (var bracket in tournament.Brackets)
            {
                PrintBracket(db, bracket);
            }
        }

        static void PrintBracket(DatabaseInterface db, BracketModel bracket)
        {
            Console.WriteLine("Title: " + bracket.BracketTitle);
            Console.WriteLine("Bracket Type: " + bracket.BracketType.Type.ToString());
            foreach (var match in bracket.Matches)
            {
                Console.WriteLine("Match number: " + match.MatchNumber);
                Console.WriteLine("Challenger: " + match.Challenger.FirstName + ' ' + match.Challenger.LastName);
                Console.WriteLine("Defender: " + match.Defender.FirstName + ' ' + match.Defender.LastName);
            }
        }

        static void PrintAllMatches(DatabaseInterface db, TournamentModel tournament)
        {
            foreach (var bracket in tournament.Brackets)
            {
                foreach (var match in bracket.Matches)
                {
                    Console.WriteLine("Match number: " + match.MatchNumber);
                    Console.WriteLine("Challenger: " + match.Challenger.FirstName + ' ' + match.Challenger.LastName);
                    Console.WriteLine("Defender: " + match.Defender.FirstName + ' ' + match.Defender.LastName);
                }

            }
        }

        static void PrintAllUsersOnTeam(DatabaseInterface db, TeamModel team)
        {
            foreach (var teamMember in team.TeamMembers)
            {
                PrintUser(db, teamMember.User);
            }
        }

        static void DeleteAllTournaments(DatabaseInterface db)
        {

            List<TournamentModel> tournaments = db.GetAllTournaments();

            foreach (var tournament in tournaments)
            {
                Console.WriteLine(tournament.Title);
                if (db.DeleteTournament(tournament) == DbError.FAILED_TO_DELETE)
                {
                    Console.WriteLine("Error");
                }
            }
        }

        static void DeleteAllUsers(DatabaseInterface db)
        {
            List<UserModel> users = db.GetAllUsers();

            foreach (var user in users)
            {
                Console.WriteLine(user.FirstName + ' ' + user.LastName);
                if (db.DeleteUser(user) == DbError.FAILED_TO_DELETE)
                {
                    Console.WriteLine("Error");
                }
            }
        }

        static void DeleteAllTeams(DatabaseInterface db)
        {
            List<TeamModel> teams = db.GetAllTeams();

            foreach (var team in teams)
            {
                Console.WriteLine("Deleting: " + team.TeamName);
                db.DeleteTeam(team);
            }
        }

        static void CreateTeams(DatabaseInterface db, int count)
        {
            for (int i = 0; i < count; i++)
            {
                TeamModel team = new TeamModel()
                {
                    TeamName = "Test Team" + i
                };
                db.AddTeam(ref team);
            }

        }

        static void CreateTeamMember(DatabaseInterface db, UserModel user, TeamModel team)
        {
            TeamMemberModel teamMember = new TeamMemberModel()
            {
                User = user,
                Permission = Permission.TEAM_CAPTAIN,
                DateJoined = DateTime.Now,
                Team = team
            };
            db.AddTeamMember(teamMember);
        }




    }
}
