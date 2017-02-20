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

        public TournamentModel(String guid) : this(guid, "Unnammed Tournament")
        { 
            
        }

        public TournamentModel(String guid, String name)
        {
            this.guid = guid;
            this.name = name;
        }

        public TournamentModel getTournament(String guid)
        {
            return this;
        }
    }
}