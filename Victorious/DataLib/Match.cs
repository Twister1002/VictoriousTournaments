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

        public int TournamentID { get; set; }

        public int? ChallengerID { get; set; }

        public int? DefenderID { get; set; }

        public int? WinnerID { get; set; }

        public int? ChallengerScore { get; set; }

        public int? DefenderScore { get; set; }

        public int RoundNumber { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public TimeSpan? MatchDuration { get; set; }

        public bool? IsBye { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual Tournament Tournament { get; set; }

        public virtual User User2 { get; set; }
    }
}
