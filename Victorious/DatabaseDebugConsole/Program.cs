﻿using DataLib;
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

            //DeleteAllTournaments(db);
            //DeleteAllUsers(db);

            //Seed(db);

            UserModel user = db.GetUserById(1);
            PrintUser(db, user);
            user.Username = "TestUsername";
            user.FirstName = "Ryan";
            user.LastName = "Kelton";
            user.PhoneNumber = "(123) 456-789";     
            db.UpdateUser(user);
            PrintUser(db, user);

            //TournamentModel tournament = new TournamentModel();
            //tournament = db.GetAllTournaments()[0];
            //UserModel user = new UserModel();
            //user = db.GetAllUsers()[0];
            //BracketModel bracket = new BracketModel();
            //bracket = user.Tournaments.ElementAt(0).Brackets.ElementAt(0);
            //PrintBracket(db, bracket);



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
            TournamentRuleModel rules = new TournamentRuleModel()
            {
                RegistrationStartDate = DateTime.Now
            };
            tournament.TournamentRules = rules;

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
                BracketTypeID = 1
            };
            db.AddBracket(ref bracket, tournament);

            BracketModel bracket2 = new BracketModel()
            {
                BracketTitle = "Bracket 2",
                BracketTypeID = 2
            };
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

        static void PrintUser(DatabaseInterface db, UserModel user)
        {
            Console.WriteLine(user.UserID);
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Password);
            Console.WriteLine(user.PhoneNumber);
            Console.WriteLine(user.Email);
            Console.WriteLine("Number of active tournaments: " + user.Tournaments.Count);
            
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

        static void Encrypt()
        {

        }
    }
}
