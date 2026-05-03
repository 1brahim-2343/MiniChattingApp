using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Abstract
{
    public interface IMessageService
    {
        Task<Message>? GetMessageAsync(Expression<Func<Message, bool>> filter);
        Task<List<Message>>? GetAllMessagesAsync(Expression<Func<Message, bool>> filter = null!);
        Task<Message> AddMessage(Message entity);
        Task<Message> UpdateMessage(Message entity);
        Task<bool> DeleteMessage(Message entity);
    }
}
