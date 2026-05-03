using MiniChattingApp.DataBaseRelated.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Repositories
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null!);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
