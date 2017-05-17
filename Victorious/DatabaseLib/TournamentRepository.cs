using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class TournamentRepository : ITournamentRepository, IDisposable
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public TournamentRepository()
        {
            context = new VictoriousEntities();
        }

        public DbError AddTournament(TournamentModel tournament)
        {
            TournamentModel _tournament = new TournamentModel();
            try
            {
                //if (AddTournamentInviteCode(tournament.InviteCode) == DbError.EXISTS)
                //    return DbError.INVITE_CODE_EXISTS;
                _tournament = tournament;

                _tournament.CreatedOn = DateTime.Now;
                _tournament.LastEditedOn = DateTime.Now;
                context.SaveChanges();
                context.TournamentModels.Add(_tournament);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(int tournamentId)
        {
            throw new NotImplementedException();
        }

        public IList<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                tournaments = context.TournamentModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        public TournamentModel GetTournament(int tournamentId)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.TournamentModels.Find(tournamentId);
                foreach (var bracket in tournament.Brackets)
                {
                    foreach (var match in bracket.Matches)
                    {
                        match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                        match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                    }
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                tournament = null;
            }
            return tournament;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public DbError UpdateTournament(TournamentModel tournament)
        {
            using (var db = new VictoriousEntities())
            {
                try
                {
                    TournamentModel _tournament = db.TournamentModels.Find(tournament.TournamentID);
                    db.Entry(_tournament).CurrentValues.SetValues(tournament);

                    foreach (var bracket in _tournament.Brackets)
                    {
                        foreach (var match in bracket.Matches)
                        {
                            match.Challenger = db.TournamentUserModels.Find(match.ChallengerID);
                            match.Defender = db.TournamentUserModels.Find(match.DefenderID);
                        }
                    }
                   
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    interfaceException = ex;
                    WriteException(ex);
                    return DbError.FAILED_TO_UPDATE;
                }
                return DbError.SUCCESS;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TournamentRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }

        public IList<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            List<TournamentUserModel> list = new List<TournamentUserModel>();
            try
            {
                list = context.TournamentModels.Find(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                list.Clear();
            }
            return list;
        }
    }
}
