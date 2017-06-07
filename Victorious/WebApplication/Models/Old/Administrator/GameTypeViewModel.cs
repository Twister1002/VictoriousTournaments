using DatabaseLib;
using System.Collections.Generic;
using WebApplication.Interfaces;
using System;

namespace WebApplication.Models.Administrator.Old
{
    public class GameTypeViewModel : ViewModel
    {
        public List<GameTypeModel> GameTypes { get; private set; }
        public GameTypeModel GameType { get; private set; }

        public GameTypeViewModel(IUnitOfWork work) : base (work)
        {
            GameType = new GameTypeModel();
            Init();
        }

        public void Init()
        {
            Select();
        }

        public void ApplyChanges()
        {
            GameType.Title = Title;
        }

        public void SetFields()
        {
            Title = GameType.Title;
        }

        public bool Update(int id)
        {
            return false;
        }

        public bool Create()
        {
            ApplyChanges();
            typeService.AddGameType(GameType);

            if (Save())
            {
                Select();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(int gameTypeId)
        {
            typeService.DeleteGameType(gameTypeId);

            if (Save())
            {
                Select();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Select()
        {
            GameTypes = typeService.GetAllGameTypes();
        }
    }
}