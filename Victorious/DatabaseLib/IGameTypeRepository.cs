using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    interface IGameTypeRepository : IDisposable
    {
        DbError AddGameType(GameTypeModel gameType);
        DbError UpdateGameType(GameTypeModel gameType);
        IList<GameTypeModel> GetAllGameTypes();
        DbError DeleteGameType(int gameTypeId);
        void Save();



    }
}
