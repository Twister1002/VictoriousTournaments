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

        public enum BracketType
        {
            SINGLE,
            DOUBLE,
            ROUNDROBIN
        }

        [Key]
        public int BracketTypeID { get; set; } // set this to private

        public string TypeName { get; set; } // will be removed

        public BracketType Type { get; set; }
    }
}
