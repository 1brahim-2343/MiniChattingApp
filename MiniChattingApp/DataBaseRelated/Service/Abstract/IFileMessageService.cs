using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Abstract
{
    public interface IFileMessageService
    {
        Task<FileMessage>? GetFileMessageAsync(Expression<Func<FileMessage, bool>> filter);
        Task<List<FileMessage>>? GetFileMessagesAsync(Expression<Func<FileMessage, bool>> filter = null!);
        Task<FileMessage> AddFileMessageAsync(FileMessage entity);
        Task<FileMessage> UpdateFileMessage(FileMessage entity);
        Task<bool> DeleteFileMessage(FileMessage entity);
    }
}
