using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDebugConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            DatabaseInterface db = new DatabaseInterface();
            db.Clear();
            //db.Clear();
            int userId = db.AddUser("Ryan", "Kelton", "keltonr01@gmail.com", "keltonr01", "password", "9542344919");
            //if (db.AddUser("Ryan", "Kelton", "keltonr01@gmail.com", "keltonr01", "password", "9542344919") != 0)
            //{

            //}
            int tournamentId = db.AddTournament("Tournament 1", "Test tournament", int.Parse(db.GetUserByUsername("keltonr01")["UserID"]));
            db.AddByeMatch(tournamentId, 1, userId);

            int bracketId = db.AddBracket("Single Elimination");

            int trId = db.AddTournamentRules(tournamentId, 4, 0, 0, 16, true, 1, new DateTime(2017, 3, 1), new DateTime(2017, 3, 3), new DateTime(2017, 3, 10));

            if (trId == 0)
            {
                Console.WriteLine("ERROR");
            }

            //Dictionary<string, string> dict = db.GetUserByUsername("keltonr01");

            //int userId;
            //int tournamentId = 0;
            //if (int.TryParse(db.GetUserByUsername("keltonr01")["UserID"], out userId))
            //{
            //    tournamentId = db.AddTournament("Tournament 1", "Test tournament", userId);
            //}
            //if (tournamentId == 0)
            //{
            //    Console.WriteLine("Error");
            //}
            //Dictionary<string, string> dict = db.GetTournamentById(tournamentId);

            //if (dict.ContainsKey("error"))
            //{
            //    Console.WriteLine("Error");
            //}
            //Console.WriteLine(tournamentId);

            //Console.WriteLine(dict["Title"]);
            //Console.WriteLine(dict["Description"]);
            //Console.WriteLine(db.GetUserById(int.Parse(dict["CreatedByID"]))["Username"]);

            //db.UpdateUserEmail(int.Parse(db.GetUserByUsername("keltonr01")["UserID"]), "keltonr01@fullsail.edu");

            //if (db.UserEmailExists("keltonr01@gmail.com"))
            //    Console.WriteLine("email exists");
            //else
            //    Console.WriteLine("email available");


            //db.AddUserToTournament(1, 1);

            //List<int> list = db.GetAllUsersInTournament(1);
            //foreach(int val in list)
            //{
            //    Console.WriteLine(db.GetUserById(val)["Username"]);
            //}

            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
}
