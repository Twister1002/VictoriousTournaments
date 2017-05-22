using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    interface IGameRepository : IDisposable
    {
        DbError AddGame(GameModel game);
        GameModel GetGame(int gameId);
        IList<GameModel> GetAllGamesInMatch(int matchId);
        DbError UpdateGame(GameModel game);
        DbError DeleteGame(int id);
        void Save();

    }
}
