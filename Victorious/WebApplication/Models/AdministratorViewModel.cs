using System;
using System.Collections.Generic;
using DatabaseLib;

namespace WebApplication.Models
{
    public class AdministratorViewModel : AdministratorFields
    {
        public List<GameTypeModel> Games { get; private set; }

        public AdministratorViewModel()
        {
            //Games = db.GetAllGameTypes();
        }

        public bool CreateGame(GameTypeModel game)
        {
            return false;
        }

        public bool UpdateGame(GameTypeModel game)
        {
            return false;
        }

        public bool DeleteGame(GameTypeModel game)
        {
            return false;
        }
    }
}