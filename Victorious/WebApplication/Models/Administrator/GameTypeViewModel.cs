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
            DbError result = DbError.NONE;

            if (!String.IsNullOrEmpty(GameType.Title))
            {
                result = db.AddGameType(GameType);
            }

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

        private void Reload()
        {
            GameTypes = db.GetAllGameTypes();
        }
    }
}