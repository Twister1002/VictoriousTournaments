namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public enum Platform
    {
        PC, Xbox, Xbox_360, Xbox_One, Nintendo_64, GameCube, Wii, Wii_U, Switch, PS1, PS2, PS3, PS4
    }

    public partial class TournamentRuleModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TournamentRuleModel()
        {
            HasEntryFee = false;
            IsPublic = true;
        }

        [Key]
        public int TournamentRulesID { get; set; }

        public int TournamentID { get; set; }

        public int NumberOfRounds { get; set; }

        public bool HasEntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal? EntryFee { get; set; }

        [Column(TypeName = "money")]
        public decimal PrizePurse { get; set; }

        //public int? NumberOfPlayers { get; set; }

        public bool IsPublic { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public DateTime? TournamentStartDate { get; set; }

        public DateTime? TournamentEndDate { get; set; }

        public DateTime? CheckInBegins { get; set; }

        public DateTime? CheckInEnds { get; set; }

        public Platform Platform { get; set; }


        //[ForeignKey("TournamentID")]
        //public virtual TournamentModel Tournament { get; set; }

    }
}
