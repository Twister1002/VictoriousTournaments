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
    
    public partial class tournamentinvites
    {
        public int TournamentInviteID { get; set; }
        public string TournamentInviteCode { get; set; }
        public int TournamentID { get; set; }
        public Nullable<System.DateTime> DateExpires { get; set; }
        public bool IsExpired { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int NumberOfUses { get; set; }
    
        public virtual tournaments tournaments { get; set; }
    }
}
