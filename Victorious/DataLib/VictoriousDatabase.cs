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

        public bool UserExists(int id)
        {
            User user = context.Users.Find(id);
            if (user == null)
                return false;
            else
                return true;
        }

        public void AddUser(string firstName, string lastName, string email, string username, string password, string phoneNumber)
        {
            User user = new User()
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

        public void LogUserInById(int id)
        {
            User user = context.Users.SingleOrDefault(u => u.UserID == id);
            user.LastLogin = DateTime.Now;

            context.SaveChanges();
        }
        public void LogUserInByUsername(string username)
        { 
            User user = context.Users.SingleOrDefault(u => u.Username == username);
            user.LastLogin = DateTime.Now;

            context.SaveChanges();
        }
        public void DeleteUserById(int id)
        {
            User user = new User() { UserID = id };
            context.Users.Attach(user);
            context.Users.Remove(user);

            context.SaveChanges();
        }

        public Dictionary<string,string> GetUserById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            User user = context.Users.Find(id);
            dict.Add("FirstName", user.FirstName);
            dict.Add("LastName", user.LastName);
            dict.Add("Username", user.Username);
            dict.Add("Email", user.Email);
            dict.Add("PhoneNumber", user.PhoneNumber);
            dict.Add("LastLogin", user.LastLogin.ToString());

            return dict;         
        }

        public Dictionary<string, string> GetUserByUsername(string username)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            User user = context.Users.SingleOrDefault(u => u.Username == username);
            dict.Add("UserID", user.UserID.ToString());
            dict.Add("FirstName", user.FirstName);
            dict.Add("LastName", user.LastName);
            dict.Add("Username", user.Username);
            dict.Add("Email", user.Email);
            dict.Add("PhoneNumber", user.PhoneNumber);
            dict.Add("LastLogin", user.LastLogin.ToString());

            return dict;
        }

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
            Tournament tournament = new Tournament()
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

            return tournament.TournamentID;
        }

        public Dictionary<string,string> GetTournamentById(int id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Tournament tournament = context.Tournaments.SingleOrDefault(t => t.TournamentID == id);
            dict.Add("Title", tournament.Title);
            dict.Add("Description", tournament.Description);
            dict.Add("CreatedByID", tournament.CreatedByID.ToString());
            dict.Add("CreatedOn", tournament.CreatedOn.ToString());

            return dict;
        }

        public void DeleteTournamentById(int id)
        {
            
            Tournament tournament = new Tournament() { TournamentID = id };
            context.Tournaments.Attach(tournament);
            context.Tournaments.Remove(tournament);
           
            context.SaveChanges();
        }


       

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

        public void DeletBracketById(int id)
        {
            Bracket bracket = new Bracket() { BracketID = id };
            context.Brackets.Attach(bracket);
            context.Brackets.Remove(bracket);

            context.SaveChanges();
        }

    }
}
