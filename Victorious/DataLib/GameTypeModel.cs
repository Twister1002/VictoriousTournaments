using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{

    public partial class GameTypeModel
    {
        [Key]
        public int GameTypeID { get; set; }

        public string Title { get; set; }

    }
}
