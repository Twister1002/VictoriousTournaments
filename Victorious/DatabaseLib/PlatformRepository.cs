using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class PlatformRepository : IPlatformRepository
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public PlatformRepository()
        {
            context = new VictoriousEntities();
        }

        public DbError AddPlatform(PlatformModel platform)
        {
            try
            {
                context.PlatformModels.Add(platform);
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

        public DbError DeletePlatform(int platformId)
        {
            try
            {
                PlatformModel platform = context.PlatformModels.Find(platformId);
                context.PlatformModels.Remove(platform);
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

        public IList<PlatformModel> GetAllPlatforms()
        {
            List<PlatformModel> platforms = new List<PlatformModel>();
            try
            {
                platforms = context.PlatformModels.ToList();
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                platforms.Clear();
            }
            return platforms;
        }

        public PlatformModel GetPlatform(int platformID)
        {
            PlatformModel platform = new PlatformModel();
            try
            {
                platform = context.PlatformModels.Find(platformID);
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                platform = null;
            }
            return platform;
        }

        public DbError UpdatePlatform(PlatformModel platform)
        {
            try
            {
                PlatformModel _platform = context.PlatformModels.Find(platform.PlatformID);
                context.Entry(_platform).CurrentValues.SetValues(platform);
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
        // ~PlatformRepository() {
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
