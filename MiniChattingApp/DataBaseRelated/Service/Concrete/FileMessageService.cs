using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Concrete
{
    public class FileMessageService : IFileMessageService
    {
        public Task<FileMessage> AddFileMessageAsync(FileMessage entity)
        {
            if(entity.)
        }

        public Task<bool> DeleteFileMessage(FileMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<FileMessage> GetFileMessageAsync(Expression<Func<FileMessage, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileMessage>> GetFileMessagesAsync(Expression<Func<FileMessage, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<FileMessage> UpdateFileMessage(FileMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
