using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class MatchRepository : IMatchRepository
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public MatchRepository()
        {
            context = new VictoriousEntities();
        }

        public DbError AddMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = new MatchModel();
                _match = match;

                _match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                _match.Defender = context.TournamentUserModels.Find(match.DefenderID);

                //context.Challengers.Add(new Challenger() { TournamentUserID = _match.ChallengerID, MatchID = _match.MatchID });
                //context.Defenders.Add(new Defender() { TournamentUserID = _match.DefenderID, MatchID = _match.MatchID });


                //context.Matches.Load();
                //context.Users.Load();
                context.MatchModels.Add(_match);
                context.Entry(_match).CurrentValues.SetValues(match);
                context.SaveChanges();
                //bracket.Matches.Add(_match);
                //context.SaveChanges();
                //context.Tournaments.Include(x => x.Brackets).Load();
                //context.Users.Include(x => x.UserID).Load();
                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.ERROR;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteMatch(int matchId)
        {
            try
            {
                MatchModel _match = context.MatchModels.Find(matchId);
                foreach (var game in _match.Games.ToList())
                {
                    context.GameModels.Remove(game);
                }
                context.MatchModels.Remove(_match);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public IList<MatchModel> GetAllMatchesInBracket(int bracketId)
        {
            List<MatchModel> matches = new List<MatchModel>();

            try
            {
                matches = context.MatchModels.Where(x => x.BracketID == bracketId).ToList();
                foreach (var match in matches)
                {
                    match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                matches.Clear();

            }
            return matches;
        }

        public MatchModel GetMatch(int matchId)
        {
            MatchModel match = new MatchModel();
            try
            {
                match = context.MatchModels.Find(matchId);
                match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                match.Defender = context.TournamentUserModels.Find(match.DefenderID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                match = null;
            }
            return match;
        }

        public DbError UpdateMatch(MatchModel match)
        {
            try
            {
                MatchModel _match = context.MatchModels.Find(match.MatchID);
                context.Entry(_match).CurrentValues.SetValues(match);

                if (_match.ChallengerID != match.ChallengerID || _match.DefenderID != match.DefenderID)
                {
                    _match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    _match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
                if (match.Challenger == null || match.Defender == null)
                {
                    match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
               

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);

                return DbError.FAILED_TO_UPDATE;
            }
            return DbError.SUCCESS;
        }

        public void Save()
        {
            throw new NotImplementedException();
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
        // ~MatchRepository() {
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

       
    }


}
