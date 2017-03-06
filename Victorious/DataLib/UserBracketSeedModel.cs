using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataLib
{
    [Table("UserBracketSeeds")]
    public partial class UserBracketSeedModel : DbModel
    {
        [Key]
        [Column(Order = 1)]
        public int? UserID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int? TournamentID { get; set; }

        [Key]
        [Column(Order = 3)]
        public int? BracketID { get; set; }

        public int? Seed { get; set; }

        [ForeignKey("UserID")]
        public virtual UserModel User { get; set; }

        [ForeignKey("TournamentID")]
        public virtual TournamentModel Tournament { get; set; }

        [ForeignKey("BracketID")]
        public virtual BracketModel Bracket { get; set; }



    }
}
