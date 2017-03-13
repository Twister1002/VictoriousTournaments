namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class TournamentRuleModel : DbModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TournamentRuleModel()
        {

        }

        [Key]
        public int TournamentRulesID { get; set; }

        public int? TournamentID { get; set; }

        public int? NumberOfRounds { get; set; }

        public bool? HasEntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal? EntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizePurse { get; set; }

        //public int? NumberOfPlayers { get; set; }

        public bool? IsPublic { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public DateTime? TournamentStartDate { get; set; }

        public DateTime? TournamentEndDate { get; set; }

        //[ForeignKey("TournamentID")]
        //public virtual TournamentModel Tournament { get; set; }

        //public ICollection<Bracket> Brackets { get; set; }

    }
}
