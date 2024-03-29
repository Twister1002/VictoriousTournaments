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
            UsersInTournament = new Collection<UserInTournamentModel>();
            //TournamentRules = new TournamentRuleModel();

        }
        [Key]
        public int TournamentID { get; set; }

        public int? TournamentRulesID { get; set; }

        public int? GameTypeID { get; set; }

        [Required]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedByID { get; set; }

        public int WinnerID { get; set; }

        public DateTime LastEditedOn { get; set; }

        public int LastEditedByID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [ForeignKey("GameTypeID")]
        public GameTypeModel GameType { get; set; }

        [ForeignKey("TournamentRulesID")]
        public TournamentRuleModel TournamentRules { get; set; }

        public ICollection<TeamModel> Teams { get; set; }

        [Obsolete("Use UsersInTournament Collection")]
        public ICollection<UserModel> Users { get; set; }

        public ICollection<BracketModel> Brackets { get; set; }

        public ICollection<UserInTournamentModel> UsersInTournament { get; set; }

    }
}
