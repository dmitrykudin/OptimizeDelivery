using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OptimizeDelivery.DataAccessLayer.GenericRepository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>, IDisposable
        where TEntity : class, IDisposable
    {
        public GenericRepository()
        {
            Context = new OptimizeDeliveryContext();
            DbSet = Context.Set<TEntity>();
        }

        public GenericRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        private DbContext Context { get; set; }

        private DbSet<TEntity> DbSet { get; set; }

        public void Dispose()
        {
            Context = null;
            DbSet = null;
        }

        public async Task<TEntity> Create(TEntity item)
        {
            var itemFromDb = DbSet.Add(item);
            await Context.SaveChangesAsync();
            return itemFromDb;
        }

        public async Task<TEntity> Get(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public void Remove(TEntity item)
        {
            DbSet.Remove(item);
            Context.SaveChanges();
        }

        public void Update(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
            Context.SaveChanges();
        }
    }
}