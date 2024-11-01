using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.TableBookingSchedule
{
    public class TableBookingScheduleDto
    {
        public int ScheduleId { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
