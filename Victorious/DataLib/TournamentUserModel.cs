using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public partial class TournamentUserModel
    {
        public TournamentUserModel()
        {
            AccountID = -1;
            Username = string.Empty;
            Postion = string.Empty;
            UniformNumber = -1;
            Seed = -1;
        }

        [Key]
        public int TournamentUserModelID { get; set; }

        public int TournamentModelID { get; set; }

        public int AccountID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Postion { get; set; }

        public int UniformNumber { get; set; }

        public int Seed { get; set; }

    }
}
