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
    
    public partial class Bracket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bracket()
        {
            this.Matches = new HashSet<Match>();
            this.TournamentUsersBrackets = new HashSet<TournamentUsersBracket>();
        }
    
        public int BracketID { get; set; }
        public string BracketTitle { get; set; }
        public int BracketTypeID { get; set; }
        public bool Finalized { get; set; }
        public int NumberOfGroups { get; set; }
        public Nullable<int> TournamentID { get; set; }
    
        public virtual Tournament Tournament { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Match> Matches { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TournamentUsersBracket> TournamentUsersBrackets { get; set; }
    }
}
