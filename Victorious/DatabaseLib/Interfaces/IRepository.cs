using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IRepository <TEntity> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate);
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        void Add(TEntity entity);
        void Delete(int id);
        void DeleteWhere(Expression<Func<TEntity, bool>> predicate);
        void DeleteEntity(TEntity entity);
        void Update(TEntity entity);
    }
}
