using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

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
            Model = db.GetMatchById(matchId);
            if (Model != null)
            {
                Match = new Match(Model);
            }
            else
            {
                Match = new Match();
            }
        }

        public void RemoveGame(int gameNum)
        {
            DbError result = db.DeleteGame(Model, Match.Games[gameNum].GetModel());
            //if (result == DbError.SUCCESS)
            //{
            //    Match.RemoveGameNumber(gameNum);
            //}
        }

        public void RemoveGames()
        {
            List<GameModel> games = Model.Games.ToList();

            foreach (GameModel game in games)
            {
                DbError result = db.DeleteGame(Model, game);
                //if (result == DbError.SUCCESS)
                //{
                //    //Match.RemoveGameNumber(game.GameNumber);
                //}
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