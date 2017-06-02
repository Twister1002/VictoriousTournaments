using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class BracketTypeRepository : IBracketTypeRepository, IDisposable
    {
        private VictoriousEntities context;
        public Exception interfaceException;

        public BracketTypeRepository()
        {
            context = new VictoriousEntities();
        }

        public BracketTypeRepository(VictoriousEntities context)
        {
            this.context = context;
        }

        public IList<BracketTypeModel> GetAllBracketTypes(bool returnOnlyActive)
        {
            List<BracketTypeModel> types = new List<BracketTypeModel>();
            try
            {
                if (returnOnlyActive)
                {
                    foreach (var bracketType in context.BracketTypeModels.ToList())
                    {
                        if (bracketType.IsActive)
                            types.Add(bracketType);
                    }
                }
                else
                {
                    types = context.BracketTypeModels.ToList();
                }
            }
            catch (Exception ex)
            {
                interfaceException = ex;
                WriteException(ex);
                types.Clear();
                return types;
            }
            return types;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public DbError UpdateBracketType(BracketTypeModel bracketType)
        {
            try
            {
                BracketTypeModel _bracketType = context.BracketTypeModels.Find(bracketType.BracketTypeID);
                context.Entry(_bracketType).CurrentValues.SetValues(bracketType);
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
        // ~BracketTypeRepository() {
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
