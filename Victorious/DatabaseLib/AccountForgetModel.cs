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
    
    public partial class AccountForgetModel
    {
        public int AccountForgetID { get; set; }
        public int AccountID { get; set; }
        public string Token { get; set; }
        public System.DateTime DateIssued { get; set; }
        public Nullable<System.DateTime> DateUsed { get; set; }
    	
    	partial void OnInit();
    
        public virtual AccountModel Account { get; set; }
    }
}