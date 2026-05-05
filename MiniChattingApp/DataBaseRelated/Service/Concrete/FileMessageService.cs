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
    public class FileMessageService : IFileMessageService
    {
        private readonly IFileMessageDal _fileMessageDal;
        private readonly IUserDal _userDal;

        public FileMessageService(IFileMessageDal FileMessageDal, IUserDal userDal)
        {
            _fileMessageDal = FileMessageDal;
            _userDal = userDal;
        }

        private async Task<bool> FileMessageChecksAsync(FileMessage entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Path))
                throw new RequiredFieldException("Message must have a path");

            if (entity.SenderId <= 0)
                throw new RequiredFieldException("Sender is required.");

            if (entity.ReceiverId <= 0)
                throw new RequiredFieldException("Receiver is required.");

            if (entity.SenderId == entity.ReceiverId)
                throw new LogicalErrorException("User can't send a file to himself");

            var sender = await _userDal.GetAsync(u => u.Id == entity.SenderId);
            if (sender == null)
                throw new NotFoundException("Sender not found");

            var receiver = await _userDal.GetAsync(u => u.Id == entity.ReceiverId);
            if (sender == null)
                throw new NotFoundException("Receiver not found");
            return true;
        }


        public async Task<FileMessage> AddFileMessageAsync(FileMessage entity)
        {
            if (await FileMessageChecksAsync(entity))
                return await _fileMessageDal.AddAsync(entity);
            return null!;
        }

        public async Task<bool> DeleteFileMessage(FileMessage entity)
        {
            var fileToDelete = _fileMessageDal.GetAsync(f => f.Id == entity.Id);
            if (fileToDelete == null)
                throw new NotFoundException("File not found, aborting delete...");
            return await _fileMessageDal.DeleteAsync(entity);
        }

        public async Task<FileMessage>? GetFileMessageAsync(Expression<Func<FileMessage, bool>> filter)
        {
            var fileMessage = await _fileMessageDal.GetAsync(filter);
            if (fileMessage == null)
                throw new NotFoundException("File not found, aborting search...");
            return fileMessage;
        }

        public async Task<List<FileMessage>>? GetFileMessagesAsync(Expression<Func<FileMessage, bool>> filter = null!)
        {
            var fileMessages = await _fileMessageDal.GetAllAsync(filter);
            if (fileMessages == null)
                throw new NotFoundException("Files not found, aborting search...");
            return fileMessages;
        }

        public async Task<FileMessage> UpdateFileMessage(FileMessage entity)
        {
            if (await FileMessageChecksAsync(entity))
                return await _fileMessageDal.UpdateAsync(entity);
            return null!;
        }
    }
}
