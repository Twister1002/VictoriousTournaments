namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.SqlClient;
    using System.Data.Entity.Spatial;

    public partial class MatchModel
    {
        public MatchModel()
        {
            MatchNumber = -1;
        }

        [Key]
        public int MatchID { get; set; }

        public int? ChallengerID { get; set; }

        public int? DefenderID { get; set; }

        //public int? TournamentID { get; set; }

        public int? WinnerID { get; set; }

        public int? ChallengerScore { get; set; }

        public int? DefenderScore { get; set; }

        public int? RoundIndex { get; set; }
             
        public int MatchNumber { get; set; }

        public bool? IsBye { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public TimeSpan? MatchDuration { get; set; }

        public int? WinsNeeded { get; set; }

        public int? MatchIndex { get; set; }

        public int? NextMatchNumber { get; set; }

        public int? PrevMatchIndex { get; set; }

        public int? NextLoserMatchNumber { get; set; }

        public int? PrevDefenderMatchNumber { get; set; }

        public int? PrevChallengerMatchNumber { get; set; }

        [ForeignKey("ChallengerID")]
        public virtual UserModel Challenger { get; set; }

        [ForeignKey("DefenderID")]
        public virtual UserModel Defender { get; set; }

        //[ForeignKey("TournamentID")]
        //public TournamentModel Tournament { get; set; }

        [ForeignKey("WinnerID")]
        public virtual UserModel Winner { get; set; }
    }
}
