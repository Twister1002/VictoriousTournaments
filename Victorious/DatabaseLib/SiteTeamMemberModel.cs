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
    
    public partial class SiteTeamMemberModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SiteTeamMemberModel()
        {
            this.TournamentTeamMembers = new HashSet<TournamentTeamMemberModel>();
    		OnInit();
        }
    
        public int SiteTeamMemberID { get; private set; }
        public int AccountID { get; set; }
        public int SiteTeamID { get; set; }
        public DatabaseLib.Permission Role { get; set; }
    	
    	partial void OnInit();
    
        public virtual AccountModel Account { get; set; }
        public virtual SiteTeamModel SiteTeam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TournamentTeamMemberModel> TournamentTeamMembers { get; set; }
    }
}