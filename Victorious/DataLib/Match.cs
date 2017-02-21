namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Match
    {
        public int MatchID { get; set; }

        public int? ChallengerID { get; set; }

        public int? DefenderID { get; set; }

        public int? TournamentID { get; set; }

        public int? WinnerID { get; set; }

        public int? ChallengerScore { get; set; }

        public int? DefenderScore { get; set; }

        public int RoundNumber { get; set; }

        public bool IsBye { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public TimeSpan? MatchDuration { get; set; }

        [ForeignKey("ChallengerID")]
        public User Challenger { get; set; }

        [ForeignKey("DefenderID")]
        public User Defender { get; set; }

        [ForeignKey("TournamentID")]
        public Tournament Tournament { get; set; }

        [ForeignKey("WinnerID")]
        public User Winner { get; set; }
    }
}
