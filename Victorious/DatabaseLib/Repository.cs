using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    /// <summary>
    /// Templated repository. Contains methods ncessary for CRUD of an <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity"> The model type of repository. </typeparam>
    /// <remarks>
    /// A repositoty should be created for each model tpye. />
    /// </remarks>
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbSet<TEntity> dbSet;
        public DbSet<TEntity> DbSet
        {
            get { return DbSet; }
        }

        /// <summary>
        /// Database context
        /// </summary>
        private VictoriousEntities context;
        public Repository(VictoriousEntities _context)
        {
            this.context = _context;
            context.Configuration.AutoDetectChangesEnabled = true;
            this.dbSet = context.Set<TEntity>();

        }

        /// <summary>
        /// Inserts a new row into the datbase.
        /// </summary>
        /// <param name="entity"> The model to be inserted. </param>
        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Deletes a row from the database.
        /// </summary>
        /// <param name="id"> The primary key of the entity to delete from the database. </param>
        public void Delete(int id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Deletes a row from the database by the .
        /// </summary>
        /// <param name="entity"> The whole model to be deleted from the database. </param>
        public void DeleteEntity(TEntity entity)
        {
            dbSet.Attach(entity);
            dbSet.Remove(entity);
        }

        /// <summary>
        /// Retrieves all itmes of type <typeparamref name="TEntity"/> from the database.
        /// </summary>
        /// <returns> Returns IQueryable collection of <typeparamref name="TEntity"/>. </returns>
        public IQueryable<TEntity> GetAll()
        {
            return dbSet;
        }

        /// <summary>
        /// Retreive a single item from the database.
        /// </summary>
        /// <param name="id"> The primary key of the entity being retreived. </param>
        /// <returns> Returns a single <typeparamref name="TEntity"/>. </returns>
        public TEntity Get(int id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="entity"> The entity to be updated. </param>
        public void Update(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            //context.Entry(entity).CurrentValues.SetValues(entity);
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

        /// <summary>
        /// Retreives a single item from the database.
        /// </summary>
        /// <param name="predicate"> Criteria used to query item from database. </param>
        /// <returns> Returns a single <typeparamref name="TEntity"/> that matches the criteria set in the predicate. </returns>
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
