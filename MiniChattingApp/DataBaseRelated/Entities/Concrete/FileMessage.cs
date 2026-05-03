using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Entities.Concrete
{
    public class FileMessage
    {
        public int Id { get; set; }
        public string? Path { get; set; }

        public int SenderId { get; set; }
        public User? SenderUser { get; set; }

        public int ReceiverId{ get; set; }
        public User? ReceiverUser { get; set; }
    }
}
