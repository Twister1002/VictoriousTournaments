using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public partial class BracketTypeModel
    {

        [Key]
        public int BracketTypeID { get; set; }

        public string TypeName { get; set; }
    }
}
