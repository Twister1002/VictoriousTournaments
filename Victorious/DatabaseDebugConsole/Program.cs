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

            //db.AddUser("Ryan", "Kelton", "keltonr01@gmail.com", "keltonr01", "password", "9542344919");

            //Dictionary<string, string> dict = db.GetUserByUsername("keltonr01");



            int id = db.AddTournament("Tournament 1", "Test tournament", int.Parse(db.GetUserByUsername("keltonr01")["UserID"]));
            Dictionary<string, string> dict = db.GetTournamentById(id);

            Console.WriteLine(id);

            Console.WriteLine(dict["Title"]);
            Console.WriteLine(dict["Description"]);
            Console.WriteLine(db.GetUserById(int.Parse(dict["CreatedByID"]))["Username"]);



            Console.ReadLine();

        }
    }
}
