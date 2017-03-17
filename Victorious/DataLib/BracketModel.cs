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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BracketModel()
        {
            this.Matches = new Collection<MatchModel>();
            this.UserSeeds = new Collection<UserBracketSeedModel>();
        }

        [Key]
        public int BracketID { get; set; }

        public string BracketTitle { get; set; }

        public int BracketTypeID { get; set; }     

        public ICollection<MatchModel> Matches { get; set; }

        public ICollection<UserBracketSeedModel> UserSeeds { get; set; }

        // Touthat holds this bracket.
        public TournamentModel Tournament { get; set; }

        [ForeignKey("BracketTypeID")]
        public BracketTypeModel BracketType { get; set; }

     
    }
}
