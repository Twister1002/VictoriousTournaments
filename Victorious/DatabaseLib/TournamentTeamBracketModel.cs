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
    
    public partial class TournamentTeamBracketModel
    {
        public int TournamentTeamID { get; set; }
        public int BracketID { get; set; }
        public int Seed { get; set; }
    	
    	partial void OnInit();
    
        public virtual BracketModel Bracket { get; set; }
        public virtual TournamentTeamModel TournamentTeam { get; set; }
    }
}
