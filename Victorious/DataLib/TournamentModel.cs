namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

 
    public partial class TournamentModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TournamentModel()
        {
                      
        }
        [Key]
        public int TournamentID { get; set; }

        [Required]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedByID { get; set; }

        public int? WinnerID { get; set; }

        public DateTime? LastEditedOn { get; set; }

        public int? LastEditedByID { get; set; }

        public virtual TournamentRuleModel TournamentRules { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MatchModel> Matches { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserModel> Users { get; set; }
    }
}
