using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Chat
{
    public class CreateChatDto
    {
        public int ChatId { get; set; }
        public int ChatBoxId { get; set; }
        public int ChatBy { get; set; }
        public string Content { get; set; } = null!;
    }
}
