//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game
    {
        public int GameID { get; set; }
        public int ChallengerID { get; set; }
        public int DefenderID { get; set; }
        public int WinnerID { get; set; }
        public Nullable<int> MatchID { get; set; }
        public int GameNumber { get; set; }
        public int ChallengerScore { get; set; }
        public int DefenderScore { get; set; }
    }
}
