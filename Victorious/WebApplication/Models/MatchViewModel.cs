using System.Collections.Generic;
using System.Linq;
using Tournament.Structure;
using DatabaseLib;

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

            Init();
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

            Init();
        }

        private void Init()
        {
            Challenger = Match.Players[(int)PlayerSlot.Challenger];
            if (Challenger == null)
            {
                Challenger = new User()
                {
                    Name = "Winner from " + Match.PreviousMatchNumbers[(int)PlayerSlot.Challenger]
                };
            }

            Defender = Match.Players[(int)PlayerSlot.Defender];
            if (Defender == null)
            {
                Defender = new User()
                {
                    Name = "Winner from " + Match.PreviousMatchNumbers[(int)PlayerSlot.Defender]
                };
            }
        }
        
        public bool Update()
        {
            return db.UpdateMatch(Model) == DbError.SUCCESS;
        }

        public bool DeleteGame(int gameId)
        {
            return db.DeleteGame(gameId) == DbError.SUCCESS;
        }

        public bool CreateGame(GameModel game)
        {
            return db.AddGame(game) == DbError.SUCCESS;
        }

        public void RemoveGames()
        {
            List<GameModel> games = Model.Games.ToList();

            foreach (GameModel game in games)
            {
                DeleteGame(game.GameID);
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