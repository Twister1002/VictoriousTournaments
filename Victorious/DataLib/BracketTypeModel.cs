using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{

    public partial class BracketTypeModel
    {


        public enum BracketType
        {
            SINGLE = 1,
            DOUBLE,
            ROUNDROBIN,
            RRGROUP,
            GSLGROUP
        }
        public BracketTypeModel()
        {
            //BracketTypeID = (int)Type;

        }

        [Key]
        public int BracketTypeID { get; set; }

        public string TypeName { get; set; }

        public BracketType Type { get; set; }

        public int NumberOfGroups { get; set; }
    }
}
