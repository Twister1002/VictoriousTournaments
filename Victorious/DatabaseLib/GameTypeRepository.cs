using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class GameTypeRepository : IGameTypeRepository
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public GameTypeRepository()
        {
            context = new VictoriousEntities();
        }

        public DbError AddGameType(GameTypeModel gameType)
        {
            try
            {
                context.GameTypeModels.Add(gameType);
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

        public DbError DeleteGameType(int gameTypeId)
        {
            GameTypeModel _gameType = context.GameTypeModels.Find(gameTypeId);
            try
            {
                context.GameTypeModels.Remove(_gameType);
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

        public IList<GameTypeModel> GetAllGameTypes()
        {
            List<GameTypeModel> gameTypes = new List<GameTypeModel>();
            try
            {
                gameTypes = context.GameTypeModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                gameTypes.Clear();
            }
            return gameTypes;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public DbError UpdateGameType(GameTypeModel gameType)
        {
            try
            {
                GameTypeModel _gameType = context.GameTypeModels.Find(gameType.GameTypeID);
                context.Entry(_gameType).CurrentValues.SetValues(gameType);
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
        // ~GameTypeRepository() {
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
