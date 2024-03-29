namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.SqlClient;
    using System.Data.Entity.Spatial;
    using System.Collections.ObjectModel;
    public partial class MatchModel
    {
        public MatchModel()
        {
            MatchNumber = -1;
            IsBye = false;
            MatchIndex = 0;
            MaxGames = 0;
            PrevMatchIndex = 0;
            Games = new Collection<GameModel>();
            //next
        }

        [Key]
        public int MatchID { get; set; }

        public int ChallengerID { get; set; }
                  
        public int DefenderID { get; set; }

        public int? WinnerID { get; set; }

        public int? ChallengerScore { get; set; }

        public int? DefenderScore { get; set; }

        public int? RoundIndex { get; set; }

        public int MatchNumber { get; set; }

        public bool? IsBye { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public TimeSpan? MatchDuration { get; set; }

        public int? MaxGames { get; set; }

        public int? MatchIndex { get; set; }

        public int? NextMatchNumber { get; set; }

        public int? PrevMatchIndex { get; set; }

        public int? NextLoserMatchNumber { get; set; }

        public int? PrevDefenderMatchNumber { get; set; }

        public int? PrevChallengerMatchNumber { get; set; }

        [InverseProperty("ChallengerMatches")]
        public virtual UserModel Challenger { get; set; }

        [InverseProperty("DefenderMatches")]
        public virtual UserModel Defender { get; set; }

        public ICollection<GameModel> Games { get; set; }

    }
}
