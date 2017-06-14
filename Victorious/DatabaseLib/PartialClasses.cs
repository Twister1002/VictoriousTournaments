using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity.Utilities;
using System.Data.Entity.SqlServer;


namespace DatabaseLib
{
    public partial class VictoriousEntities : DbContext
    {
        //public VictoriousEntities(string server, string targetDb) : 
        //    base(@"metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;provider connection string=';data source="+server+";initial catalog="+targetDb+";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework';")
        //{
        //    SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
        //    sqlBuilder.DataSource = server;
        //    sqlBuilder.InitialCatalog = targetDb;


        //}

        public VictoriousEntities(string name) :
           base(name)
        {

        }

    }


    public partial class AccountModel
    {
        public const int FirstNameLength = 50;
        public const int LastNameLength = 50;
        public const int UsernameLength = 50;
        public const int EmailLength = 100;
        public const int PasswordLength = 50;

        partial void OnInit()
        {
            Tournaments = new Collection<TournamentModel>();
        }

        public ICollection<TournamentModel> Tournaments { get; set; }

        internal string GetFullName()
        {
            return this.FirstName + ' ' + this.LastName;
        }


        //internal string FullName
        //{
        //   get { return this.FirstName + ' ' + this.LastName; }
        //}


    }

    public partial class AccountInviteModel
    {
        partial void OnInit()
        {

        }

        internal string FullName()
        {
            return this.Account.FirstName + ' ' + this.Account.LastName;
        }
    }

    public partial class TournamentInviteModel
    {
        //partial void OnInit()
        //{
        //    //Tournaments = new Collection<TournamentModel>();
        //}

    }

    public partial class TournamentModel
    {
        partial void OnInit()
        {
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

    public partial class BracketModel
    {
        partial void OnInit()
        {
            Matches = new Collection<MatchModel>();
        }
    }

    public partial class MatchModel
    {
        partial void OnInit()
        {
            //Challenger = new TournamentUserModel();
            //Defender = new TournamentUserModel();
        }

        [NotMapped]
        public TournamentUserModel Challenger { get; set; }

        [NotMapped]
        public TournamentUserModel Defender { get; set; }


    }

    public partial class GameModel
    {
        partial void OnInit()
        {

        }
    }


}
