namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bracket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bracket()
        {
            
        }

        public int BracketID { get; set; }

        [Required]
        [StringLength(50)]
        public string BracketType { get; set; }

      
        
    }
}
