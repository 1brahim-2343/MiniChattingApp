using MiniChattingApp.DataBaseRelated.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Entities.Concrete
{
    public class FileMessage :IEntity
    {
        public int Id { get; set; }
        public string? Type { get; set; } = "fileMessage";

        public string? Path { get; set; }

        public int SenderId { get; set; }
        public virtual User? Sender { get; set; }

        public int ReceiverId{ get; set; }
        public virtual User? Receiver { get; set; }
    }
}
