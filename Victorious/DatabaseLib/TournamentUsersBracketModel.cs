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
    
    public partial class TournamentUsersBracketModel
    {
        public int TournamentUserID { get; set; }
        public int BracketID { get; set; }
        public Nullable<int> Seed { get; set; }
    	
    	partial void OnInit();
    
        public virtual BracketModel Bracket { get; set; }
        public virtual TournamentUserModel TournamentUser { get; set; }
    }
}