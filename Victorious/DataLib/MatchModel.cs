namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MatchModel
    {
        [Key]
        public int MatchID { get; set; }

        public int? ChallengerID { get; set; }

        public int? DefenderID { get; set; }

        //public int? TournamentID { get; set; }

        public int? WinnerID { get; set; }

        public int? ChallengerScore { get; set; }

        public int? DefenderScore { get; set; }

        public int? RoundNumber { get; set; }

        public int? MatchNumber { get; set; }

        public bool? IsBye { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public TimeSpan? MatchDuration { get; set; }

        public int? WinsNeeded { get; set; }

        public int? MatchIndex { get; set; }

        public int? NextMatchIndex { get; set; }

        public int? PrevMatchIndex { get; set; }

        public int? NextLoserMatchIndex { get; set; }

        public int? PrevDefenderMatchIndex { get; set; }

        public int? PrevChallengerMatchIndex { get; set; }

        [ForeignKey("ChallengerID")]
        public UserModel Challenger { get; set; }

        [ForeignKey("DefenderID")]
        public UserModel Defender { get; set; }

        //[ForeignKey("TournamentID")]
        //public TournamentModel Tournament { get; set; }

        [ForeignKey("WinnerID")]
        public UserModel Winner { get; set; }
    }
}
