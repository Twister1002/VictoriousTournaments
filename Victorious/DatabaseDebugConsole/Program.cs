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
            VictoriousDatabase db = new VictoriousDatabase();

            //db.Clear();
            //db.AddUser("Ryan", "Kelton", "keltonr01@gmail.com", "keltonr01", "password", "9542344919");
            //if (db.AddUser("Ryan", "Kelton", "keltonr01@gmail.com", "keltonr01", "password", "9542344919") != 0)
            //{

            //}
            db.AddTournament("Tournament 1", "Test tournament", int.Parse(db.GetUserByUsername("keltonr01")["UserID"]));

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




            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
}
