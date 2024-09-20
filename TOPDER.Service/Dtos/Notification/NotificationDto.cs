using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Notification
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int Uid { get; set; }
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
