using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public partial class AccountModel
    {
        public const int FirstNameLength = 50;
        public const int LastNameLength = 50;
        public const int UsernameLength = 50;
        public const int EmailLength = 100;
        public const int PasswordLength = 50;

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

            Brackets = new Collection<BracketModel>();
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
            Challenger = new TournamentUserModel();
            Defender = new TournamentUserModel();
        }

        public virtual TournamentUserModel Challenger { get; set; }

        public virtual TournamentUserModel Defender { get; set; }

      
    }

    public partial class GameModel
    {
        partial void OnInit()
        {

        }
    }


}
