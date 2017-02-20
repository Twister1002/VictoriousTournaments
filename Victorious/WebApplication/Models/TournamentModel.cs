using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class TournamentModel
    {
        public String guid { get; private set; }
        public String name { get; private set; }
        public String org { get; private set; }

        public int rounds = 2;

        public TournamentModel(String guid) : this(guid, null)
        { 
            
        }

        public TournamentModel(String guid, String org) : this(guid, org, "Unnamed Torunament")
        {
            
        }

        public TournamentModel(String guid, String org, String name)
        {
            this.guid = guid;
            this.name = name;
            this.org = org;
        }

        public TournamentModel getTournament(String guid)
        {
            return this;
        }
    }
}