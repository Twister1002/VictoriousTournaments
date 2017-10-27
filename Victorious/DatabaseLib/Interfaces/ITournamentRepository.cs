using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface ITournamentRepository
    {
        IList<TournamentModel> GetAllTournaments();
        TournamentModel GetTournament(int tournamentId);
        DbError AddTournament(TournamentModel tournament);
        DbError UpdateTournament(TournamentModel tournament);
        DbError DeleteTournament(int tournamentId);
        IList<TournamentUserModel> GetAllUsersInTournament(int tournamentId);
        List<KeyValuePair<GameTypeModel, int>> GetAllTournamentsByGame();
        void Save();
    }
}
