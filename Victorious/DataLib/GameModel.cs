using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{

    public partial class GameModel
    {
        [Key]
        public int GameID { get; set; }

        public string Title { get; set; }

    }
}
