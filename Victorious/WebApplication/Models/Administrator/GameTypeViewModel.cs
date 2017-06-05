using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Administrator
{
    public class GameTypeViewModel : GameTypeFields
    {
        public List<GameTypeModel> GameTypes { get; private set; }
        public GameTypeModel GameType { get; private set; }

        public GameTypeViewModel()
        {
            GameType = new GameTypeModel();
        }

        protected override void Init()
        {
            Select();
        }

        public void ApplyFields()
        {
            GameType.Title = Title;
        }

        public void SetFields()
        {
            Title = GameType.Title;
        }

        public bool Update()
        {
            return false;
        }

        public bool Create()
        {
            ApplyFields();
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