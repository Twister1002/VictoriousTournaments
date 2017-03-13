using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
  
    public partial class TeamModel
    {
        public TeamModel()
        {
            TeamMembers = new Collection<TeamMemberModel>();
        }

        [Key]
        public int TeamID { get; set; }

        public string TeamName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public virtual ICollection<TeamMemberModel> TeamMembers { get; set; }






    }
}
