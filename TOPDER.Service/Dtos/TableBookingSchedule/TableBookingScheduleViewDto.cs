using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.TableBookingSchedule
{
    public class TableBookingScheduleViewDto
    {
        public int ScheduleId { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public string? TableName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
