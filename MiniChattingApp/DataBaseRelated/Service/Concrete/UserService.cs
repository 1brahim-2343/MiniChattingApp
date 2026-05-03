using MiniChattingApp.DataBaseRelated.DataAccess.Abstraction;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Abstract;
using MiniChattingApp.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Concrete
{
    public class UserService : IUserService
    {

        private readonly IUserDal _userDal;

        public UserService(IUserDal userDal)
        {
            _userDal = userDal;
        }
        public async Task<User> AddUserAsync(User entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new RequiredFieldException("Email is required");
            if (string.IsNullOrWhiteSpace(entity.Username))
                throw new RequiredFieldException("Username is required");
            var existing = _userDal.GetAsync(e => e.Email == entity.Email);
            if (existing != null)
                throw new DuplicateEntityException($"This entity already exists, Username: {entity.Username}");
            return await _userDal.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(User entity)
        {
            if (entity.IsOnline == true)
                throw new ActiveUserDeletionException("Can not delete the user that is still online");
            var entityToDelete = _userDal.GetAsync(e => e.Id == entity.Id);
            if (entityToDelete == null)
                throw new NotFoundException("User not found");
            return await _userDal.DeleteAsync(entity);
        }

        public async Task<List<User>>? GetAllAsync(Expression<Func<User, bool>> filter = null!)
        {
            var users = await _userDal.GetAllAsync(filter);
            if (users != null)
                return users;
            throw new NotFoundException("User not found");
        }

        public async Task<User>? GetUserAsync(Expression<Func<User, bool>> filter)
        {
            var user = await _userDal.GetAsync(filter);
            if (user != null)
                return user;
            throw new NotFoundException("User not found");
        }

        public async Task<User> UpdateAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Pass entity");
            return await _userDal.UpdateAsync(entity);
        }
    }
}
