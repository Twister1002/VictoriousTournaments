using Tournament.Structure;
using DatabaseLib;
using System;

namespace WebApplication.Models
{
    public class GameViewModel : GameFields
    {
        public GameModel Model { get; private set; }
        public IGame Game { get; private set; }

        public GameViewModel()
        {
            Game = new Game();
            Model = Game.GetModel();
        }

        public GameViewModel(GameModel model)
        {
            Model = model;
            Game = new Game(model);
        }

        public GameViewModel(IGame game)
        {
            Game = game;
            Model = Game.GetModel();
        }

        protected override void Init()
        {
        }

        public bool Create()
        {
            return false;
        }

        public bool Delete()
        {
            return db.DeleteGame(Model.GameID) == DbError.SUCCESS;
        }

        public bool Update()
        {
            return false;
        }
    }
}