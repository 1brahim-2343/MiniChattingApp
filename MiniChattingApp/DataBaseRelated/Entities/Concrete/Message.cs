using MiniChattingApp.DataBaseRelated.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Entities.Concrete
{
    public class Message : IEntity
    {
        public int Id { get; set; }
        public string? Type { get; set; } = "message";

        public string? Content { get; set; }
        public bool IsRead { get; set; }

        public int SenderId { get; set; }
        public virtual User? Sender { get; set; }

        public int ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }

        public DateTime? SentTime { get; set; }

    }
}
