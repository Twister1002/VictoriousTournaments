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
    
    public partial class tournamentteambrackets
    {
        public int TournamentTeamID { get; set; }
        public int BracketID { get; set; }
        public int Seed { get; set; }
    
        public virtual brackets brackets { get; set; }
        public virtual tournamentteams tournamentteams { get; set; }
    }
}
