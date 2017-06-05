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
    
    public partial class BracketModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BracketModel()
        {
            this.TournamentUsersBrackets = new HashSet<TournamentUsersBracketModel>();
            this.Matches = new HashSet<MatchModel>();
    		OnInit();
        }
    
        public int BracketID { get; set; }
        public int BracketTypeID { get; set; }
        public bool Finalized { get; set; }
        public int NumberOfGroups { get; set; }
        public Nullable<int> TournamentID { get; set; }
        public int MaxRounds { get; set; }
    	
    	partial void OnInit();
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TournamentUsersBracketModel> TournamentUsersBrackets { get; set; }
        public virtual BracketTypeModel BracketType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MatchModel> Matches { get; set; }
        public virtual TournamentModel Tournament { get; set; }
    }
}
