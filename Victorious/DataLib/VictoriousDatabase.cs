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

        public bool TournamentExists(int id)
        {
            return true;
        }

        public void DeleteTournamentById(int id)
        {
            
            Tournament tournament = new Tournament() { TournamentID = id };
            context.Tournaments.Attach(tournament);
            context.Tournaments.Remove(tournament);
           
            context.SaveChanges();
        }

        public void DeleteUserById(int id)
        {

        }


    }
}
