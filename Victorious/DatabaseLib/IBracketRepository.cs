using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IBracketRepository : IDisposable
    {
        DbError AddBracket(BracketModel bracket);

        BracketModel GetBracket(int id);

        IList<BracketModel> GetAllBracketsInTournament(int tournamentId);

        DbError UpdateBracket(BracketModel bracket);

         DbError DeleteBracket(int id);


    }
}
