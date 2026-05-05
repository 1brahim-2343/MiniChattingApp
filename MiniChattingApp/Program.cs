using Microsoft.EntityFrameworkCore;
using MiniChattingApp.DataBaseRelated.Data;
using MiniChattingApp.DataBaseRelated.DataAccess.Concrete.EntityFramework;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Concrete;
using MiniChattingApp.Helpers;
using Newtonsoft.Json;

namespace MiniChattingApp
{
    internal class Program
    {
        static async Task AddDatToDb(DbContext context, EFUserDal userDal, UserService userService,
            EFMessageDal messageDal, MessageService messageService,
            EFFileMessageDal fileMessageDal, FileMessageService fileMessageService)
        {
            var admin = new User { Email = "11@gm.com", Username = "admin" };
            await userDal.AddAsync(admin);
        }
        static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            using var dbContext = new MiniChattingDBContext();
            var userDal = new EFUserDal(dbContext);
            var userService = new UserService(userDal);

            var messageDal = new EFMessageDal(dbContext);
            var messageService = new MessageService(messageDal, userDal);

            var fileMessageDal = new EFFileMessageDal(dbContext);
            var fileMessageService = new FileMessageService(fileMessageDal, userDal);
            //await AddDatToDb(dbContext, userDal, userService, messageDal, messageService, fileMessageDal, fileMessageService);

            Chatting.DBContext = dbContext;
            Chatting.UserService = userService;
            Chatting.UserDal = userDal;
            Chatting.MessageDal = messageDal;
            await Chatting.Initiate();
            
        }
    }
}
