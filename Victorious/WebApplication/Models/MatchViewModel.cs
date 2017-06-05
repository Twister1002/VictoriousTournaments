﻿using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;
using System;

namespace WebApplication.Models
{
    public class MatchViewModel : MatchFields
    {
        public MatchModel Model { get; private set; }
        public IMatch Match { get; private set; }

        public IPlayer Challenger { get; private set;}
        public IPlayer Defender { get; private set; }

        public MatchViewModel()
        {
            Match = new Match();
            Model = new MatchModel();
        }

        public MatchViewModel(IMatch match)
        {
            Match = match;
            Model = Match.GetModel();
            LoadPlayers();
        }

        public MatchViewModel(MatchModel match)
        {
            Model = match;
            //LoadPlayerObjects();
            Match = new Match(match);
            LoadPlayers();
        }

        public MatchViewModel(int matchId)
        {
            Model = tournamentService.GetMatch(matchId);
            if (Model != null)
            {
                Match = new Match(Model);
            }
            else
            {
                Match = new Match();
            }
            LoadPlayers();
        }

        // Acquires data from the database if the objects are null
        private void LoadPlayerObjects()
        {
            if (Model.Challenger == null)
            {
                Model.Challenger = tournamentService.GetTournamentUser(Model.ChallengerID);
            }
            if (Model.Defender == null)
            {
                Model.Defender = tournamentService.GetTournamentUser(Model.DefenderID);
            }

            LoadPlayers();
        }

        protected override void Init()
        {
            
        }
        
        private void LoadPlayers()
        {
            Challenger = Match.Players[(int)PlayerSlot.Challenger];
            if (Challenger == null)
            {
                String Name = Match.PreviousMatchNumbers[(int)PlayerSlot.Challenger] == -1 ? "" : "Match " + Match.PreviousMatchNumbers[(int)PlayerSlot.Challenger];
                Challenger = new User()
                {
                    Name = Name
                };
            }

            Defender = Match.Players[(int)PlayerSlot.Defender];
            if (Defender == null)
            {
                String Name = Match.PreviousMatchNumbers[(int)PlayerSlot.Defender] == -1 ? "" : "Match " + Match.PreviousMatchNumbers[(int)PlayerSlot.Defender];
                Defender = new User()
                {
                    Name = Name
                };
            }
        }

        public bool Update()
        {
            tournamentService.UpdateMatch(Model);

            foreach (GameModel game in Model.Games)
            {
                if (game.GameID == -1)
                {
                    tournamentService.AddGame(game);
                }
                else
                {
                    tournamentService.UpdateGame(game);
                }
            }
            if (Save())
            {
                Match = new Match(Model);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int DefenderScore()
        {
            return Match.Score[(int)PlayerSlot.Defender];
        }

        public int ChallengerScore()
        {
            return Match.Score[(int)PlayerSlot.Challenger];
        }
    }
}