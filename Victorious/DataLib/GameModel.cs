using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public partial class GameModel
    {
        [Key]
        public int GameID { get; set; }

        public int ChallengerID { get; set; }

        public int DefenderID { get; set; }

        public int WinnerID { get; set; }

        public int? MatchID { get; set; }

        public int GameNumber { get; set; }

        public int ChallengerScore { get; set; }

        public int DefenderScore { get; set; }


    }
}
