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
    
    public partial class TournamentUserSeed
    {
        public int TournamentUserID { get; set; }
        public int BracketID { get; set; }
        public int Seed { get; set; }
    
        public virtual Bracket Bracket { get; set; }
        public virtual TournamentUser TournamentUser { get; set; }
    }
}
