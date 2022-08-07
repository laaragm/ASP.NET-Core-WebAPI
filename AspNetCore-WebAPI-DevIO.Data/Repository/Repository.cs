using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_WebAPI_DevIO.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly DatabaseContext DatabaseContext;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(DatabaseContext db)
        {
            DatabaseContext = db;
            DbSet = db.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate) 
            => await DbSet.AsNoTracking().Where(predicate).ToListAsync();

        public virtual async Task<TEntity> GetById(Guid id) => await DbSet.FindAsync(id);

        public virtual async Task<List<TEntity>> GetAll()=> await DbSet.ToListAsync();

        public virtual async Task Add(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Update(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Delete(Guid id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges() => await DatabaseContext.SaveChangesAsync();

        public void Dispose() => DatabaseContext?.Dispose();
    }
}