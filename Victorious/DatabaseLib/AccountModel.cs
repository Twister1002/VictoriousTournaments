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
    
    public partial class AccountModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccountModel()
        {
            this.AccountInvites = new HashSet<AccountInviteModel>();
            this.SiteTeamMembers = new HashSet<SiteTeamMemberModel>();
            this.AccountForgets = new HashSet<AccountForgetModel>();
    		OnInit();
        }
    
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime LastLogin { get; set; }
        public int PermissionLevel { get; set; }
        public string InviteCode { get; set; }
        public Nullable<int> InvitedByID { get; set; }
        public string Salt { get; set; }
        public Nullable<bool> ReceiveTournamentUpdates { get; set; }
    	
    	partial void OnInit();
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountInviteModel> AccountInvites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteTeamMemberModel> SiteTeamMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountForgetModel> AccountForgets { get; set; }
    }
}
