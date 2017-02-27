namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

 
    public partial class BracketModel
    {
        public const int BracketTypeLength = 50;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BracketModel()
        {         
        }

        [Key]
        public int BracketID { get; set; }

        [StringLength(BracketTypeLength)]
        public string BracketType { get; set; }

    }
}
