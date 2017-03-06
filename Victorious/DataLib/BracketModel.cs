namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class BracketModel : DbModel
    {
        public const int BracketTypeLength = 50;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BracketModel()
        {
            this.Matches = new Collection<MatchModel>();
            this.UserSeeds = new Collection<UserBracketSeedModel>();
        }

        [Key]
        public int BracketID { get; set; }

        public string BracketTitle { get; set; }

        [StringLength(BracketTypeLength)]
        public string BracketType { get; set; }

        public virtual ICollection<MatchModel> Matches { get; set; }

        public virtual ICollection<UserBracketSeedModel> UserSeeds { get; set; }

        // Tournament that holds this bracket.
        public virtual TournamentModel Tournament { get; set; }

    }
}
