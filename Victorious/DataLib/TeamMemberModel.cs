using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public enum TeamMemberRole
    {
       Standard = 0, Captain
    }

  
    public partial class TeamMemberModel
    {
        [Key]
        [Column(Order = 1)]
        public int UserID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int TeamID { get; set; }

        public Permission? Permission { get; set; }

        public DateTime? DateJoined { get; set; }

        public DateTime? DateLeft { get; set; }

        [ForeignKey("UserID")]
        public UserModel User { get; set; }

        [ForeignKey("TeamID")]
        public TeamModel Team { get; set; }



    }
}


