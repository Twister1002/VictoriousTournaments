using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
}
