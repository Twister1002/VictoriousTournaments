using System;
using System.Collections.Generic;
using DatabaseLib;

namespace WebApplication.Models
{
    public class AdministratorViewModel : AdministratorFields
    {
        public List<GameType> Games { get; private set; }

        public AdministratorViewModel()
        {
            Games = db.GetAllGameTypes();
        }

        public bool CreateGame(GameType game)
        {
            return false;
        }

        public bool UpdateGame(GameType game)
        {
            return false;
        }

        public bool DeleteGame(GameType game)
        {
            return false;
        }
    }
}