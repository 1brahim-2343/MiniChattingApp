using MiniChattingApp.DataBaseRelated.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Entities.Concrete
{
    public class User:IEntity
    {
        public int Id { get; set; }
        public string? Type { get; set; } = "user";
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsVerified { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? OfflineSince { get; set; }
        public virtual List<Message>? SentMessages { get; set; }
        public virtual List<Message>? ReceivedMessages { get; set; }

        public virtual List<FileMessage>? ReceivedFiles { get; set; }
        public virtual List<FileMessage>? SentFiles { get; set; }

        public string? IpAddress { get; set; }
        public string? Port { get; set; }
    }
}
