using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public struct MatchGame
    {
        public int ChallengerScore;
        public int DefenderScore;
        public bool isProcessed;
    }

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
    }
}