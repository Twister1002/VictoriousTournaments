using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class GameViewModel
    {
        private GameModel Model;
        public IGame Game { get; private set; }

        public GameViewModel()
        {

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


    }
}