﻿using System;
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

        }

        public TournamentUserModel Challenger { get; set; }

        public TournamentUserModel Defender { get; set; }
    }

    public partial class GameModel
    {
        partial void OnInit()
        {

        }
    }


}
