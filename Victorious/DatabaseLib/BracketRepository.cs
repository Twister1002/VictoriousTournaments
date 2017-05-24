using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class BracketRepository : IBracketRepository, IDisposable
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public BracketRepository()
        {
            context = new VictoriousEntities();
        }

        public BracketRepository(VictoriousEntities context)
        {
            this.context = context;
        }

        public DbError AddBracket(BracketModel bracket)
        {
            try
            {
                context.BracketModels.Add(bracket);
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

        public DbError DeleteBracket(int id)
        {
            try
            {
                BracketModel _bracket = context.BracketModels.Find(id);
                // TODO: implement delete for matches
                //foreach (var match in _bracket.Matches.ToList())
                //{
                //    DeleteMatch(match.MatchID);
                //}
                context.BracketModels.Remove(_bracket);
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

        public IList<BracketModel> GetAllBracketsInTournament(int tournamentId)
        {
            List<BracketModel> brackets = new List<BracketModel>();
            try
            {
                brackets = context.BracketModels.Where(x => x.TournamentID == tournamentId).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                brackets.Clear();
            }
            return brackets;
        }

        public BracketModel GetBracket(int id)
        {
            BracketModel bracket = new BracketModel();
            try
            {
                bracket = context.BracketModels.Single(b => b.BracketID == id);
                foreach (var match in bracket.Matches)
                {
                    match.Challenger = context.TournamentUserModels.Find(match.ChallengerID);
                    match.Defender = context.TournamentUserModels.Find(match.DefenderID);
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                bracket = null;
            }
            return bracket;
        }

        public DbError UpdateBracket(BracketModel bracket)
        {
            try
            {
                BracketModel _bracket = context.BracketModels.Find(bracket.BracketID);
                context.Entry(_bracket).CurrentValues.SetValues(bracket);
                foreach (var match in _bracket.Matches)
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
        // ~BracketRepository() {
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
