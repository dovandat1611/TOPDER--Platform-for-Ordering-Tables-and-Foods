using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Chat
    {
        public int ChatId { get; set; }
        public int ChatBoxId { get; set; }
        public DateTime ChatTime { get; set; }
        public string Content { get; set; } = null!;
        public int ChatBy { get; set; }

        public virtual ChatBox ChatBox { get; set; } = null!;
        public virtual User ChatByNavigation { get; set; } = null!;
    }
}
