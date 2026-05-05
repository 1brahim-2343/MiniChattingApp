using Microsoft.EntityFrameworkCore;
using MiniChattingApp.DataBaseRelated.Data;
using MiniChattingApp.DataBaseRelated.DataAccess.Abstraction;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Repositories.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.DataAccess.Concrete.EntityFramework
{
    public class EFUserDal : EntityFrameworkBase<User, MiniChattingDBContext>, IUserDal
    {
        public DbContext _context { get; set; }
        public EFUserDal(MiniChattingDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> UpdateUserStatusAsync(int id, bool isOnline)
        {
            var user = await GetAsync(u => u.Id == id);
            user!.IsOnline = isOnline;
            if (!isOnline)
                user.OfflineSince = DateTime.UtcNow.AddHours(4);
            else
                user.OfflineSince = null;
            return (_context.SaveChanges()) > 0;

        }
    }
}
