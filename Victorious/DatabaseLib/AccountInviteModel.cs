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
    
    public partial class AccountInviteModel
    {
        public int AccountInviteID { get; set; }
        public string AccountInviteCode { get; set; }
        public int SentByID { get; set; }
        public string SentToEmail { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateExpires { get; set; }
        public bool IsExpired { get; set; }
    
        public virtual AccountModel Account { get; set; }
    }
}
