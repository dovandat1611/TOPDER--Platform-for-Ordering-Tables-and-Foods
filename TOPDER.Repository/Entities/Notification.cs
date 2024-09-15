using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int Uid { get; set; }
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool? IsRead { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
    }
}
