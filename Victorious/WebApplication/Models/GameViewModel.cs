using Tournament.Structure;
using DatabaseLib;
using System;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class GameViewModel : GameFields, IViewModel
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

        public void Init() { 
        }

        public bool Create()
        {
            return false;
        }

        public bool Delete()
        {
            tournamentService.DeleteGame(Model.GameID);
            return Save();
        }

        public bool Update()
        {
            return false;
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void SetFields()
        {
            throw new NotImplementedException();
        }
    }
}