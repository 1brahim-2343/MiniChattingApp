using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.DataAccess.Abstraction
{
    public interface IMessageDal:IEntityRepository<Message>
    {

    }
}
