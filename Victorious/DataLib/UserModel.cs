namespace DataLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

 
    public partial class UserModel : DbModel
    {
        public const int FirstNameLength = 50;
        public const int LastNameLength = 50;
        public const int EmailLength = 50;
        public const int UsernameLength = 50;
        public const int PasswordLength = 50;
        public const int PhoneNumberLength = 15;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserModel()
        {
          
        }
        [Key]
        public int UserID { get; set; }

        [StringLength(FirstNameLength)]
        public string FirstName { get; set; }

        [StringLength(LastNameLength)]
        public string LastName { get; set; }

        [StringLength(EmailLength)]
        public string Email { get; set; }

       
        [StringLength(UsernameLength)]
        public string Username { get; set; }

        [StringLength(PasswordLength)]
        public string Password { get; set; }

        [StringLength(PhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? LastLogin { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Match> Matches { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Match> Matches1 { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Match> Matches2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TournamentModel> Tournaments { get; set; }
    }
}
