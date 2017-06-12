using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace DatabaseLib
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbSet<TEntity> dbSet;
        public DbSet<TEntity> DbSet
        {
            get { return DbSet; }
        }

        private VictoriousEntities context;
        public Repository(VictoriousEntities _context)
        {
            this.context = _context;
            this.dbSet = context.Set<TEntity>();

        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(int id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            dbSet.Remove(entityToDelete);
        }

        public void DeleteEntity(TEntity entity)
        {
            dbSet.Attach(entity);
            dbSet.Remove(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return dbSet;
        }

        public TEntity Get(int id)
        {
            return dbSet.Find(id);
        }

        public void Update(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateDetachCheck(TEntity entity)
        {
            context.Set<TEntity>().AddOrUpdate(entity);
        }

        public IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Single(predicate);
        }

        public void DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entity = dbSet.Single(predicate);
            dbSet.Remove(entity);
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
        // ~Repository() {
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
