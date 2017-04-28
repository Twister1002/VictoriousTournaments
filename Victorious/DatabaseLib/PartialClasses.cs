using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public partial class AccountModel
    {
        public AccountModel()
        {
            Tournaments = new Collection<TournamentModel>();
        }

        public ICollection<TournamentModel> Tournaments { get; set; }

    }

    public partial class TournamentModel
    {
        partial void OnInit()
        {

            IsPublic = true;
         
            WinnerID = -1;
            CreatedOn = DateTime.Now;
            LastEditedOn = DateTime.Now;
        }

    }

    public partial class TournamentUserModel
    {
        partial void OnInit()
        {

        }
    }
}
