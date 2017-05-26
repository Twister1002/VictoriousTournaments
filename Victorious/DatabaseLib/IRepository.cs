﻿using System;
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
        TEntity Get(int id);
        void Add(TEntity entity);
        void Delete(int id);
        void Update(TEntity entity);
    }
}
