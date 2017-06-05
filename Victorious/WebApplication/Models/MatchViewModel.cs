using System.Collections.Generic;
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

            Init();
        }

        public MatchViewModel(MatchModel match)
        {
            Model = match;
            LoadPlayerObjects();
            Match = new Match(match);
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

        /// <summary>
        /// This will reload the model from one of two sources
        /// </summary>
        /// <param name="fromDatabase">true: Reloads from database; false: Reloads from IMatch</param>
        public void ReloadModel(MatchModel model)
        {
            Model = model;
            //Match = new Match(model);
        }

        // Acquires data from the database if the objects are null
        private void LoadPlayerObjects()
        {
            if (Model.Challenger == null)
            {
                Model.Challenger = db.GetTournamentUser(Model.ChallengerID);
            }
            if (Model.Defender == null)
            {
                Model.Defender = db.GetTournamentUser(Model.DefenderID);
            }
        }

        protected override void Init()
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
            DbError matchUpdate = db.UpdateMatch(Model);
            DbError gameUpdate = DbError.NONE;
            foreach (GameModel game in Model.Games)
            {
                if (game.GameID == -1)
                {
                    gameUpdate = db.AddGame(game);
                }
                else
                {
                    gameUpdate = db.UpdateGame(game);
                }
            }

            bool matchResult = matchUpdate == DbError.SUCCESS;
            bool gameResult = gameUpdate == DbError.SUCCESS || gameUpdate == DbError.NONE;
            Match = new Match(Model);

            return matchResult && gameResult;
        }

        public bool DeleteGame(int gameId)
        {
            bool gameResult = db.DeleteGame(gameId) == DbError.SUCCESS;
            bool matchUpdate = Update();

            return matchUpdate && gameResult;
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