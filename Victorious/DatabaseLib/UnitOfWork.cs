using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class UnitOfWork : IDisposable
    {
        private VictoriousEntities context = new VictoriousEntities();
        private ITournamentRepository tournamentRepository;
        private IAccountRepository accountRepository;
        private IBracketRepository bracketRepository;
        private IBracketTypeRepository bracketTypeRepository;
        private IGameRepository gameRepository;
        private IGameTypeRepository gameTypeRepository;
        private IMatchRepository matchRepository;
        private IPlatformRepository platformRepository;

        public ITournamentRepository TournamentRepository
        {
            get
            {
                if (this.tournamentRepository == null)
                {
                    this.tournamentRepository = new TournamentRepository(context);
                }
                return tournamentRepository;
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
        // ~UnitOfWork() {
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
    }
}
