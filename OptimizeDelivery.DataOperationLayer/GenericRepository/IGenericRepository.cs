using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OptimizeDelivery.DataAccessLayer.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Create(TEntity item);

        Task<TEntity> Get(int id);

        Task<IEnumerable<TEntity>> Get();

        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate);

        void Remove(TEntity item);

        void Update(TEntity item);
    }
}