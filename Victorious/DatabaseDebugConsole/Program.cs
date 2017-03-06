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

namespace DatabaseDebugConsole
{
    class Program
    {

        static void Main(string[] args)
        {

            DatabaseInterface db = new DatabaseInterface();

            //db.Clear();
            //Seed(db);
            TournamentModel tournament = db.GetTournamentById(1);

            UserBracketSeedModel ubs = new UserBracketSeedModel();

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

            UserModel user = db.GetUserById(1);
            Console.WriteLine(user.Username);
            //user.Username = "testUsername";
            //db.UpdateUser(user);
            //Console.WriteLine(user.Username);

            //foreach (var match in tournament.Brackets.ElementAt(0).Matches)
            //{
            //    Console.WriteLine(match.MatchNumber);
            //}

            Console.WriteLine("Done");
            Console.ReadLine();

        }


        static void Seed(DatabaseInterface db)
        {
            TournamentModel tournament = new TournamentModel()
            {
                Title = "Tournament 1",
                Description = "Test Tournament",
                CreatedByID = 1
            };
            db.AddTournament(ref tournament);


            using (StreamReader reader = new StreamReader("..\\..\\Random User Info.txt"))
            {
                for (int i = 0; i < 10; i++)
                {
                    string firstName = reader.ReadLine().Split(':')[1];
                    string lastName = reader.ReadLine().Split(':')[1];
                    string email = reader.ReadLine().Split(':')[1];
                    string username = reader.ReadLine().Split(':')[1];
                    string password = reader.ReadLine().Split(':')[1];
                    string phoneNumber = reader.ReadLine().Split(':')[1];
                    reader.ReadLine();
                    UserModel user = new UserModel() { FirstName = firstName, LastName = lastName, Email = email, Username = username, Password = password, PhoneNumber = phoneNumber };
                    db.AddUserToTournament(tournament, user);
                    //db.AddUser(user);
                }
            }     

            BracketModel bracket = new BracketModel()
            {
                BracketTitle = "Bracket 1",
                BracketType = "Double Elimination"
            };
            //db.AddBracket(tournament, bracket);
            db.AddBracket(ref bracket, tournament);

            BracketModel bracket2 = new BracketModel()
            {
                BracketTitle = "Bracket 2",
                BracketType = "Double Elimination"
            };
            //db.AddBracket(tournament, bracket);
            db.AddBracket(ref bracket2, tournament);


            for (int i = 1; i < 11; i += 2)
            {
                MatchModel match = new MatchModel();
                UserModel challenger = new UserModel();
                UserModel defender = new UserModel();
                challenger = db.GetUserById(i);
                match.Challenger = challenger;
                defender = db.GetUserById(i + 1);
                //db.AddDefender(match, defender);
                match.Defender = defender;
                db.UpdateMatch(match);
                //db.AddMatch(match, db.GetBracketByID(1));
                db.AddMatch(ref match, bracket);
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

        static void Encrypt()
        {

        }
    }
}
