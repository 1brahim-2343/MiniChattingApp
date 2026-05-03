using MiniChattingApp.DataBaseRelated.DataAccess.Abstraction;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Abstract;
using MiniChattingApp.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Service.Concrete
{
    public class MessageService : IMessageService
    {
        private readonly IMessageDal _messageDal;
        private readonly IUserDal _userDal;

        public MessageService(IMessageDal messageDal, IUserDal userDal)
        {
            _messageDal = messageDal;
            _userDal = userDal;
        }
        private async Task<bool> MessageChecksAsync(Message entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Content))
                throw new RequiredFieldException("Message must have a content");
            if (entity.Content.Length >= 1000)
                throw new ArgumentException("Length of message content can not be greater than 1000 characters");
            if (entity.SenderId <= 0)
                throw new RequiredFieldException("Sender is required.");

            if (entity.ReceiverId <= 0)
                throw new RequiredFieldException("Receiver is required.");
            if (entity.SenderId == entity.ReceiverId)
                throw new LogicalErrorException("User can't send a message to himself");

            var sender = await _userDal.GetAsync(u => u.Id == entity.SenderId);
            if (sender == null)
                throw new NotFoundException("Sender not found");

            var receiver = await _userDal.GetAsync(u => u.Id == entity.ReceiverId);
            if (sender == null)
                throw new NotFoundException("Receiver not found");
            return true;
        }
        public async Task<Message> AddMessage(Message entity)
        {
            if(await MessageChecksAsync(entity))
                return await _messageDal.AddAsync(entity);
            return null!;
        }

        public async Task<bool> DeleteMessage(Message entity)
        {
            var messageToDelete = _messageDal.GetAsync(m => m.Id == entity.Id);
            if (messageToDelete == null)
                throw new NotFoundException("Message not found");
            return await _messageDal.DeleteAsync(entity);
        }

        public async Task<List<Message>>? GetAllMessagesAsync(Expression<Func<Message, bool>> filter = null!)
        {
            var messages = await _messageDal.GetAllAsync(filter);
            if (messages == null)
                throw new NotFoundException("Messages not found");
            return messages;
        }

        public async Task<Message>? GetMessageAsync(Expression<Func<Message, bool>> filter)
        {
            var message = await _messageDal.GetAsync(filter);
            if (message == null)
                throw new NotFoundException("Message not found");
            return message;
        }

        public async Task<Message> UpdateMessage(Message entity)
        {
            if (await MessageChecksAsync(entity))
                return await _messageDal.UpdateAsync(entity);
            return null!;
        }
    }
}
