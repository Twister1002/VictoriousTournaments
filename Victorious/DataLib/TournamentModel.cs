namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class TournamentModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TournamentModel()
        {
            Users = new Collection<UserModel>();
            Brackets = new Collection<BracketModel>();
            
        }
        [Key]
        public int TournamentID { get; set; }

        public int TournamentRulesID { get; set; }

        [Required]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedByID { get; set; }

        public int? WinnerID { get; set; }

        public DateTime? LastEditedOn { get; set; }

        public int? LastEditedByID { get; set; }

        [ForeignKey("TournamentRulesID")]
        public virtual TournamentRuleModel TournamentRules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        //public virtual ICollection<MatchModel> Matches { get; set; }

        public virtual ICollection<UserModel> Users { get; set; }

        public virtual ICollection<BracketModel> Brackets { get; set; }

        //public virtual ICollection<UserBracketSeedModel> UserBracketSeeds { get; set; }

        //private ICollection<MatchModel> _Matches;

        //public virtual ICollection<MatchModel> Matches
        //{
        //    get { return _Matches ?? (_Matches = new Collection<MatchModel>()); }
        //    set { _Matches = value; }
        //}


        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //private ICollection<UserModel> _Users;

        //public virtual ICollection<UserModel> Users
        //{
        //    get { return _Users ?? (_Users = new Collection<UserModel>()); }
        //    set { _Users = value; }
        //}
    }
}
