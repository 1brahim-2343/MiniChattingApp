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
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsVerified { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? OfflineSince { get; set; }
        public List<Message>? SentMessages { get; set; }
        public List<Message>? ReceivedMessages { get; set; }

        public List<FileMessage>? ReceivedFiles { get; set; }
        public List<FileMessage>? SentFiles { get; set; }
    }
}
