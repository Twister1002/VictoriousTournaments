using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public partial class UserInTournamentModel
    {
        [Key]
        [Column(Order = 1)]
        public int UserID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int TournamentID { get; set; }

        public Permission Permission { get; set; }

        [ForeignKey("UserID")]
        public UserModel User { get; set; }

        [ForeignKey("TournamentID")]
        public TournamentModel Tournament { get; set; }



    }
}
