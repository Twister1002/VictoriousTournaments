using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Administrator
{
    public class GameTypeViewModel : GameTypeFields
    {
        private List<GameTypeModel> GameTypes;
        public GameTypeModel GameType { get; private set; }

        public GameTypeViewModel()
        {
            Reload();
            GameType = new GameTypeModel();
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
            DbError result = db.AddGameType(GameType);

            Reload();
            return result == DbError.SUCCESS;
        }

        public bool Delete(int gameTypeId)
        {
            DbError result = db.DeleteGameType(gameTypeId);

            Reload();
            return result == DbError.SUCCESS;
        }

        public List<GameTypeModel> Select()
        {
            return GameTypes;
        }
        
        public bool Select(int gameTypeId)
        {
            GameType = GameTypes.First(x => x.GameTypeID == gameTypeId);

            return true;
        }

        public bool Select(String title)
        {
            GameType = GameTypes.First(x => x.Title == title);

            return true;
        }

        private void Reload()
        {
            GameTypes = db.GetAllGameTypes();
        }
    }
}