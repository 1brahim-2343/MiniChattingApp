using Microsoft.EntityFrameworkCore;
using MiniChattingApp.DataBaseRelated.Data;
using MiniChattingApp.DataBaseRelated.Entities.Abstract;
using MiniChattingApp.DataBaseRelated.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Repositories.EntityFramework
{
    public class EntityFrameworkBase<TEntity, TContext>
        : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext
    {
        private readonly MiniChattingDBContext _context;

        public EntityFrameworkBase(MiniChattingDBContext context)
        {
            _context = context;
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var addedEntity = await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            var deletedEntity = _context.Set<TEntity>().Remove(entity);
            return (await _context.SaveChangesAsync()) > 0;

        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null!)
        {
            return filter == null ? await _context.Set<TEntity>().ToListAsync()
                : await _context.Set<TEntity>().Where(filter).ToListAsync();
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(filter);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityToUpdate = _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return entityToUpdate.Entity;
        }
    }
}
