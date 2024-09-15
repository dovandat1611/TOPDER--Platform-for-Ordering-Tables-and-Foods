using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class ChatBox
    {
        public ChatBox()
        {
            Chats = new HashSet<Chat>();
        }

        public int ChatBoxId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<Chat> Chats { get; set; }
    }
}
