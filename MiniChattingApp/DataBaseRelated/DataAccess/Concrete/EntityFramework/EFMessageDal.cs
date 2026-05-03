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
    public class EFMessageDal : EntityFrameworkBase<Message, MiniChattingDBContext>, IMessageDal
    {
        public EFMessageDal(MiniChattingDBContext context) : base(context)
        {
            
        }
    }
}
