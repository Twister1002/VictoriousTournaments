using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IMatchRepository 
    {
        IList<MatchModel> GetAllMatchesInBracket(int bracketId);
        DbError AddMatch(MatchModel match);
        MatchModel GetMatch(int matchId);
        DbError UpdateMatch(MatchModel match);
        DbError DeleteMatch(int matchId);
        void Save();

    }
}
