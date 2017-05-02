﻿using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;

namespace WebApplication.Models
{
    public class MatchViewModel : MatchFields
    {
        public MatchModel Model { get; private set; }
        public IMatch Match { get; private set; }

        public MatchViewModel()
        {
            Match = new Match();
            Model = new MatchModel();
        }

        public MatchViewModel(IMatch match)
        {
            Match = match;
            Model = Match.GetModel();
        }

        public MatchViewModel(int matchId)
        {
            Model = db.GetMatch(matchId);
            if (Model != null)
            {
                Match = new Match(Model);
            }
            else
            {
                Match = new Match();
            }
        }

        public bool DeleteGame(int gameId)
        {
            return db.DeleteGame(gameId) == DbError.SUCCESS;
        }

        public bool Update()
        {
            return db.UpdateMatch(Model) == DbError.SUCCESS;
        }

        public void RemoveGames()
        {
            List<GameModel> games = Model.Games.ToList();

            foreach (GameModel game in games)
            {
                DeleteGame(game.GameID);
            }
        }

        public IPlayer Challenger()
        {
            IPlayer player = Match.Players[(int)PlayerSlot.Challenger];
            if (player == null)
            {
                player = new User()
                {
                      Name = "Winner from "+Match.PreviousMatchNumbers[(int)PlayerSlot.Challenger]
                };
            }

            return player;
        }

        public IPlayer Defender()
        {
            IPlayer player = Match.Players[(int)PlayerSlot.Defender];
            if (player == null)
            {
                player = new User()
                {
                    Name = "Winner from " + Match.PreviousMatchNumbers[(int)PlayerSlot.Defender]
                };
            }

            return player;
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