using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class GameRepository : IGameRepository, IDisposable
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public GameRepository()
        {
            context = new VictoriousEntities();
        }

        public GameRepository(VictoriousEntities context)
        {
            this.context = context;
        }

        public DbError AddGame(GameModel game)
        {
            try
            {
                context.GameModels.Add(game);
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

        public DbError DeleteGame(int id)
        {
            try
            {
                GameModel _game = context.GameModels.Find(id);
                context.GameModels.Remove(_game);
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

        public IList<GameModel> GetAllGamesInMatch(int matchId)
        {
            List<GameModel> games = new List<GameModel>();
            try
            {
                games = context.GameModels.Where(x => x.MatchID == matchId).ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                games.Clear();
            }
            return games;
        }

        public GameModel GetGame(int gameId)
        {
            GameModel game = new GameModel();
            try
            {
                game = context.GameModels.Find(gameId);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                game = null;
            }
            return game;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public DbError UpdateGame(GameModel game)
        {
            try
            {
                GameModel _game = context.GameModels.Find(game.GameID);
                context.Entry(_game).CurrentValues.SetValues(game);
                if (_game.ChallengerID != game.ChallengerID || _game.DefenderID != game.DefenderID)
                {
                    _game.ChallengerID = game.ChallengerID;
                    _game.DefenderID = game.DefenderID;
                }
                if (_game.ChallengerID != _game.Match.ChallengerID || _game.DefenderID != _game.Match.DefenderID)
                {
                    _game.ChallengerID = game.Match.ChallengerID;
                    _game.DefenderID = game.Match.DefenderID;
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
        // ~GameRepository() {
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
