namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TournamentRule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TournamentRule()
        {

        }

        [Key]
        public int TournamentRulesID { get; set; }

        public int TournamentID { get; set; }

        public int BracketID { get; set; }

        public int NumberOfRounds { get; set; }

        public bool HasEntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal? EntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizePurse { get; set; }

        public int? NumberOfPlayers { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CutoffDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual Bracket Bracket { get; set; }

        public virtual Tournament Tournament { get; set; }
    }
}
