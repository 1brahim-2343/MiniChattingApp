using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Abstract
{
    public interface IUserService
    {
        Task<User>? GetUserAsync(Expression<Func<User, bool>> filter);
        Task<List<User>>? GetAllAsync(Expression<Func<User, bool>> filter = null!);
        Task<User> AddUserAsync(User entity);

        Task<User> UpdateAsync(User entity);
        Task<bool> DeleteAsync(User entity);
    }
}
